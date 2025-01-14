
using Battle;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class MainView : UiBase
{
    //class PageInfo {
    //    public string uiPath;
    //    public U3dObj btn;
    //}

    //private Button btn_openPutSoldier;
    Dictionary<RedPointBase, U3dObj> dictRedPoint;

    U3dObj BtnWeb3;

    //U3dObj btnPvp;
    //U3dObj btnPve;
    //U3dObj btnHome;
    public U3dObj btnShop;


    U3dObj btnLock1;
    U3dObj btnLock2;

    //Text texName;
    U3dObj layoutGoldTxt;
    U3dObj layoutDiamondsTxt;

    U3dObj btnOrder;
    U3dObj btnEmail;
    U3dObj btnTask;
    U3dObj btnUnion;
    U3dObj btnSet;

    U3dObj btnArmy;
    U3dObj btnSkill;

    U3dObj txtName;
    U3dObj imgHead;

    U3dObj LayoutPower;

    ToggleGroup troopsToggleGroup;
    List<Toggle> toggles = new List<Toggle>();

    //SortedDictionary<int, PageInfo> pageNames;

    List<PageInfo> pageNames;

    int currPageNum;

    protected override void OnInit()
    {
        dictRedPoint = new Dictionary<RedPointBase, U3dObj>();

        pageNames = new List<PageInfo>();
        //pageNames = new SortedDictionary<int, PageInfo>();

        //pageNames = new SortedDictionary<int, string> { { 0, UiMgr.GetUiAssetPath<HomeView>() },
        //    {1, UiMgr.GetUiAssetPath<PvpView>() }, { 2, UiMgr.GetUiAssetPath<PveView>()}};

        LayoutPower = Find("LayoutPower");

        BtnWeb3 = Find("BtnWeb3");
        SetOnClick(BtnWeb3, OnBtnWeb3);
        btnShop = Find("btnShop");
        SetOnClick(btnShop, OnBtnShop);

        //U3dObj LayoutRightBtns = Find("LayoutRightBtns");

        //btnHome = LayoutRightBtns.Find("btnHome");
        //SetOnClick(btnHome.gameObject, OnBtnOpenHome);
        //pageNames.Add(new PageInfo() { uiPath = UiMgr.GetUiAssetPath<HomeView>(), btn = btnHome });

        //btnPvp = LayoutRightBtns.Find("btnPvp");
        //SetOnClick(btnPvp.gameObject, OnBtnPvp);
        //pageNames.Add(new PageInfo() { uiPath = UiMgr.GetUiAssetPath<PvpView>(), btn = btnPvp });

        //btnPve = LayoutRightBtns.Find("btnPve");
        //SetOnClick(btnPve.gameObject, OnBtnPve);
        //pageNames.Add(new PageInfo() { uiPath = UiMgr.GetUiAssetPath<PveView>(), btn = btnPve });

        //btnLock1 = LayoutRightBtns.Find("btnLock1");
        //SetOnClick(btnLock1.gameObject, OnBtnLock);
        //btnLock2 = LayoutRightBtns.Find("btnLock2");
        //SetOnClick(btnLock2.gameObject, OnBtnLock);

        U3dObj LayoutLeftBtns = Find("LayoutLeftBtns");
        //btnShop = LayoutLeftBtns.Find("btnShop");
        //SetOnClick(btnShop, OnBtnShop);

        btnOrder = LayoutLeftBtns.Find("BtnOrder");
        SetOnClick(btnOrder.gameObject, OnBtnLock);

        btnEmail = LayoutLeftBtns.Find("BtnEmail");
        SetOnClick(btnEmail.gameObject, OnMail);

        btnTask = LayoutLeftBtns.Find("BtnTask");
        SetOnClick(btnTask.gameObject, OnBtnTask);
        //SetOnClick(btnTask.gameObject, OnBtnLock);

        btnSet = LayoutLeftBtns.Find("BtnSet");
        SetOnClick(btnSet.gameObject, OnBtnSet);

        btnUnion = LayoutLeftBtns.Find("BtnUnion");
        SetOnClick(btnUnion.gameObject, OnBtnLock);

        btnArmy = Find("BtnArmy");
        SetOnClick(btnArmy.gameObject, OnBtnOpenPutSoldier);

        btnSkill = Find("BtnSkill");
        SetOnClick(btnSkill.gameObject, OnBtnSkill);

        txtName = Find("LayoutHead/TexName");
        imgHead = Find("LayoutHead/ImgHead");

        layoutGoldTxt = Find("LayoutGold/TxtGold");
        layoutDiamondsTxt = Find("LayoutDiamonds/TxtDiamonds");

        var togGroupTroops = Find("TogGroupTroops");

        for (int i = 0; i < togGroupTroops.transform.childCount; i++)
        {
            var toggle = togGroupTroops.transform.GetChild(i).GetComponent<Toggle>();
            toggles.Add(toggle);
            int index = i;
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    var cur_deploy = GameData.instance.PermanentData["cur_deploy"];
                    var to_deploy = index + 1;
                    if (to_deploy != cur_deploy)
                    {
                        EventDispatcher<int>.instance.TriggerEvent(EventName.Scene_ChangeTroops, to_deploy);
                    }
                }
            });
        }

        dictRedPoint.Add(RedPointMgr.GetRedPoint(typeof(MissionRedPoint)), btnTask);
        dictRedPoint.Add(RedPointMgr.GetRedPoint(typeof(MailRedPoint)), btnEmail);
        dictRedPoint.Add(RedPointMgr.GetRedPoint(typeof(ShopRedPoint)), btnShop);
        dictRedPoint.Add(RedPointMgr.GetRedPoint(typeof(PutSoldierRedPoint)), btnArmy);
        //dictRedPoint.Add(RedPointMgr.GetRedPoint(typeof(PvpRedPoint)), btnPvp);
    }

    protected override void OnShow()
    {
        EventDispatcher<CmdEventData>.instance.AddEvent("power", OnPowerChange);
        LayoutPower.Text.text = GameData.instance.PermanentData["power"].ToString();

        if (AppConfig.Platform == "discord")
        {
            txtName.Text.text = GameData.instance.DiscordPlayerName;
            //if (GameData.instance.DiscordIcon != null)
            //{
            //    imgHead.Image.sprite = GameData.instance.DiscordIcon;
            //}
        }
        else
        {
            txtName.Text.text = GameData.instance.UserData["name"];
        }

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.bag_info, ChangeBaginfo);
        EventDispatcher<int>.instance.AddEvent(EventName.UI_OpenSoldierLevelUpView, OpenSoldierLevelUpView);

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.cur_deploy, CurDeployChange);

        troopsToggleGroup = Find("TogGroupTroops").transform.GetComponent<ToggleGroup>();

        CurDeployChange(EventName.cur_deploy, null);

        EventDispatcher<CmdEventData>.instance.TriggerEvent(EventName.bag_info);

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.record, (evtName, evt) =>
        {
            var ranksData = evt[0].JsonNode;
        });

        //EventDispatcher<int>.instance.AddEvent(EventName.UI_ChangeRightUI, ChangeRightUI);

        EventDispatcher<RedPointBase>.instance.AddEvent(EventName.Red_Point, OnRedPointChange);

        //UiMgr.Open<HomeView>();
        currPageNum = 0;

        if (GameData.instance.DailyData["shop_boxc"] == null) {
            RedPointMgr.GetRedPoint<ShopRedPoint>().CheckRed();
        }

        RefreshRedPoint();

        UiMgr.Open<TopShowResView>();
        UiMgr.Open<RightView>();
    }

    //red
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
        //BtnOptionList
        foreach (var item in dictRedPoint)
        {
            item.Value.SetRedPoint(item.Key.IsRed);
        }
    }

    //=======

    void OnPowerChange(string name, CmdEventData[] datas)
    {
        LayoutPower.Text.text = GameData.instance.PermanentData["power"].ToString();
    }

    private void CurDeployChange(string evtName, CmdEventData[] args)
    {
        for (int i = 0; i < troopsToggleGroup.transform.childCount; i++)
        {
            var tog = troopsToggleGroup.transform.GetChild(i).GetComponent<Toggle>();
            tog.group = troopsToggleGroup;
            tog.isOn = false;
            int curDeploy = GameData.instance.PermanentData["cur_deploy"];
            if (i + 1 == curDeploy)
            {
                tog.isOn = true;
            }
        }
    }

    //private void ChangeRightUI(string evtName, int[] args)
    //{
    //    int dir = args[0];

    //    currPageNum += dir;

    //    if (currPageNum < 0)
    //    {
    //        currPageNum = 0;
    //        return;
    //    }
    //    else if (currPageNum >= pageNames.Count)
    //    {
    //        currPageNum = pageNames.Count - 1;
    //        return;
    //    }

    //    if (currPageNum == 0)
    //    {
    //        OnBtnOpenHome();
    //    }
    //    else if (currPageNum == 1)
    //    {
    //        OnBtnPvp();
    //    }
    //    else if (currPageNum == 2)
    //    {
    //        OnBtnPve();
    //    }
    //}

    private void OpenSoldierLevelUpView(string evtName, int[] args)
    {
        OnBtnOpenSoldierLevelUp();
    }

    private void ChangeBaginfo(string evtName, CmdEventData[] args)
    {
        var gold = GameData.instance.PermanentData["bag_info"]["100001"]["num"];
        layoutGoldTxt.Text.text = gold == null ? "0" : gold;

        var diamonds = GameData.instance.PermanentData["bag_info"]["100002"]["num"];
        layoutDiamondsTxt.Text.text = diamonds == null ? "0" : diamonds;
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.bag_info, ChangeBaginfo);
        EventDispatcher<int>.instance.RemoveEvent(EventName.UI_OpenSoldierLevelUpView, OpenSoldierLevelUpView);
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.cur_deploy, CurDeployChange);
        //EventDispatcher<int>.instance.RemoveEvent(EventName.UI_ChangeRightUI, ChangeRightUI);
        EventDispatcher<RedPointBase>.instance.RemoveEvent(EventName.Red_Point, OnRedPointChange);
        EventDispatcher<CmdEventData>.instance.RemoveEvent("power", OnPowerChange);
        UiMgr.Hide<TopShowResView>();
        UiMgr.Hide<RightView>();
    }

    protected override void OnDestroy()
    {

    }

    void ShowPage(int page, params object[] args) 
    {
        currPageNum = page;
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
            else {
                UiMgr.Close(path);
                btn.SetSprite("MainAtlas", "ui_taskbar_a", true);
                btn.Text.transform.SetActiveEx(false);
                btn.FindTrans("Image").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            }
        }
    }

    //public void OnBtnOpenHome()
    //{
    //    ShowPage(0);
    //    EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, false);
    //}

    //private void OnBtnPvp()
    //{

    //    ShowPage(1);
    //    EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, false);
    //}

    //private void OnBtnPve()
    //{
    //    if (UiMgr.IsOpenView<PveView>())
    //        return;
    //    ShowPage(2);
    //}

    public void OnBtnShop()
    {
        UiMgr.Open<ShopView>();
    }
    
    public void OnBtnOpenPutSoldier()
    {
        GameData.instance.SelectedTroop = null;
        UiMgr.Open<PutSoldierView>();
        //ShowPage(UiMgr.GetUiAssetPath<PutSoldierView>());
        //EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, false);
        //UiMgr.Close<MainView>();
        // UIMgr.instance.CreatePanel<PutSoldierView>("PutSoldier/PutSoldierView");
        //UiMgr.Open<PutSoldierView>();
    }

    private void OnBtnOpenSoldierLevelUp()
    {
        UiMgr.Open<PutSoldierView>(null, GameData.instance.SelectedTroop);
        //EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, false);
        //UiMgr.Close<MainView>();
        // UIMgr.instance.CreatePanel<PutSoldierView>("PutSoldier/PutSoldierView");
        //UiMgr.Open<PutSoldierView>();
    }


    void OnBtnTask() {
        UiMgr.Close<SoldierLevelUpView>();
        UiMgr.Close<PutSoldierView>();
        UiMgr.Close<SkillView>();
        UiMgr.Open<MissionView>();
    }

    private void OnBtnSkill()
    {
        UiMgr.Open<SkillView>();
    }

    private void OnMail()
    {
        UiMgr.Close<SoldierLevelUpView>();
        UiMgr.Close<PutSoldierView>();
        UiMgr.Close<SkillView>();
        UiMgr.Open<MailView>();
    }

    private void OnBtnSet()
    {
        UiMgr.Close<SoldierLevelUpView>();
        UiMgr.Close<PutSoldierView>();
        UiMgr.Close<SkillView>();
        UiMgr.Open<SetUpView>();
    }

    private void OnBtnLock()
    {
        //UiMgr.Open<MailView>();
        //UiMgr.ShowTips("comming soon");
    }


    void OnBtnWeb3() {
        UiMgr.Open<Web3View>();
    }

    //===========guide

    public override RectTransform GetGuideTrans(GuideNode guideNode)
    {
        return base.GetGuideTrans(guideNode);
    }

    public override void TriggerGuide(GuideNode guideNode)
    {

        //if (guideNode.cfg["strArg2"] == "btnArmy")
        //{
        //    OnBtnOpenPutSoldier();
        //    guideNode.NextStep();
        //    return;
        //}

        base.TriggerGuide(guideNode);
    }
}
