using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web3ChainItem : ItemBase
{
    public JSONNode cfg;

    U3dObj ImgChainIcon;
    U3dObj TxtChain;

    Action<JSONNode> clickCallback;

    protected override void OnInit(params object[] args)
    {
        ImgChainIcon = Find("ImgChainIcon");
        TxtChain = Find("TxtChain");

        SetOnClick(gameObject, OnClickItem);
    }

    public void SetData(JSONNode cfg) { 
        this.cfg = cfg;
        Refresh();
    }

    public void OnClickItem()
    {
        clickCallback?.Invoke(cfg);
    }

    public void SetClickCallBack(Action<JSONNode> callback) {
        clickCallback = callback;
    }

    protected override void OnRefresh()
    {
        ImgChainIcon.SetSprite("Web3IconAtlas", cfg["icon"]);
        TxtChain.Text.text = cfg["name"];
    }

    protected override void OnRelease()
    {
        clickCallback = null;
    }
}
