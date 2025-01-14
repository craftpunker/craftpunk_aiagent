using System.Collections;
using System.Collections.Generic;
using System;
using Battle;
using SimpleJSON;

public class PutSoldierRedPoint : RedPointBase
{
    protected override void InitData()
    {
        EventList = new string[] {"bag_info", "troop_info" };
        ParentList = new Type[0];
    }

    protected override bool OnCheckRed()
    {
        JSONNode soldierCfg = GameData.instance.TableJsonDict["SoldierConf"];
        JSONNode bag_info = GameData.instance.PermanentData["bag_info"];

        foreach (var kv in soldierCfg)
        {
            var cfg = kv.Value;

            if (cfg["enabled"] == 1)
            {
                int cfgid = cfg["cfgId"];
                var troop_infos = GameData.instance.PermanentData["troop_info"];
                int itemCfgid = cfg["cfgId"] + 200000; //碎片ID
                JSONNode itemData = bag_info[itemCfgid.ToString()];

                int value = 0; //1 兵种已全部上阵；0 没有该兵卡
                foreach (var info in troop_infos)
                {
                    var troop_info_cfgid = info.Value["cfgId"];

                    if (troop_info_cfgid == cfgid)
                    {
                        value = 1;
                        break;
                    }
                }
                if (value == 0 && itemData != null && itemData["num"] != 0) //有碎片 
                {
                    return true;
                }
            }
        }

        return false;
    }
}
