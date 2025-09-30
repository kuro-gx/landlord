using System;
using UnityEngine;

public abstract class UIBase : MonoBehaviour {
    protected abstract void Init();

    public void Show(bool isShow = true) {
        gameObject.SetActive(isShow);
    }

    protected virtual void Start() {
        Init();
    }

    protected void ShowSystemTips(string msg, Color color) {
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
    
    protected T GetOrAddComponent<T>(GameObject obj) where T : Component {
        T cpt = obj.GetComponent<T>();
        if (cpt == null) {
            cpt = obj.AddComponent<T>();
        }

        return cpt;
    }
    
    #region 设置预制体的点击事件

    protected void OnEnter(GameObject go, Action<GameObject> callback) {
        var listener = GetOrAddComponent<ClickListener>(go);
        listener.OnEnter = callback;
    }

    protected void OnClickDown(GameObject go, Action<GameObject> callback) {
        var listener = GetOrAddComponent<ClickListener>(go);
        listener.OnClickDown = callback;
    }

    protected void OnClickUp(GameObject go, Action<GameObject> callback) {
        var listener = GetOrAddComponent<ClickListener>(go);
        listener.OnClickUp = callback;
    }

    #endregion
}