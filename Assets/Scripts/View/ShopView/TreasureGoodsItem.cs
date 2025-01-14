using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureGoodsItem : ShopGoodsItem
{
     public ShopView.TreasureBoxData data;

    public void SetData(ShopView.TreasureBoxData data) { 
        
        this.data = data;
        Refresh();
    }

    int LessCount = 100;
    JSONNode price;
    JSONNode costItemCfg;

    bool[] _SpriteLoadFinishList;
    protected override void OnRefresh()
    {
        _SpriteLoadFinishList = new bool[] { false };
        Bg.SetActive(true);
        ImgBenefit.SetActive(false);

        int boughtCount = ShopData.GetTreasureBoughtCount(data.cfg["cfgId"].ToString());

        //JSONNode shop_boxc = GameData.instance.DailyData["shop_boxc"];
        //if (shop_boxc != null && shop_boxc[data.cfg["cfgId"].ToString()] != null)
        //{
        //    boughtCount = shop_boxc[data.cfg["cfgId"].ToString()];
        //}

        ImgGoods.SetSprite("ShopAtlas", "ui_Treasure_" + data.cfg["animCfg"], false, () => {
            _SpriteLoadFinishList[0] = true;
        });

        TxtCount.Text.text = "x" + data.cfg["count"];
        costItemCfg = null;

        if (data.cfg["dailyFree"] > boughtCount)
        {
            price = null;
        }
        else
        {
            price = data.cfg["price"][0];
        }

        if (price == null || price[1] == 0)
        {
            SetPriceIcon(false);
            TxtPrice.Text.text = "free";
        }
        else {
            SetPriceIcon(true);
            JSONNode itemCfg = GameData.instance.TableJsonDict["ItemConf"][price[0].ToString()];

            ImgPrice.SetSprite(itemCfg["iconAtlas"], itemCfg["icon"]);
            TxtPrice.Text.text = price[1].ToString();
            costItemCfg = itemCfg;
        }

        Bg.SetRedPoint(price == null);
        OnResChange();
    }

    public bool CheckIsCanGuide()
    {
        foreach (bool isLoad in _SpriteLoadFinishList)
        {
            if (!isLoad)
            {
                return false;
            }
        }
        return true;
    }

    public override void OnResChange()
    {
        if (price == null || price[1] == 0)
        {
            SetPriceColorRed(false);
            return;
        }
        SetPriceColorRed(!ModuleData.CheckResEnought(price[0], price[1]));
    }

    public override void OnClickItem()
    {
        if (LessCount <= 0) {
            return;
        }

        if (price != null && !ModuleData.CheckResEnought(price[0], price[1])) {
            return;
        }

        //UiMgr.Open<ShopBoxOpenView>(null, this.data);


        ShopConfirmView.ShopConfirmViewData data = new ShopConfirmView.ShopConfirmViewData();
        data.Title = "Buy Treasure";
        data.IconAtlas = "ShopAtlas";
        data.Icon = "ui_Treasure_" + this.data.cfg["animCfg"];

        if (price != null)
        {
            data.CostType = price[0];
        }
        else {
            data.CostType = string.Empty;
        }

        
        data.Cost = TxtPrice.Text.text;

        if (costItemCfg != null) {
            data.CostAtlas = costItemCfg["iconAtlas"];
            data.CostIcon = costItemCfg["icon"];
        }

        
        //data.costAtlas = null;

        data.callback = () =>
        {
            UiMgr.Open<ShopBoxOpenView>(null, this.data);
            //ShopData.GetRecord(CfgId);
        };

        UiMgr.Open<ShopConfirmView>(null, data);
    }

    protected override void OnRelease()
    {

    }
}
