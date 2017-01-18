using UnityEngine;
using System.Collections;
using System;

public class UIGrayScreen : UI3Wnd {

    // Use this for initialization
    public GameObject spriteBg = null;
    private System.Action _onClick = null;
    private GameObject _wnd;
    public UIPanel panel;
    private int _depth;
	void Start () {
        UIEventListener.Get(spriteBg).onClick = OnClickSpriteBg;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClickSpriteBg(GameObject go)
    {
        //Debug.LogError("OnClickSpriteBg go " + go.ToString() + " (Wnd == null) " + (Wnd == null));
        if(_wnd != null && OnClickEventCallback != null)
        {
            OnClickEventCallback();
        }
    }

    public System.Action OnClickEventCallback
    {
        get
        {
            return _onClick;
        }
        set
        {
            _onClick = value;
        }
    }

    public GameObject Wnd
    {
        get
        {
            return _wnd;
        }
        set
        {
            _wnd = value;
        }
    }

    public void SetDepth(int depth)
    {
        _depth = depth;
        UI3System.setDepth(gameObject, _depth);
    }
}
