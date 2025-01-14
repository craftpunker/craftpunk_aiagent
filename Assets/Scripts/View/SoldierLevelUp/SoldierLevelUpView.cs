using Battle;
using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoldierLevelUpView : UiBase
{
    public int SoldierCfgId;

    public JSONNode soldierCfg;


    U3dObj LayoutArmy;
    public U3dObj BtnCloseView;
    U3dObj ArmyScrollView;
    Dictionary<GameObject, ItemBase> _ItemList = new Dictionary<GameObject, ItemBase>();
    SoldierLevelArmyDragItem dragItem;

    U3dObj LayoutLevelUp;
    public U3dObj BtnCloseLevelUp;

    U3dObj Icon;

    U3dObj Slider;

    public U3dObj BtnUpgrade;
    U3dObj TextCost;

    U3dObj LayoutAttr;
    U3dObj LayoutLevelUpTxtTitle;
    U3dObj LayoutArmyTxtTitle;

    SoldierLevelUpAttrItem AtkAttrItem;
    SoldierLevelUpAttrItem BloodAttrItem;
    SoldierLevelUpAttrItem PopuLationAttrItem;

    private List<JSONNode> soldierDatas = new List<JSONNode>();

    private string soldierLevelUPId;
    public SoldierLevelArmyItem CurrSelectedArmyItem;
    U3dObj TxtItemCount;
    U3dObj TxtContent;

    U3dObj TxtLevel;

    //U3dObj Icon;

    int dataCount;
    bool canUpgrade;


    public bool IsDragUI;
    public Transform UIDragSoliderTrans; //ÍÏ±øÊµÌå
    private Vector3 dragSoliderhitPos;

    private List<DragSoldier> dragSoldierList = new List<DragSoldier>();
    private RaycastHit hit;

    public float rayLength = 100f;
    int layerMask;

    //private bool isLayoutAdd;

    protected override void OnInit()
    {

        LayoutArmy = Find("LayoutArmy");

        BtnCloseView = LayoutArmy.Find("BtnCloseView");
        SetOnClick(BtnCloseView.gameObject, OnBtnClose);

        LayoutLevelUpTxtTitle = Find("LayoutLevelUp/TxtTitle");
        LayoutArmyTxtTitle = Find("LayoutArmy/TxtTitle");

        ArmyScrollView = LayoutArmy.Find("ArmyScrollView");
        ArmyScrollView.LoopScrollView.Init(_ItemList);
        ArmyScrollView.LoopScrollView.SetRenderHandler(OnRenderItem);

        LayoutLevelUp = Find("LayoutLevelUp");
        BtnCloseLevelUp = LayoutLevelUp.Find("BtnCloseLevelUp");
        SetOnClick(BtnCloseLevelUp.gameObject, OnBtnCloseLevelUp);

        Icon = LayoutLevelUp.Find("Icon");
        Slider = LayoutLevelUp.Find("Slider");
        TxtItemCount = Slider.Find("TxtItemCount");

        BtnUpgrade = LayoutLevelUp.Find("BtnUpgrade");
        SetOnClick(BtnUpgrade.gameObject, OnBtnUpgrade);

        TextCost = BtnUpgrade.Find("NoMax/TextCost");

        LayoutAttr = LayoutLevelUp.Find("LayoutAttr");
        TxtContent = LayoutArmy.Find("TxtContent");

        TxtLevel = LayoutLevelUp.Find("ImgLevelBG/TxtLevel");

        AtkAttrItem = ItemBase.GetItem<SoldierLevelUpAttrItem>(LayoutAttr.FindTrans("AtkAttrItem"));
        BloodAttrItem = ItemBase.GetItem<SoldierLevelUpAttrItem>(LayoutAttr.FindTrans("BloodAttrItem"));
        PopuLationAttrItem = ItemBase.GetItem<SoldierLevelUpAttrItem>(LayoutAttr.FindTrans("PopuLationAttrItem"));

        dragItem = ItemBase.GetItem<SoldierLevelArmyDragItem>(FindGameObject("SoldierLevelArmyItem"));
        dragItem.transform.SetActiveEx(false);

    }

    //Args[0] = cfgid
    protected override void OnShow()
    {
        LayoutLevelUp.SetActive(false);
        LayoutArmy.SetActive(true);

        layerMask = LayerMask.GetMask("RayCast");
        IsDragUI = false;


        SoldierCfgId = (int)Args[0];
        var table = GameData.instance.TableJsonDict["SoldierConf"];

        foreach (JSONNode item in table)
        {
            if ((int)item["cfgId"] == SoldierCfgId)
            {
                soldierCfg = item;
            }
        }

        TxtContent.Text.text = soldierCfg["desc"];

        ResetScrollViewDataCount();

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.bag_info, UpdateBagInfo);

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.battle_troop, UpdateBattleTroop);
    }

    private void ResetScrollViewDataCount()
    {
        //isLayoutAdd = false;
        soldierDatas.Clear();
        var troop_info = GameData.instance.PermanentData["troop_info"];
        dataCount = 0;
        foreach (var troop in troop_info)
        {
            if (troop.Value["cfgId"] == soldierCfg["cfgId"])
            {
                dataCount++;
                soldierDatas.Add(troop.Value);
            }
        }
        PhysicsRayMgr.instance.MouseOnUI += OnMouseOnUI;
        PhysicsRayMgr.instance.Mouse += OnMouse;

        int itemCount = (int)Math.Ceiling((float)dataCount / 3);
        if (itemCount == 0)
        {
            ArmyScrollView.LoopScrollView.SetDataCount(1);
        }
        else
        {
            int surplus = dataCount % 3;
            surplus = surplus == 0 ? 1 : 0;
            ArmyScrollView.LoopScrollView.SetDataCount(itemCount + surplus);
        }

        if (CurrSelectedArmyItem != null)
        {
            UpdateLevelUpAttr(CurrSelectedArmyItem.TroopInfo);
        }

        LayoutArmyTxtTitle.Text.text = soldierCfg["name"];
    }

    private void UpdateBattleTroop(string evtName, CmdEventData[] args)
    {
        ResetScrollViewDataCount();
        //ArmyScrollView.LoopScrollView.SetRenderHandler(OnRenderItem);
    }

    private void UpdateBagInfo(string evtName, CmdEventData[] args)
    {
        ResetScrollViewDataCount();
        //ArmyScrollView.LoopScrollView.SetRenderHandler(OnRenderItem);
    }

    public void StartLevelUp(JSONNode troopInfo) 
    {
        soldierLevelUPId = troopInfo["id"];
        LayoutLevelUp.SetActive(true);
        DoTweenAnimUtil.ScaleAni(LayoutLevelUp.transform, 0, 0.3f, DG.Tweening.Ease.OutQuad, Vector3.zero);
        //TxtTitle.Text
        LayoutArmy.SetActive(false);

        //Icon.SetSprite()
        Icon.SetSprite("SoldierIconAtlas", soldierCfg["icon"], true);

        UpdateLevelUpAttr(troopInfo);
    }

    private void UpdateLevelUpAttr(JSONNode troopInfo)
    {
        int level = troopInfo["level"];
        var soldierConf = GameData.instance.TableJsonDict["SoldierConf"];
        //TxtTitle.Text
        JSONNode nowCfg = null;
        JSONNode nextLevelCfg = null;
        var conf = GameData.instance.TableJsonDict["SoldierBattleConf"];
        string cfgid = troopInfo["cfgId"];

        var soldierData = soldierConf[cfgid];

        TxtLevel.Text.text = $"{level}/{soldierData["maxLevel"]}";
        LayoutLevelUpTxtTitle.Text.text = soldierData["name"];
        nowCfg = TableUtils.FindSoldierBattleConfByCfgidLevel(SoldierCfgId, level);
        var levelUpItemCfgId = nowCfg["levelUpNeedItem"][1][0];
        var bagItemInfo = GameData.instance.PermanentData["bag_info"][levelUpItemCfgId.ToString()];
        nextLevelCfg = TableUtils.FindSoldierBattleConfByCfgidLevel(SoldierCfgId, level + 1);
        if (bagItemInfo == null)
        {
            TxtItemCount.Text.text = $"{0}/{nextLevelCfg["levelUpNeedItem"][1][1]}";
            Slider.Slider.value = 0;
        }
        else
        {
            TxtItemCount.Text.text = $"{bagItemInfo["num"]}/{nextLevelCfg["levelUpNeedItem"][1][1]}";
            Slider.Slider.value = bagItemInfo["num"].AsFloat / nextLevelCfg["levelUpNeedItem"][1][1].AsFloat;
        }

        AtkAttrItem.ShowImgUp(false);
        BloodAttrItem.ShowImgUp(false);
        PopuLationAttrItem.ShowImgUp(false);

        var nowAtk = nowCfg["atk"];
        var nowMaxHp = nowCfg["maxHp"];
        var nowCount = nowCfg["count"];

        AtkAttrItem.SetAttr2(GetAttr(nowAtk));
        BloodAttrItem.SetAttr2(GetAttr(nowMaxHp));
        PopuLationAttrItem.SetAttr2(nowCount);

        if (nextLevelCfg != null)
        {
            Slider.SetActive(true);
            BtnUpgrade.Find("TxtMax").SetActive(false);
            BtnUpgrade.Find("NoMax").SetActive(true);

            AtkAttrItem.ShowAttr1(true);
            BloodAttrItem.ShowAttr1(true);
            PopuLationAttrItem.ShowAttr1(true);

            var nextAtk = nextLevelCfg["atk"];
            var nextMaxHp = nextLevelCfg["maxHp"];
            var nextCount = nextLevelCfg["count"];

            AtkAttrItem.SetAttr1(GetAttr(nextAtk));
            BloodAttrItem.SetAttr1(GetAttr(nextMaxHp));
            PopuLationAttrItem.SetAttr1(nextCount);

            if (nowAtk < nextAtk)
            {
                AtkAttrItem.ShowImgUp(true);
            }

            if (nowMaxHp < nextMaxHp)
            {
                BloodAttrItem.ShowImgUp(true);
            }

            if (nowCount < nextCount)
            {
                PopuLationAttrItem.ShowImgUp(true);
            }
        }
        else
        {
            Slider.SetActive(false);
            AtkAttrItem.ShowAttr1(false);
            BloodAttrItem.ShowAttr1(false);
            PopuLationAttrItem.ShowAttr1(false);

            AtkAttrItem.ShowImgUp(false);
            BloodAttrItem.ShowImgUp(false);
            PopuLationAttrItem.ShowImgUp(false);


            BtnUpgrade.Find("TxtMax").SetActive(true);
            BtnUpgrade.Find("NoMax").SetActive(false);
        }

        var needGold = nowCfg["levelUpNeedItem"][0][1];
        var needItem = nowCfg["levelUpNeedItem"][1][1];
        string itemCfgid = nowCfg["levelUpNeedItem"][1][0];
        
        TextCost.Text.text = nowCfg["levelUpNeedItem"][0][1];

        var gold = GameData.instance.PermanentData["bag_info"]["100001"]["num"];
        var hasItem = GameData.instance.PermanentData["bag_info"][itemCfgid]["num"];

        if (gold == null)
            gold = 0;

        if (hasItem == null)
            hasItem = 0;

        Debug.Log($"gold:{gold}, needGold:{needGold}, needItem:{needItem}, hasItem:{hasItem}");
        var value = gold < needGold || hasItem < needItem;
        //BtnUpgrade.SetImgGray(gold < needGold || hasItem < needItem);

        BtnUpgrade.SetImgGray(value);
        canUpgrade = !value;
    }

    string GetAttr(int attr) { 
        return (attr / 1000).ToString();
    }

    public void OnBtnClose()
    {
        //EventDispatcher<TroopsMono>.instance.TriggerEvent(EventName.Scene_CloseTroopRange);
        Close();
    }

    public void OnBtnCloseLevelUp()
    {
        LayoutLevelUp.SetActive(false);
        LayoutArmy.SetActive(true);
        DoTweenAnimUtil.ScaleAni(LayoutArmy.transform, 0, 0.3f, DG.Tweening.Ease.OutQuad, Vector3.zero);
        CurrSelectedArmyItem = null;

        ResetScrollViewDataCount();
    }

    public void OnBtnUpgrade()
    {
        if (!canUpgrade)
            return;

        JSONObject obj = new JSONObject();
        JSONObject obj1 = new JSONObject();
        obj.Add("data", obj1);
        obj1.Add("id", soldierLevelUPId);
        Cmd.instance.C2S_UP_TROOP_LEVEL(obj);
    }

    protected override void OnUpdate()
    {

    }

    void OnRenderItem(GameObject obj, int index)
    {
        int childCount = obj.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            int idx = index * childCount + i;
            SoldierLevelArmyItem item = ItemBase.GetItem<SoldierLevelArmyItem>(obj.transform.GetChild(i).gameObject, _ItemList, this);
            item.OnBeginDragPutSoldier = OnBeginDragPutSoldier;
            item.OnEndDragPutSoldier = OnEndDragPutSolider;
            item.SetActive(true);
            if (idx < dataCount)
            {
                var data = soldierDatas[idx];
                item.TroopInfo = data;
                item.Data.Cfg = data;
                item.Data.soldierCfg = soldierCfg;
                item.Data.Id = data["id"];
                item.Data.Idx = idx;
                item.Data.View = this;
                item.Data.DragItem = dragItem;
                item.isLayoutAdd = false;
                //item.Id = 

                //item.SetActive(true);
                item.LayoutAdd.SetActive(false);
                item.LayoutArmy.SetActive(true);
                item.SetData(this);
            }
            else if(idx == dataCount)
            {
                var maximum = soldierCfg["maximum"];

                if (idx >= maximum)
                {
                    item.LayoutAdd.SetActive(false);
                }
                else
                {
                    item.LayoutAdd.SetActive(true);
                }

                item.isLayoutAdd = true;
                //item.SetActive(false);
                item.LayoutArmy.SetActive(false);
                item.SetData(this);
            }
            else
            {
                item.SetActive(false);
            }
        }

        //if (CurrSelectedArmyItem != null)
        //{
        //    UpdateLevelUpAttr(CurrSelectedArmyItem.TroopInfo);
        //}
    }

    public void OnBeginDragPutSoldier(SoldierLevelArmyItemData data)
    {
        IsDragUI = true;

        if (data.Id != null && GameData.instance.PermanentData["troop_info"].HasKey(data.Id))
        {
            var troopInfoData = GameData.instance.PermanentData["troop_info"][data.Id];

            JSONNode soldierBattleData = TableUtils.FindSoldierBattleConfByCfgidLevel(troopInfoData["cfgId"], troopInfoData["level"]);
            int count = soldierBattleData["count"];
            for (int i = 0; i < count; i++)
            {
                var item = ClassPool.instance.Pop<DragSoldier>();
                item.Init(i, soldierBattleData);
                dragSoldierList.Add(item);
            }
        }

        if (UIDragSoliderTrans == null)
        {
            //UIDragSoliderTrans = ResMgr.instance.LoadGameObjectAsync("TroopMono").transform;
            //UIDragSoliderTrans.gameObject.SetActive(false);

            ResMgr.instance.LoadGameObjectAsync("TroopMono", (go) =>
            {
                UIDragSoliderTrans = go.transform;
                UIDragSoliderTrans.gameObject.SetActive(false);
            });
        }
    }

    public void OnEndDragPutSolider(int troopId)
    {
        if (UIDragSoliderTrans.gameObject.activeSelf)
        {
            //var nearestPos = GameUtils.FindNearestGridPoint(dragSoliderhitPos);
            var gridId = GameUtils.FindNearestGrid(dragSoliderhitPos);
            if (gridId != -1)
            {
                GuideNode guideNode = null;
                if (GuideMgr.guideNodeList.Count > 0) {
                    foreach (GuideNode item in GuideMgr.guideNodeList)
                    {
                        if (item.IsDragSoldier && item.stage == GuideNode.Stage.Guiding)
                        {
                            guideNode = item;
                        }
                    }
                }

                if (guideNode == null || guideNode.GridId == gridId)
                {
                    JSONObject obj = new JSONObject();
                    JSONObject obj1 = new JSONObject();
                    obj.Add("data", obj1);
                    obj1.Add("grid", gridId);
                    obj1.Add("id", troopId);
                    Cmd.instance.C2S_PUT_TROOP_ON(obj);

                    if (guideNode != null) {
                        UiMgr.Close<GuideView>();
                        guideNode.NextStep();
                    }
                }
            }
        }

        foreach (var item in dragSoldierList)
        {
            item.Release();
        }
        dragSoldierList.Clear();

        IsDragUI = false;
        ResMgr.instance.ReleaseGameObject(UIDragSoliderTrans.gameObject);
        UIDragSoliderTrans = null;
    }

    public void OnMouseOnUI(Ray ray)
    {
        if (UIDragSoliderTrans != null)
        {
            UIDragSoliderTrans.SetActiveEx(false);
        }
    }

    public void OnMouse(Ray ray)
    {
        if (IsDragUI)
        {
            if (Physics.Raycast(ray, out hit, rayLength, layerMask))
            {
                if (UIDragSoliderTrans != null)
                {
                    UIDragSoliderTrans.SetActiveEx(true);

                    dragSoliderhitPos = hit.point;

                    dragSoliderhitPos = GameUtils.CheckMapBorderX(FixVector3.ToFixVector3(dragSoliderhitPos).ToFixVector2()).ToVector3();

                    UIDragSoliderTrans.position = dragSoliderhitPos;

                    foreach (var item in dragSoldierList)
                    {
                        item.UpdatePos(dragSoliderhitPos);
                    }
                }
            }
        }
    }


    void OnClickGain()
    {
        UiMgr.ShowTips("comming soon");
    }

    void OnBtnAuto()
    {
        DebugUtils.Log("OnBtnAuto");
    }


    void OnClickReward1()
    {
        UiMgr.ShowTips("comming soon");

    }

    void OnClickReward2()
    {
        UiMgr.ShowTips("comming soon");

    }


    void OnClickReward3()
    {
        UiMgr.ShowTips("comming soon");

    }


    protected override void OnHide()
    {
        soldierDatas.Clear();
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.bag_info, UpdateBagInfo);
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.battle_troop, UpdateBattleTroop);
        CurrSelectedArmyItem = null;

        PhysicsRayMgr.instance.MouseOnUI -= OnMouseOnUI;
        PhysicsRayMgr.instance.Mouse -= OnMouse;
    }

    protected override void OnDestroy()
    {
        ArmyScrollView.LoopScrollView.Release();
    }

    //=============guide

    SoldierLevelArmyItem guidingItem;
    public override RectTransform GetGuideTrans(GuideNode guideNode)
    {
        if (guideNode.IsDragSoldier) {

            foreach (var item in _ItemList.Values)
            {
                SoldierLevelArmyItem soldierLevelArmyItem = (SoldierLevelArmyItem)item;
                if (soldierLevelArmyItem.CheckIsCanDrag())
                {
                    return soldierLevelArmyItem.transform.GetComponent<RectTransform>();
                }
            }

            return null;
        }
        if (guideNode.cfg["strArg2"] == "CreateSoldier" || guideNode.cfg["strArg2"] == "CreateSoldier2")
        {
            foreach (var item in _ItemList.Values)
            {
                SoldierLevelArmyItem soldierLevelArmyItem = (SoldierLevelArmyItem)item;
                if (soldierLevelArmyItem.isLayoutAdd)
                {
                    return soldierLevelArmyItem.transform.GetComponent<RectTransform>();
                }
            }
            return null;
        }
        else if (guideNode.cfg["strArg2"] == "Update") {
            foreach (var item in _ItemList.Values)
            {
                SoldierLevelArmyItem soldierLevelArmyItem = (SoldierLevelArmyItem)item;
                if (!soldierLevelArmyItem.isLayoutAdd)
                {
                    guidingItem = soldierLevelArmyItem;
                    return soldierLevelArmyItem.transform.GetComponent<RectTransform>();
                }
            }
            return null;
        }

        return base.GetGuideTrans(guideNode);
    }

    public override void TriggerGuide(GuideNode guideNode)
    {
        if (guideNode.cfg["strArg2"] == "CreateSoldier" || guideNode.cfg["strArg2"] == "CreateSoldier2")
        {
            foreach (var item in _ItemList.Values)
            {
                SoldierLevelArmyItem soldierLevelArmyItem = (SoldierLevelArmyItem)item;
                if (soldierLevelArmyItem.isLayoutAdd)
                {
                    soldierLevelArmyItem.OnLayoutAdd();
                    guideNode.NextStep();
                }
            }
            return;
        }
        else if (guideNode.cfg["strArg2"] == "Update")
        {
            if (guidingItem != null) {
                guidingItem.OnClickItem();
                guideNode.NextStep();
            }
            return;
        }

        base.TriggerGuide(guideNode);
    }

}
