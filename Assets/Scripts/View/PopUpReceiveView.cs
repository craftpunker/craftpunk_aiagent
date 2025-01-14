
//领取道具通用界面
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopUpReceiveView : UiBase
{
    public struct ItemData {
        public string ItemCfgId;
        public int count;
    }

    List<ItemData> DataList;

    U3dObj ReceivePanel;
    U3dObj ReceivePanelContent;
    
    U3dObj SecondaryPanel;
    U3dObj BtnContine;
    Dictionary<GameObject, ItemBase> PropItems = new Dictionary<GameObject, ItemBase>();

    protected override void OnInit()
    {
        base.OnInit();
        ReceivePanel = Find("ReceivePanel");

        ReceivePanelContent = ReceivePanel.Find("ScrollView/Viewport/Content");
        ReceivePanelContent.ItemView.Init(PropItems);
        ReceivePanelContent.ItemView.SetRenderHandler(OnRenderItem);

        BtnContine = ReceivePanel.Find("BtnContine");
        SetOnClick(BtnContine.gameObject, OnBtnContine);
    }

    private void OnBtnContine()
    {
        UiMgr.Close<PopUpReceiveView>();
    }

    protected override void OnShow()
    {
        base.OnShow();

        DataList = (List<ItemData>)Args[0];
        ReceivePanelContent.ItemView.SetDataCount(DataList.Count);
    }


    void OnRenderItem(GameObject go, int index) {
        PropItem item = ItemBase.GetItem<PropItem>(go, PropItems);
        item.SetData(DataList[index]);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    protected override void OnDestroy()
    {
        ReceivePanelContent.ItemView.Release();
        base.OnDestroy();
    }
}
