using System.Collections.Generic;
using System.Threading;

/// <summary>
/// 客户端连接对象管理类
/// </summary>
public class SessionMgr : Singleton<SessionMgr> {
    private int _instanceInter;
    private Dictionary<int, Session> _sessionDict = new Dictionary<int, Session>();

    public void AddSession(Session session, int sessionId = -1) {
        if (sessionId <= 0) {
            sessionId = GetInstanceInter();
        }

        if (!_sessionDict.ContainsKey(sessionId)) {
            session.SessionId = sessionId;
            _sessionDict.Add(sessionId, session);
        }
    }

    /// <summary>
    /// 移除客户端连接对象
    /// </summary>
    public void RemoveSession(int sessionId) {
        if (_sessionDict.ContainsKey(sessionId)) {
            _sessionDict.Remove(sessionId);
        }
    }

    /// <summary>
    /// 获取客户端连接对象
    /// </summary>
    public Session GetSession(int sessionId) {
        if (_sessionDict.ContainsKey(sessionId)) {
            return _sessionDict[sessionId];
        }

        return null;
    }

    /// <summary>
    /// 获取客户端所有连接的数量
    /// </summary>
    public int GetSessionCount() {
        return _sessionDict.Count;
    }

    private int GetInstanceInter() {
        return Interlocked.Increment(ref _instanceInter);
    }
}