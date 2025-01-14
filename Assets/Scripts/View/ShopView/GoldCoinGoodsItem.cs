using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoinGoodsItem : ShopGoodsItem
{

    public JSONNode cfg;
    JSONNode priceCfg;

    public void SetData(JSONNode cfg) { 
        this.cfg = cfg;
        Refresh();
    
    }
    protected override void OnRefresh()
    {
        ImgBenefit.SetActive(false);

        ImgGoods.SetSprite("ShopAtlas", "gold_" + cfg["cfgId"].ToString(), true, () => {
            ImgGoods.RectTransform.localScale = new Vector3(1, 1, 1);
            ImgGoods.SetSpriteRectBaseW(134);
        });

        priceCfg = cfg["price"][0];
        //JSONNode itemCfg = GameData.instance.TableJsonDict["ItemConf"][priceCfg[0].ToString()];
        SetPriceIcon(true);
        TxtPrice.Text.text = priceCfg[1].ToString();

        //ImgPrice.SetSprite(itemCfg["iconAtlas"], itemCfg["icon"]);

        ImgPrice.SetSprite("CommonAtlas", "Ui_frame_Diamonds");

        TxtCount.Text.text = cfg["rewardItem"][0][1];

        JSONNode itemCfg = GameData.instance.TableJsonDict["ItemConf"]["100001"];
        Bg.SetSprite(itemCfg["bgIconAtlas"], itemCfg["bgIcon"]);

        OnResChange();
    }

    public override void OnResChange()
    {
        SetPriceColorRed(!ModuleData.CheckResEnought(priceCfg[0], priceCfg[1]));
    }

    public override void OnClickItem()
    {
        if (!ModuleData.CheckResEnought(priceCfg[0], priceCfg[1])) {
            return;
        }

        ShopConfirmView.ShopConfirmViewData data = new ShopConfirmView.ShopConfirmViewData();
        data.Title = "Buy Coin";
        data.IconAtlas = "ShopAtlas";
        data.Icon = "gold_" + cfg["cfgId"].ToString();

        data.CostType = cfg["price"][0][0];
        data.Cost = TxtPrice.Text.text;

        data.CostAtlas = "CommonAtlas";
        data.CostIcon = "Ui_frame_Diamonds";
        //data.costAtlas = null;

        data.callback = () =>
        {
            ShopData.C2S_SHOP_BUY_GOLD(cfg["cfgId"]);
        };

        UiMgr.Open<ShopConfirmView>(null, data);
    }

    protected override void OnRelease()
    {

    }
}
