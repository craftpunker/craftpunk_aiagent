using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondGoodsItem : ShopGoodsItem
{

    public void SetData(string CfgId)
    {
        this.CfgId = CfgId;
        Refresh();
    }

    public JSONNode cfg;
    protected override void OnRefresh()
    {
        base.OnRefresh();

        cfg = GameData.instance.TableJsonDict["RechargeConf"][CfgId];
        
        ImgGoods.Image.rectTransform.localScale = new Vector2(1, 1);
        ImgGoods.SetSprite("ShopAtlas", "Ui_diamond_" + cfg["icon"]);


        if (cfg["benefitDiamonds"] > 0)
        {
            ImgBenefit.SetActive(true);
            TxtBenefit.Text.text = "+" + ((float)cfg["benefitDiamonds"]).ToString();
        }
        else {
            ImgBenefit.SetActive(false);
        }

        TxtCount.Text.text = cfg["rewardItem"][0][1];
        SetPriceIcon(false);
        TxtPrice.Text.text = "$" + ((float)cfg["price"] / 100).ToString();

        JSONNode itemCfg = GameData.instance.TableJsonDict["ItemConf"]["100002"];
        Bg.SetSprite(itemCfg["bgIconAtlas"], itemCfg["bgIcon"]);
    }

    public override void OnClickItem()
    {
        ShopConfirmView.ShopConfirmViewData data = new ShopConfirmView.ShopConfirmViewData();
        data.Title = "Buy Diamond";
        data.IconAtlas = "ShopAtlas";
        data.Icon = "Ui_diamond_" + cfg["icon"];
        data.Cost = TxtPrice.Text.text;
        //data.costAtlas = null;

        data.callback = () =>
        {
            ShopData.GetRecord(CfgId);
        };

        UiMgr.Open<ShopConfirmView>(null, data);
    }
}
