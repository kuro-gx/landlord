/// <summary>
/// 单例基类
/// </summary>
public class Singleton<T> where T : new() {
    private static T _instance;
    private static readonly object _instanceLock = new object();

    public static T Instance {
        get {
            if (_instance == null) {
                lock (_instanceLock) {
                    if (_instance == null) {
                        _instance = new T();
                    }
                }
            }

            return _instance;
        }
    }
}