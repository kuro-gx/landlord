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
}