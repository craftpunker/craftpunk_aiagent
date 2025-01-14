using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public enum UIType
{
    //BG,
    Main,
    Normal,
    Top,
    Loading,
    Tips,
    // Menu,
    //

}

public struct UiInfo {
    public UIType uiType;
    public string assetPath;
    public Type type;
}

public class UiManager
{
    public GameObject UIRoot;
    public GraphicRaycaster graphicRaycaster;
    public Transform MainBg;

    private Dictionary<UIType, Transform> m_DictUiRoot;
    private Dictionary<Type, UiBase> m_DictUi;
    public List<UiBase> UpdateList;

    private Dictionary<Type, UiInfo> m_DictUiInfo;
    private Dictionary<string, UiInfo> m_DictNameUiInfo;


    public Dictionary<Type, UiInfo> DictUiInfo { 
        get { 
            return m_DictUiInfo; 
        } 
    }

    public MessageView MessageView;
    //public PnlTips PnlTips;

    public UiManager() {
        Init();
    }

    public void Init() {
        m_DictUi = new Dictionary<Type, UiBase>();
        m_DictUiRoot = new Dictionary<UIType, Transform>();
        UpdateList = new List<UiBase>();

        m_DictUiInfo = new Dictionary<Type, UiInfo>();
        m_DictNameUiInfo = new Dictionary<string, UiInfo>();

        UIRoot = GameObject.Find("UIRoot");
        graphicRaycaster = UIRoot.GetComponent<GraphicRaycaster>();
        GameObject.DontDestroyOnLoad(UIRoot);

       // m_DictUiRoot.Add(UIType.BG, UIRoot.transform.Find("BG"));

        m_DictUiRoot.Add(UIType.Main, UIRoot.transform.Find("MainNode")); 
        m_DictUiRoot.Add(UIType.Normal, UIRoot.transform.Find("NormalNode"));
        m_DictUiRoot.Add(UIType.Top, UIRoot.transform.Find("TopNode"));
        m_DictUiRoot.Add(UIType.Loading, UIRoot.transform.Find("LoadingNode"));
        m_DictUiRoot.Add(UIType.Tips, UIRoot.transform.Find("MessageNode"));

       // m_DictUiRoot.Add(UIType.Top, UIRoot.transform.Find("Top"));
      //  m_DictUiRoot.Add(UIType.Menu, UIRoot.transform.Find("Menu"));        
      //  m_DictUiRoot.Add(UIType.Loading, UIRoot.transform.Find("Loading"));
        

        MainBg = m_DictUiRoot[UIType.Main].transform.Find("MainBg");
        RegistUi();
        //PnlTips = (PnlTips)Open<PnlTips>();

        MessageView = Open<MessageView>() as MessageView;
    }

    void RegistUi() {
        // Main
        Regist<MainView>(UIType.Main, "MainView");

        // nomal
        Regist<PveView>(UIType.Main, "PveView");
        Regist<PvpView>(UIType.Main, "PvpView");

        Regist<PutSoldierView>(UIType.Normal, "PutSoldierView");
        Regist<SoldierLevelUpView>(UIType.Normal, "SoldierLevelUpView");

        Regist<BattleView>(UIType.Normal, "BattleView");

        Regist<F5View>(UIType.Normal, "F5View");


        Regist<MissionView>(UIType.Normal, "MissionView");
        Regist<ShopView>(UIType.Normal, "ShopView");
        Regist<ShopBoxOpenView>(UIType.Normal, "ShopBoxOpenView");
        Regist<ShopConfirmView>(UIType.Normal, "ShopConfirmView");

        Regist<Web3View>(UIType.Normal, "Web3View");

        // top
        
        Regist<RightView>(UIType.Normal, "RightView");

        // Loading

        Regist<LoadingView>(UIType.Loading, "LoadingView");
        Regist<PopUpReceiveView>(UIType.Loading, "PopUpReceiveView");
        Regist<PopUpConfirmView>(UIType.Loading, "PopUpConfirmView");
        Regist<TopShowResView>(UIType.Loading, "TopShowResView");
        Regist<GuideView>(UIType.Loading, "GuideView");

        // message
        Regist<MessageView>(UIType.Tips, "MessageView");

        //login
        Regist<LoginView>(UIType.Normal, "LoginView");

        //main
        Regist<HomeView>(UIType.Main, "HomeView");
        Regist<TimeChestView>(UIType.Normal, "TimeChestView");
        Regist<PowerChestView>(UIType.Normal, "PowerChestView");


        // pvp
        Regist<PvpInforView>(UIType.Normal, "PvpInforView");
        Regist<PvpRankingView>(UIType.Normal, "PvpRankingView");
        Regist<PvpHistoryView>(UIType.Normal, "PvpHistoryView");
        Regist<SetUpView>(UIType.Normal, "SetUpView");

        //mail
        Regist<MailView>(UIType.Normal, "MailView");
        Regist<SkillView>(UIType.Normal, "SkillView");

        // Regist<PnlMain>(UIType.Top, "PnlMain");

        // Menu
        //Regist<PnlMenu>(UIType.Menu, "PnlMenu");

        //GoldChest



        // tips
        //Regist<PnlTips>(UIType.Tips, "PnlTips");
        //m_DictUiInfo.Add(typeof(PnlTips), new UiInfo() { uiType = UIType.Tips, assetPath = "PnlTips", type = typeof(PnlTips) });

    }

