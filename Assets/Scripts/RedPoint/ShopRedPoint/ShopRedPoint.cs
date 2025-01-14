using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;

public class ShopRedPoint : RedPointBase
{
    protected override void InitData()
    {
        EventList = new string[] { "shop_cards", "shop_boxc" };
        ParentList = new Type[0];
    }

    protected override bool OnCheckRed()
    {
        JSONNode cfg = GameData.instance.TableJsonDict["ShopConf"];

        foreach (var item in GameData.instance.DailyData["shop_cards"].Values)
        {
            if (item["cnt"] != 0) {
                continue;
            }

            JSONNode subCfg = cfg[item["cfgId"].ToString()];
            JSONNode price = subCfg["price"];
            if (price == null || price.Count == 0 || price[0][1] == 0)
            {
                return true;
            }
        }

        JSONNode boxCfg = GameData.instance.TableJsonDict["BoxConf"];
        JSONNode shop_boxc = GameData.instance.DailyData["shop_boxc"];

        foreach (var subCfg in boxCfg.Values)
        {
            int boughtCount = ShopData.GetTreasureBoughtCount(subCfg["cfgId"].ToString());
            if (subCfg["dailyFree"] > boughtCount) {
                return true;
            }
        }

        return false;
    }
}
