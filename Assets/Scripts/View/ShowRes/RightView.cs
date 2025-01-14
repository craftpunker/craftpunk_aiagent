
using Battle;
using System.Collections.Generic;
using UnityEngine;

class PageInfo
{
    public string uiPath;
    public U3dObj btn;
}

public class RightView : UiBase
{
    int currPageNum;
    List<PageInfo> pageNames;

    U3dObj btnPvp;
    public U3dObj btnPve;
    U3dObj btnHome;

    Dictionary<RedPointBase, U3dObj> dictRedPoint;

    protected override void OnInit()
    {
        base.OnInit();
        pageNames = new List<PageInfo>();
        U3dObj LayoutRightBtns = Find("LayoutRightBtns");

        btnHome = LayoutRightBtns.Find("btnHome");
        SetOnClick(btnHome.gameObject, OnBtnOpenHome, "ui_rightButton_click");
        pageNames.Add(new PageInfo() { uiPath = UiMgr.GetUiAssetPath<HomeView>(), btn = btnHome });

        btnPvp = LayoutRightBtns.Find("btnPvp");
        SetOnClick(btnPvp.gameObject, OnBtnPvp, "ui_rightButton_click");
        pageNames.Add(new PageInfo() { uiPath = UiMgr.GetUiAssetPath<PvpView>(), btn = btnPvp });

        btnPve = LayoutRightBtns.Find("btnPve");
        SetOnClick(btnPve.gameObject, OnBtnPve, "ui_rightButton_click");
        pageNames.Add(new PageInfo() { uiPath = UiMgr.GetUiAssetPath<PveView>(), btn = btnPve });

        dictRedPoint = new Dictionary<RedPointBase, U3dObj>();
        dictRedPoint.Add(RedPointMgr.GetRedPoint(typeof(PvpRedPoint)), btnPvp);
    }

    protected override void OnShow()
    {
        base.OnShow();
        EventDispatcher<int>.instance.AddEvent(EventName.UI_ChangeRightUI, ChangeRightUI);
        EventDispatcher<RedPointBase>.instance.AddEvent(EventName.Red_Point, OnRedPointChange);
        currPageNum = -1;

        if (BattleMgr.instance.PvMode == PvMode.Pvp)
        {
            OnBtnPvp();
        }
        else if (BattleMgr.instance.PvMode == PvMode.Pve)
        {
            OnBtnPve();
        }
        else
        {
            OnBtnOpenHome();
        }
        RefreshRedPoint();
    }

    void OnRedPointChange(string evtName, RedPointBase[] evt)
    {
        RedPointBase redpoint = evt[0];
        if (dictRedPoint.TryGetValue(redpoint, out U3dObj obj))
        {
            obj.SetRedPoint(redpoint.IsRed);
        }
    }

    void RefreshRedPoint()
    {
        foreach (var item in dictRedPoint)
        {
            item.Value.SetRedPoint(item.Key.IsRed);
        }
    }

    public void OnBtnOpenHome()
    {
        ShowPage(0);
        EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, false);
    }

    private void OnBtnPvp()
    {

        ShowPage(1);
        EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, false);
    }

    public void OnBtnPve()
    {
        if (UiMgr.IsOpenView<PveView>())
            return;

        ShowPage(2);
    }

    void ShowPage(int page, params object[] args)
    {
        if (currPageNum == page)
            return;

        currPageNum = page;

        var UIRoot = GameObject.Find("UIRoot");
        var rightView = UiMgr.GetView<RightView>();
        UiMgr.SetUIType(UIType.Normal, rightView);

        //var normalNode = UIRoot.transform.Find("NormalNode");
        //var index = normalNode.Find("MainView").GetSiblingIndex() + 1;
        //rightView.transform.SetSiblingIndex(index);

        for (int i = 0; i < pageNames.Count; i++)
        {
            string path = pageNames[i].uiPath;
            U3dObj btn = pageNames[i].btn;

            if (page == i)
            {
                UiMgr.Open(path, null, args);
                btn.SetSprite("MainAtlas", "ui_taskbar_bulge_a", true);
                btn.Text.transform.SetActiveEx(true);
                btn.FindTrans("Image").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 9);
            }
            else
            {
                UiMgr.Close(path);
                btn.SetSprite("MainAtlas", "ui_taskbar_a", true);
                btn.Text.transform.SetActiveEx(false);
                btn.FindTrans("Image").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            }
        }
    }

    private void ChangeRightUI(string evtName, int[] args)
    {
        int dir = args[0];

        currPageNum += dir;

        if (currPageNum < 0)
        {
            currPageNum = 0;
            return;
        }
        else if (currPageNum >= pageNames.Count)
        {
            currPageNum = pageNames.Count - 1;
            return;
        }

        if (currPageNum == 0)
        {
            OnBtnOpenHome();
        }
        else if (currPageNum == 1)
        {
            OnBtnPvp();
        }
        else if (currPageNum == 2)
        {
            OnBtnPve();
        }
    }

    protected override void OnHide()
    {
        base.OnHide();

        EventDispatcher<int>.instance.RemoveEvent(EventName.UI_ChangeRightUI, ChangeRightUI);
        EventDispatcher<RedPointBase>.instance.RemoveEvent(EventName.Red_Point, OnRedPointChange);
    }
}
