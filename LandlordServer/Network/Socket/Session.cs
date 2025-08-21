using System;
using System.Net.Sockets;

public class Session {
    private Socket _socket;
    private byte[] buffer = new byte[1024 * 4]; // 接收数据的缓冲区

    /// <summary>
    /// 接收客户端数据
    /// </summary>
    /// <param name="socket">客户端连接对象</param>
    public void ReceiveData(Socket socket) {
        _socket = socket;
        _socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceiveHandle, null);
    }

    /// <summary>
    /// 处理客户端发送的数据
    /// </summary>
    private void OnReceiveHandle(IAsyncResult ar) {
        // 本次接收数据的总字节数
        int len = _socket.EndReceive(ar);
        if (len > 0) {
            while (true) {
                
            }
        }
    }
}