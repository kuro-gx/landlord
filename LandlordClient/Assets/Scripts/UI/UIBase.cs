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
    
    #region 滑动、点击事件

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

    // 是否是滑动选择状态
    private bool _isSlideSelect;
    /// <summary>
    /// 滑动选择
    /// </summary>
    public void OnSlideSelect(GameObject targetEl, GameObject rootEl, Action<GameObject> cb, Action<GameObject> endCb) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        // 点击目标触发
        OnClickDown(targetEl, clickEl => {
            _isSlideSelect = false;
            cb(clickEl);
        });

        // 鼠标进入目标触发
        OnEnter(targetEl, enterEl => {
            if (!Input.GetMouseButton(0)) {
                return;
            }
            _isSlideSelect = true;
            cb(enterEl);
        });
#else
        OnEnter(gameObject, enterEl => {
            _isSlideSelect = true;
            cb(enterEl);
        });
#endif

        // 鼠标在目标物体抬起触发
        OnClickUp(targetEl, endCb);
        // 鼠标在rootEl组件内抬起触发
        OnClickUp(rootEl, upEl => {
            if (_isSlideSelect) {
                endCb(upEl);
                _isSlideSelect = false;
            }
        });
    }

    #endregion
}