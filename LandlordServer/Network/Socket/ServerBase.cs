using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Google.Protobuf;

public class ServerBase {
    protected Socket _socket;
    private byte[] _buffer = new byte[1024 * 4]; // 接收数据的缓冲区
    protected ConnState _connState; // 连接状态

    protected Dictionary<int, IContainer> _cmdDict = new Dictionary<int, IContainer>();
    public Action<int, ByteString> OnReceiveMsg;

    /// <summary>
    /// 接收服务器 or 客户端发送的数据
    /// </summary>
    protected void BeginReceive() {
        _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceiveHandle, null);
    }

    /// <summary>
    /// 处理服务器 or 客户端发送过来的数据
    /// </summary>
    private void OnReceiveHandle(IAsyncResult ar) {
        try {
            // 本次接收数据的总字节数
            int len = _socket.EndReceive(ar);
            if (len > 0) {
                while (true) {
                    // 消息体长度，占2字节，0-65535
                    ushort msgLen = BitConverter.ToUInt16(_buffer, 0);
                    if (len >= msgLen + 2) {
                        byte[] data = NetUtils.Instance.ParseData(_buffer, msgLen);
                        // 数据不为空，则反序列化数据
                        if (data != null) {
                            BasePackage package = BasePackage.Parser.ParseFrom(data);
                            Console.WriteLine(package.ToString());
                            CommandHandle(package);
                        }

                        len -= (msgLen + 2);
                        // 如何 len大于0，则发生了粘包
                        if (len > 0) {
                            Buffer.BlockCopy(_buffer, msgLen + 2, _buffer, 0, len);
                        }
                    } else {
                        break;
                    }
                }

                // 继续接收数据
                BeginReceive();
            }
        } catch (Exception e) {
            DisconnectHandle();
            Console.WriteLine(e.Message);
        }
    }

    // 断线处理
    protected virtual void DisconnectHandle() {
        _connState = ConnState.Disconnected;
        if (_socket != null) {
            _socket.Close();
            _socket = null;
        }
    }
    
    protected virtual void CommandHandle(BasePackage package) {
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="package">数据包</param>
    /// <param name="code">协议码</param>
    /// <param name="data">数据</param>
    public void SendData(BasePackage package, int code = -1, ByteString data = null) {
        try {
            if (code != -1) {
                package.Code = code;
            }

            if (data != null) {
                package.Data = data;
            }

            _socket.Send(NetUtils.Instance.CreateData(package.ToByteArray()));
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }

    public void SendData(int code = -1, ByteString data = null) {
        BasePackage package = new BasePackage();
        SendData(package, code, data);
    }

    public void SendError(BasePackage package, CmdCode code) {
        ErrMsg err = new ErrMsg() {
            Code = code
        };
        SendData(package, NetDefine.CMD_ErrCode, err.ToByteString());
    }
}