    void Regist<T>(UIType uIType, string assetPath) where T : UiBase
    {
        UiInfo info = new UiInfo() { 
            uiType = uIType, 
            assetPath = assetPath, 
            type = typeof(T) 
        };

        m_DictUiInfo.Add(typeof(T), info);
        m_DictNameUiInfo.Add(assetPath, info);
    }

    public UiBase Open(string name, Action callBack = null, params object[] args)
    {
        if (m_DictNameUiInfo.TryGetValue(name, out UiInfo uiInfo)) {

            return Open(uiInfo, callBack, args);
        }
        return null;
    }

    public UiBase Open<T>(Action callBack = null, params object[] args) where T : UiBase, new()
    {
        if (m_DictUiInfo.TryGetValue(typeof(T), out UiInfo uiInfo))
        {
            return Open(uiInfo, callBack, args);
        }
        return null;
    }

    public UiBase Open(UiInfo uiInfo, Action callBack = null, params object[] args)
    {
        if (m_DictUi.TryGetValue(uiInfo.type, out UiBase ui))
        {
            if (ui.IsLoading == true) {
                return ui;
            }

            if(ui.uiStage == UiStage.show)
                return ui;

            ui.Show(args);
            callBack?.Invoke();
        }
        else
        {
            ui = Activator.CreateInstance(uiInfo.type) as UiBase;
            m_DictUi.Add(uiInfo.type, ui);
            AddUpdateList(ui, true);

            ui.IsLoading = true;
            ui.uiStage = UiStage.show;


            ResMgr.instance.LoadGameObjectAsync(uiInfo.assetPath, (go) =>
            {
                if (ui.uiStage == UiStage.close)
                {
                    ResMgr.Destroy(go);
                    //return false;
                }

                ui.IsLoading = false;
                go.transform.SetParent(m_DictUiRoot[uiInfo.uiType], false);
                ui.Init(go);

                var rect = go.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                ui.Show(args);
                //return true;
                callBack?.Invoke();
            });

            //ResManager.InstantiateAsync(uiInfo.assetPath, (go) =>
            //{
            //    if (ui.uiStage == UiStage.close)
            //    {
            //        return false;
            //    }

            //    ui.IsLoading = false;
            //    go.transform.SetParent(m_DictUiRoot[uiInfo.uiType], false);
            //    ui.Init(go);
            //    ui.Show(args);
            //    return true;
            //});

            //ResManager.LoadAssetAsync<GameObject>(uiInfo.assetPath, (go) =>
            //{
            //    go = GameObject.Instantiate(go);
            //    ui.IsLoading = false;
            //    go.transform.SetParent(m_DictUiRoot[uiInfo.uiType], false);
            //    ui.Init(go);
            //    ui.Show(args);
            //});
        }

        return ui;
    }

