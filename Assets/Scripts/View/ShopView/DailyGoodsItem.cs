using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyGoodsItem : ShopGoodsItem
{
    string Index;

    JSONNode data;
    JSONNode cfg;

    public void SetData(string index) {
        Index = index;

        data = GameData.instance.DailyData["shop_cards"][Index];
        cfg = GameData.instance.TableJsonDict["ShopConf"][data["cfgId"].ToString()];

        Refresh();
    }

    JSONNode rewardCfg;
    JSONNode costCfg;

    protected override void OnRefresh()
    {
        ImgBenefit.SetActive(false);

        costCfg = null;
        if (cfg["price"] != null && cfg["price"].Count > 0) {
            costCfg = GameData.instance.TableJsonDict["ItemConf"][cfg["price"][0][0].ToString()];
        }

        if (costCfg == null || cfg["price"][0][1] == 0)
        {
            SetPriceColorRed(false);
            SetPriceIcon(false);
            TxtPrice.Text.text = "free";

            Bg.SetRedPoint(true);
        }
        else {
            SetPriceIcon(true);
            ImgPrice.SetSprite(costCfg["iconAtlas"], costCfg["icon"]);
            TxtPrice.Text.text = cfg["price"][0][1];

            SetPriceColorRed(!ModuleData.CheckResEnought(cfg["price"][0][0], cfg["price"][0][1]));

            Bg.SetRedPoint(false);
        }

        rewardCfg = GameData.instance.TableJsonDict["ItemConf"][cfg["rewardItem"][0][0].ToString()];

        ImgGoods.SetSprite(rewardCfg["iconAtlas"], rewardCfg["icon"], true);
        ImgGoods.Image.rectTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        TxtCount.Text.text = cfg["rewardItem"][0][1];

        Bg.SetImgGray(data["cnt"] != 0);
        Bg.SetRedPoint(data["cnt"] == 0 && (costCfg == null || cfg["price"][0][1] == 0));
        Bg.SetSprite(rewardCfg["bgIconAtlas"], rewardCfg["bgIcon"]);
    }

    public override void OnResChange()
    {
        if (costCfg == null || cfg["price"][0][1] == 0)
        {
            SetPriceColorRed(false);
        }
        else
        {
            SetPriceColorRed(!ModuleData.CheckResEnought(cfg["price"][0][0], cfg["price"][0][1]));
        }
    }

    public override void OnClickItem()
    {
        if (this.data["cnt"] != 0) {
            return;
        }

        if (!ModuleData.CheckResEnought(cfg["price"][0][0], cfg["price"][0][1])) {
            return;
        
        }

        ShopConfirmView.ShopConfirmViewData data = new ShopConfirmView.ShopConfirmViewData();
        data.Title = "Buy Goods";
        data.IconAtlas = rewardCfg["iconAtlas"];
        data.Icon = rewardCfg["icon"];

        data.CostType = cfg["price"][0][0];
        data.Cost = TxtPrice.Text.text;

        if (costCfg == null || cfg["price"][0][1] == 0)
        {
            data.CostAtlas = string.Empty;
            data.CostIcon = string.Empty;
        }
        else
        {
            data.CostAtlas = costCfg["iconAtlas"];
            data.CostIcon = costCfg["icon"];
        }

        data.callback = () =>
        {
            ShopData.C2S_SHOP_BUY_CARD(Convert.ToInt16(Index));
        };

        UiMgr.Open<ShopConfirmView>(null, data);
    }

    protected override void OnRelease()
    {

    }
}
