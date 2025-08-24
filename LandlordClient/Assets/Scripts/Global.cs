using UnityEngine;

public class Global : MonoBehaviour {
    public static Global Instance;

    private void Awake() {
        Instance = this;

        NetSocketMgr.Instance.Init();
    }

    private void OnApplicationQuit() {
        NetSocketMgr.Instance.Disconnect();
    }
}