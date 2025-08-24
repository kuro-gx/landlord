using System.Collections.Generic;
using Google.Protobuf;

public delegate void OnActionHandler(ByteString data);

/// <summary>
/// 将服务端发送的数据派发出去
/// </summary>
public class SocketDispatcher : Singleton<SocketDispatcher> {
    private readonly Dictionary<int, OnActionHandler> _actionDict = new();

    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="code">协议码</param>
    /// <param name="handler">事件委托</param>
    public void AddEventHandler(int code, OnActionHandler handler) {
        if (!_actionDict.ContainsKey(code) && handler != null) {
            _actionDict.Add(code, handler);
        }
    }

    /// <summary>
    /// 删除事件
    /// </summary>
    /// <param name="code">协议码</param>
    public void RemoveEventHandler(int code) {
        if (_actionDict.ContainsKey(code)) {
            _actionDict.Remove(code);
        }
    }

    /// <summary>
    /// 派发事件
    /// </summary>
    /// <param name="code">协议码</param>
    /// <param name="data">消息数据</param>
    public void DispatcherEvent(int code, ByteString data) {
        if (_actionDict.ContainsKey(code)) {
            _actionDict[code]?.Invoke(data);
        }
    }
}