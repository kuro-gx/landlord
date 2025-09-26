using UnityEngine;

public class Global : MonoBehaviour {
    public static Global Instance;
    // 登录用户的信息
    public static LoginResponse LoginUser;
    // 登录用户的坐位索引
    public static int CurPos = -1;

    private void Awake() {
        Instance = this;

        NetSocketMgr.Instance.Init();
        // 场景切换时保留本组件
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit() {
        NetSocketMgr.Instance.Disconnect();
    }
}