    public void Close<T>() where T:UiBase {
        if (m_DictUi.TryGetValue(typeof(T), out UiBase ui)) {
            ui.Close();
        }
    }

    public void Close(string name)
    {
        if (m_DictNameUiInfo.TryGetValue(name, out UiInfo uiInfo))
        {
            if (m_DictUi.TryGetValue(uiInfo.type, out UiBase ui))
            {
                ui.Close();
            }
        }
    }

    public void Update()
    {
        for (int i = UpdateList.Count - 1; i >= 0; i--) {

            UiBase ui = UpdateList[i];
            ui.Update();

            if (ui.uiStage == UiStage.close) {
                if (ui.closeTime <= Time.time) {
                    Destroy(ui);
                }
            }
        }
    }

    public void Destroy(UiBase ui)
    {
        ui.Destroy();
        UpdateList.Remove(ui);
        m_DictUi.Remove(ui.GetType());
    }

    public void AddUpdateList(UiBase ui, bool isAdd)
    {
        if (isAdd)
        {
            if (UpdateList.Contains(ui))
            {
                return;
            }
            else
            {
                UpdateList.Add(ui);
            }
        }
        else {
            UpdateList.Remove(ui);
            //UpdateList.RemoveAll(x => x == ui);
        }
    }

    public void HideView<T>() where T : UiBase
    {
        if (m_DictUi.TryGetValue(typeof(T), out UiBase ui))
        {

            ui.Hide();
            //bool isShowMainBg = false;
            //bool isPauseGame = false;

            //foreach (var item in m_DictUi.Values)
            //{
            //    if (item.uiStage == UiStage.show)
            //    {
            //        if (item.isShowMainBg) {
            //            isShowMainBg = true;
            //        }

            //        if (item.isPauseGame)
            //        {
            //            isPauseGame = true;
            //        }
            //    }
            //}
            //MainBg.SetActiveEx(isShowMainBg);

            //if (!isPauseGame) {
            //    Main.Instance.SetStage(GameStage.Run);
            //}
        }
    }

    public UiBase GetView(string name)
    {
        if (m_DictNameUiInfo.TryGetValue(name, out UiInfo info))
        {
            if (m_DictUi.TryGetValue(info.type, out UiBase ui))
            {
                return ui;
            }
        }
        return null;
    }

    public T GetView<T>() where T : UiBase
    {
        if (m_DictUi.TryGetValue(typeof(T), out UiBase ui))
        {
            return (T)ui;
        }
        return null;
    }

    public string GetUiAssetPath<T>() {
        return m_DictUiInfo[typeof(T)].assetPath;
    }

    public string GetUiAssetPath(Type type)
    {
        return m_DictUiInfo[type].assetPath;
    }

    public void CloseAll()
    {
        foreach (var kv in m_DictNameUiInfo)
        {
            if (kv.Value.type == typeof(MessageView)) {
                continue;
            }

            Close(kv.Key);
        }
    }

    public void ShowTips(string tips)
    {
        MessageView.ShowTip(tips);
    }

    public void SetUIType(UIType uiType, UiBase view)
    {
        if (uiType == UIType.Normal)
        {
            view.transform.SetParent(UIRoot.transform.Find("NormalNode"));
        }
        else if (uiType == UIType.Loading)
        {
            view.transform.SetParent(UIRoot.transform.Find("LoadingNode"));
        }
    }

    public void CreateUISpecialEffect(int animCfgid, Action<UISpecialEffect> callBack)
    {
        var animData = AnimMgr.instance.GetAnimData(animCfgid);

        ResMgr.instance.LoadGameObjectAsync(animData.Prefab, (obj) =>
        {
            var ise = obj.GetComponent<UISpecialEffect>();
            ise.AnimData = animData;
            var trans = obj.transform;
            trans.localScale = Vector3.one;
            var rect = trans.GetComponent<RectTransform>();
            rect.sizeDelta = animData.PrefabSize;
            rect.GetComponent<Image>().raycastTarget = false;

            callBack?.Invoke(ise);
        });
    }
}
