using UnityEngine;

public abstract class UIBase : MonoBehaviour {
    protected virtual void Awake() {
    }

    public abstract void Init();

    public void Show(bool isShow = true) {
        gameObject.SetActive(isShow);
    }

    protected virtual void Start() {
        Init();
    }

    protected virtual void Update() {
    }
    
    public void ShowSystemTips(string msg, Color color) {
        GameObject obj = Resources.Load<GameObject>("Prefabs/SystemTips");
        if (obj == null) {
            return;
        }
        
        GameObject go = Instantiate(obj, GameObject.Find("Canvas").transform, true);
        go.transform.localPosition = new Vector2(0, 410);
        go.transform.localScale = Vector3.one;
        SystemTips tips = go.GetComponent<SystemTips>();
        if (tips != null) {
            tips.RefreshUI(msg, color);
        }
    }
}