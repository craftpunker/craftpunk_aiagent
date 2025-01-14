using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : UiBase
{
    public U3dObj BtnClose;
    U3dObj ScrollView;
    U3dObj Content;

    //LayoutGift
    U3dObj LayoutGift;
    U3dObj GiftsItemView;

    List<string> GiftDataList = new List<string>(); 
    Dictionary<GameObject, ItemBase> DictGifts = new Dictionary<GameObject, ItemBase>(); 

    //LayoutDailySelection
    U3dObj LayoutDailySelection;

    U3dObj LayoutRefreshTime;
    U3dObj LayoutRefreshLess;
    U3dObj TxtRefresh;

    U3dObj LayoutRefresh;

    U3dObj DailySelectionItemView;
    
    List<string> DailySelectionDataList = new List<string>();
    Dictionary<GameObject, ItemBase> DictDailySelectionItems = new Dictionary<GameObject, ItemBase>();

    //LayoutTreasure
    U3dObj LayoutTreasure;

    U3dObj TreasureItemView;
    List<TreasureBoxData> TreasureDataList = new List<TreasureBoxData>();
    Dictionary<GameObject, ItemBase> TreasureItems = new Dictionary<GameObject, ItemBase>();


    //LayoutDiamond
    U3dObj LayoutDiamond;
    U3dObj DiamondItemView;
    Dictionary<GameObject, ItemBase> dictDiamondItems = new Dictionary<GameObject, ItemBase>();
    List<string> DiamondDataList = new List<string>();

    //LayoutGoldCoin
    U3dObj LayoutGoldCoin;

    U3dObj GoldCoinItemView;
    Dictionary<GameObject, ItemBase> GoldCoinItems = new Dictionary<GameObject, ItemBase>();
    List<JSONNode> goldCoinItemDataList = new List<JSONNode>();


    protected override void OnInit()
    {

        BtnClose = Find("Bg/BtnClose");
        SetOnClick(BtnClose, OnClickClose);

        ScrollView = Find("Bg/ScrollView");
        Content = ScrollView.Find("Viewport/Content");

        //LayoutGift
        LayoutGift = Content.Find("LayoutGift");
        GiftsItemView = LayoutGift.Find("GiftsItemView");
        GiftsItemView.ItemView.Init(DictGifts);
        GiftsItemView.ItemView.SetRenderHandler(OnRenderGift);

        //LayoutDailySelection
        LayoutDailySelection = Content.Find("LayoutDailySelection");

        LayoutRefreshTime = LayoutDailySelection.Find("LayoutTop/LayoutRefreshTime");
        LayoutRefreshLess = LayoutDailySelection.Find("LayoutTop/LayoutRefreshLess");
        TxtRefresh = LayoutRefreshLess.Find("TxtRefresh");

        LayoutRefresh = LayoutDailySelection.Find("LayoutTop/LayoutRefresh");
        SetOnClick(LayoutRefresh, OnClickRefreshDaily);

        DailySelectionItemView = LayoutDailySelection.Find("DailySelectionItemView");
        DailySelectionItemView.ItemView.Init(DictDailySelectionItems);
        DailySelectionItemView.ItemView.SetRenderHandler(OnRenderDailySelectionItem);

        //LayoutTreasure
        LayoutTreasure = Content.Find("LayoutTreasure");
        TreasureItemView = LayoutTreasure.Find("TreasureItemView");
        TreasureItemView.ItemView.Init(TreasureItems);
        TreasureItemView.ItemView.SetRenderHandler(OnRenderTreasureItems);

        //LayoutDiamond
        LayoutDiamond = Content.Find("LayoutDiamond");
        DiamondItemView = LayoutDiamond.Find("DiamondItemView");

        DiamondItemView.ItemView.Init(dictDiamondItems);
        DiamondItemView.ItemView.SetRenderHandler(OnRenderDiamond);

        //LayoutGoldCoin
        LayoutGoldCoin = Content.Find("LayoutGoldCoin");
        GoldCoinItemView = LayoutGoldCoin.Find("GoldCoinItemView");
        GoldCoinItemView.ItemView.Init(GoldCoinItems);
        GoldCoinItemView.ItemView.SetRenderHandler(OnRenderGoldCoin);
    }

    public void OnClickClose() { 
        Close();
    
    }

    //args = [callback, jump2]
    protected override void OnShow()
    {
        EventDispatcher<CmdEventData>.instance.AddEvent("rhg_record", OnRhg_record);
        EventDispatcher<CmdEventData>.instance.AddEvent("shop_cards", OnShop_cards);
        EventDispatcher<CmdEventData>.instance.AddEvent("shop_boxc", OnShop_boxc);

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.recharge_finish, OnRechargeFinish);
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.shop_buy_coin, OnButCoin);

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.bag_info, OnChangeBaginfo);

        RefreshGift();
        RefreshDailySelection();
        RefreshTreasure();
        RefreshDiamond();
        RefreshGoldCoin();
        LayoutRebuilder.ForceRebuildLayoutImmediate(Content.RectTransform);

        if (Args.Length >= 1)
        {
            int jump2 = (int)Args[0];
            U3dObj target = null;

            if (jump2 == 4)
            {
                target = LayoutDiamond;
            }
            else if (jump2 == 5)
            {
                target = LayoutGoldCoin;
            }

            if (target != null)
            {
                Content.RectTransform.anchoredPosition = new Vector2(Content.RectTransform.anchoredPosition.x, -target.RectTransform.anchoredPosition.y);
            }
        }
        else {
            Content.RectTransform.anchoredPosition = new Vector2(Content.RectTransform.anchoredPosition.x, 0);
        }
    }

    private void OnChangeBaginfo(string evtName, CmdEventData[] args)
    {

        foreach (var item in DictDailySelectionItems.Values) {
            DailyGoodsItem dailyGoodsItem = (DailyGoodsItem)item;
            dailyGoodsItem.OnResChange();
        }

        foreach (var item in TreasureItems.Values)
        {
            TreasureGoodsItem treasureGoodsItem = (TreasureGoodsItem)item;
            treasureGoodsItem.OnResChange();
        }

        foreach (var item in GoldCoinItems.Values)
        {
            GoldCoinGoodsItem goldCoinGoodsItem = (GoldCoinGoodsItem)item;
            goldCoinGoodsItem.OnResChange();
        }
    }

    void OnRechargeFinish(string evtName, CmdEventData[] evt)
    {
        JSONNode data = evt[0].JsonNode;
        foreach (ItemBase item in dictDiamondItems.Values) {

            DiamondGoodsItem diamondGoodsItem = (DiamondGoodsItem)item;
            if (diamondGoodsItem.CfgId == data["cfgId"].ToString())
            {
                ResAnim.instance.ShowUIAnim(data["rewardItems"], diamondGoodsItem.ImgGoods.transform.position);
                return;
            }
        }

        foreach (ItemBase item in DictGifts.Values)
        {

            ShopGiftItem shopGiftItem = (ShopGiftItem)item;
            if (shopGiftItem.cfgId == data["cfgId"].ToString())
            {
                ResAnim.instance.ShowUIAnim(data["rewardItems"], shopGiftItem.ImgGoods.transform.position);
                return;
            }
        }
    }

    void OnButCoin(string evtName, CmdEventData[] evt) {
        JSONNode data = evt[0].JsonNode;

        foreach (ItemBase item in GoldCoinItems.Values)
        {
            GoldCoinGoodsItem CoinItem = (GoldCoinGoodsItem)item;
            if (CoinItem.cfg["cfgId"] == data["cfgId"])
            {
                ResAnim.instance.ShowUIAnim(data["rewardItems"], CoinItem.ImgGoods.transform.position);
                return;
            }
        }
    }

    void OnRhg_record(string evtName, CmdEventData[] evt) {
        RefreshGift();
        RefreshDiamond();
        LayoutRebuilder.ForceRebuildLayoutImmediate(Content.RectTransform);
    }

    void OnShop_cards(string evtName, CmdEventData[] evt)
    {
        RefreshDailySelection();
        LayoutRebuilder.ForceRebuildLayoutImmediate(Content.RectTransform);
    }

    void OnShop_boxc(string evtName, CmdEventData[] evt)
    {
        RefreshTreasure();
        LayoutRebuilder.ForceRebuildLayoutImmediate(Content.RectTransform);
    }

    //LayoutGift==============================================
    void RefreshGift() {
        JSONNode data = GameData.instance.PermanentData["rhg_record"];

        GiftDataList.Clear();

        JSONNode cfg = GameData.instance.TableJsonDict["RechargeConf"];
        foreach (var item in data) {
            string cfgId = item.Key;
            JSONNode subCfg = cfg[cfgId];

            if (!ShopData.DictLessTimeRecord.TryGetValue(cfgId, out float lessEnd)) {
                lessEnd = 0;
            }

            if (subCfg != null && (item.Value["less"] == -1 || lessEnd > Time.time)) {
                if (subCfg["type"] == 2 && item.Value["cnt"] < subCfg["buyCount"]) {
                    GiftDataList.Add(cfgId);
                }
            }
        }

        if (GiftDataList.Count == 0) {
            LayoutGift.SetActive(false);
            return;
        }

        LayoutGift.SetActive(true);

        GiftDataList.Sort((a, b) => {
            int idA = Convert.ToInt32(a);
            int idB = Convert.ToInt32(b);

            if (idA == idB)
            {
                return 0;
            }

            return idA < idB ? -1 : 1;
        });

        GiftsItemView.ItemView.SetDataCount(GiftDataList.Count);
        RectTransform giftItem = GiftsItemView.ItemView.Item.transform.GetComponent<RectTransform>();
        float itemLenth = giftItem.rect.height * GiftDataList.Count;

        LayoutGift.RectTransform.SetRectSizeY(61 + itemLenth + 10);
    }

    void OnRenderGift(GameObject go, int index)
    {
        ShopGiftItem item = ItemBase.GetItem<ShopGiftItem>(go, DictGifts);
        item.SetData(GiftDataList[index]);
    }

    //LayoutDailySelection==============================================

    void RefreshDailySelection() { 

        int shopRef = GameData.instance.DailyData["shop_refc"];
        int maxRef = GameData.instance.TableJsonDict["GlobalConf"]["shopRef"]["intValue"];
        TxtRefresh.Text.text = (maxRef - shopRef).ToString();

        JSONNode shopCostCfg = GameData.instance.TableJsonDict["GlobalConf"]["shopCost"]["anyValue"];

        int cost = 0;
        if (shopRef + 1 > shopCostCfg.Count)
        {
            cost = shopCostCfg[shopCostCfg.Count - 1][1];
        }
        else {
            cost = shopCostCfg[shopRef][1];
        }

        LayoutRefresh.Text.text = cost.ToString();

        JSONNode data = GameData.instance.DailyData["shop_cards"];
        DailySelectionDataList.Clear();

        foreach (var item in data) {
            DailySelectionDataList.Add(item.Key);
        }

        DailySelectionDataList.Sort((a, b) => { 
            int idA = Convert.ToInt32(a);
            int idB = Convert.ToInt32(b);

            if (idA == idB) {
                return 0;
            }
            return idA < idB ? -1 : 1;
        });

        int count = (int)Math.Ceiling((decimal)DailySelectionDataList.Count / DailySelectionItemView.ItemView.Item.childCount);
        DailySelectionItemView.ItemView.SetDataCount(count);

        float ItemLenth = DailySelectionItemView.ItemView.Item.GetComponent<RectTransform>().rect.height * count;
        LayoutDailySelection.RectTransform.SetRectSizeY(ItemLenth + 61 + 63 + 20);

    }

    void OnRenderDailySelectionItem(GameObject go, int index)
    {

        for (int i = 0; i < go.transform.childCount; i++)
        {
            int idx = go.transform.childCount * index + i;

            Transform child = go.transform.GetChild(i);
            DailyGoodsItem item = ItemBase.GetItem<DailyGoodsItem>(child, DictDailySelectionItems);


            if (idx + 1 <= DailySelectionDataList.Count)
            {
                item.SetData(DailySelectionDataList[idx]);
            }
            else
            {
                item.SetActive(false);
            }
        }
    }


    void OnClickRefreshDaily()
    {
        ShopData.C2S_SHOP_REFRESH_CARD();
        //Debug.Log("rrrrrrrrrrrrr");
        //GuideView guideView = (GuideView)UiMgr.Open<GuideView>();
        //guideView.SetTarget(LayoutRefresh.RectTransform);

    }
    // Treasure===============================================

    public struct TreasureBoxData {
        //public int index;
        public JSONNode cfg;
        //public int count;
    }
    
    void RefreshTreasure()
    {
        JSONNode boxCfg = GameData.instance.TableJsonDict["BoxConf"];

        TreasureDataList.Clear();

        foreach (var item in boxCfg) {
            TreasureBoxData treasureBoxData = new TreasureBoxData();
            treasureBoxData.cfg = item.Value;
            TreasureDataList.Add(treasureBoxData);
        }

        TreasureDataList.Sort((a, b) => {
            int countA = a.cfg["count"];
            int countB = b.cfg["count"];

            if (countA != countB)
            {
                return countA < countB ? -1 : 1;
            }

            int cfgIdA = a.cfg["cfgId"];
            int cfgIdB = b.cfg["cfgId"];

            return cfgIdA < cfgIdB ? -1 : 1;
        });

        //for (int i = 0; i < 4; i++)
        //{
        //    TreasureBoxData treasureBoxData = new TreasureBoxData();
        //    treasureBoxData.index = i + 1;

        //    if (i + 1 <= 2)
        //    {
        //        treasureBoxData.cfg = boxCfg[i];
        //        treasureBoxData.count = 1;
        //    }
        //    else {
        //        treasureBoxData.cfg = boxCfg[i - 2];
        //        treasureBoxData.count = 10;
        //    }

        //    TreasureDataList.Add(treasureBoxData);
        //}


        int count = (int)Math.Ceiling((decimal)TreasureDataList.Count / TreasureItemView.ItemView.Item.childCount);
        TreasureItemView.ItemView.SetDataCount(count);

        float ItemLenth = TreasureItemView.ItemView.Item.GetComponent<RectTransform>().rect.height * count;
        LayoutTreasure.RectTransform.SetRectSizeY(ItemLenth + 61 + 20);
    }


    void OnRenderTreasureItems(GameObject go, int index) {

        for (int i = 0; i < go.transform.childCount; i++)
        {
            int idx = go.transform.childCount * index + i;

            Transform child = go.transform.GetChild(i);
            TreasureGoodsItem item = ItemBase.GetItem<TreasureGoodsItem>(child, TreasureItems);

            if (idx + 1 <= TreasureDataList.Count)
            {
                item.SetData(TreasureDataList[idx]);
            }
            else {
                item.SetActive(false);
            }
        }
    }

    //Diamond==========================================


    void RefreshDiamond() {
        JSONNode data = GameData.instance.PermanentData["rhg_record"];
        JSONNode cfg = GameData.instance.TableJsonDict["RechargeConf"];

        DiamondDataList.Clear();

        foreach (var item in data)
        {
            string cfgId = item.Key;
            JSONNode subCfg = cfg[cfgId];

            if (!ShopData.DictLessTimeRecord.TryGetValue(cfgId, out float lessEnd))
            {
                lessEnd = 0;
            }

            if (subCfg != null && (item.Value["less"] == -1 || lessEnd > Time.time))
            {
                if (subCfg["type"] == 1)
                {
                    DiamondDataList.Add(cfgId);
                }
            }
        }

        DiamondDataList.Sort((a, b) => { 
            int idA = Convert.ToInt32(a);
            int idB = Convert.ToInt32(b);

            if (idA == idB) {
                return 0;
            }

            return idA < idB ? -1 : 1;
        });

        int count = (int)Math.Ceiling((decimal)DiamondDataList.Count / DiamondItemView.ItemView.Item.childCount);
        DiamondItemView.ItemView.SetDataCount(count);

        float ItemLenth = DiamondItemView.ItemView.Item.GetComponent<RectTransform>().rect.height * count;

        LayoutDiamond.RectTransform.SetRectSizeY(ItemLenth + 61 + 10 + 10);
    }

    void OnRenderDiamond(GameObject go, int index ) {

        for (int i = 0; i < go.transform.childCount; i++)
        {
            int idx = go.transform.childCount * index + i;

            Transform child = go.transform.GetChild(i);
            DiamondGoodsItem item = ItemBase.GetItem<DiamondGoodsItem>(child, dictDiamondItems);

            if (idx + 1 <= DiamondDataList.Count)
            {
                item.SetData(DiamondDataList[idx]);
            }
            else
            {
                item.SetActive(false);
            }
        }
    }

    //GoldCoin======================================

    void RefreshGoldCoin() {

        JSONNode shopCfg = GameData.instance.TableJsonDict["ShopConf"];

        goldCoinItemDataList.Clear();

        foreach (var item in shopCfg.Values)
        {
            if (item["type"] == 2) {
                goldCoinItemDataList.Add(item);
            }
        }

        int count = (int)Math.Ceiling((decimal)goldCoinItemDataList.Count / GoldCoinItemView.ItemView.Item.childCount);
        GoldCoinItemView.ItemView.SetDataCount(count);

        float ItemLenth = GoldCoinItemView.ItemView.Item.GetComponent<RectTransform>().rect.height * count;
        LayoutGoldCoin.RectTransform.SetRectSizeY(ItemLenth + 61 + 20);
    }

    void OnRenderGoldCoin(GameObject go, int index)
    {

        for (int i = 0; i < go.transform.childCount; i++)
        {
            int idx = go.transform.childCount * index + i;

            Transform child = go.transform.GetChild(i);
            GoldCoinGoodsItem item = ItemBase.GetItem<GoldCoinGoodsItem>(child, GoldCoinItems);

            if (idx + 1 <= goldCoinItemDataList.Count)
            {
                item.SetData(goldCoinItemDataList[idx]);
            }
            else
            {
                item.SetActive(false);
            }
        }
    }

    //===========================================

    protected override void OnUpdate()
    {
        foreach (var item in DictGifts.Values)
        {
            ShopGiftItem shopGiftItem = item as ShopGiftItem;
            shopGiftItem.Update();
        }

        float lessRefreshTime = ModuleData.DayEnd - Time.time;

        TimeUtils.TimeHMS t = TimeUtils.DHMS(lessRefreshTime);
        LayoutRefreshTime.Text.text = $"{t.Hour}:{t.Minute}:{t.Second}";
    }

    protected override void OnHide()
    {
        EventDispatcher<CmdEventData>.instance.RemoveEvent("rhg_record", OnRhg_record);
        EventDispatcher<CmdEventData>.instance.RemoveEvent("shop_cards", OnShop_cards);
        EventDispatcher<CmdEventData>.instance.RemoveEvent("shop_boxc", OnShop_boxc);
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.recharge_finish, OnRechargeFinish);
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.bag_info, OnChangeBaginfo);
    }

    protected override void OnDestroy()
    {
        GiftsItemView.ItemView.Release();
        DailySelectionItemView.ItemView.Release();
        DiamondItemView.ItemView.Release();
        GoldCoinItemView.ItemView.Release();
    }

    // guide================================
    public override RectTransform GetGuideTrans(GuideNode guideNode)
    {
        if (guideNode.cfg["strArg2"] == "box") {

            foreach (var item in TreasureItems.Values)
            {
                TreasureGoodsItem treasureGoodsItem = (TreasureGoodsItem)item;
                TreasureBoxData data = treasureGoodsItem.data;
                if (data.cfg["cfgId"].ToString() == guideNode.cfg["strArg3"] && treasureGoodsItem.CheckIsCanGuide()) {
                    Content.RectTransform.anchoredPosition = new Vector2(Content.RectTransform.anchoredPosition.x, -LayoutTreasure.RectTransform.anchoredPosition.y);
                    return treasureGoodsItem.transform.GetComponent<RectTransform>();
                }
            }
            return null;
        }

        return base.GetGuideTrans(guideNode);
    }

    public override void TriggerGuide(GuideNode guideNode)
    {
        if (guideNode.cfg["strArg2"] == "box")
        {

            foreach (var item in TreasureItems.Values)
            {
                TreasureGoodsItem treasureGoodsItem = (TreasureGoodsItem)item;
                TreasureBoxData data = treasureGoodsItem.data;
                if (data.cfg["cfgId"].ToString() == guideNode.cfg["strArg3"])
                {
                    treasureGoodsItem.OnClickItem();
                    guideNode.NextStep();
                    return;
                }
            }
            return;
        }

        base.TriggerGuide(guideNode);
    }
}
