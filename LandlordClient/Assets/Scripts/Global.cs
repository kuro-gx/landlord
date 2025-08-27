using UnityEngine;

public class Global : MonoBehaviour {
    public static Global Instance;
    public static LoginRes LoginUser;

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