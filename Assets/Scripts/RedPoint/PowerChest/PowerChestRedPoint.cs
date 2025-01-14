using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System;


public class PowerChestRedPoint : RedPointBase
{
    protected override void InitData()
    {
        EventList = new string[] { "power_rwd", EventName.power_reward, "power" };
        ParentList = new Type[0];
    }

    protected override bool OnCheckRed()
    {
        JSONNode PowerChestConf = GameData.instance.TableJsonDict["PowerChestConf"];
        JSONNode powerRwdData = GameData.instance.PermanentData["power_rwd"];

        foreach (JSONNode cfg in PowerChestConf.Values)
        {
            JSONNode subPowerRwdData = powerRwdData[cfg["cfgId"].ToString()];
            float curPower = GameData.instance.PermanentData["power"];

            if (subPowerRwdData == null && curPower >= cfg["needPower"])
            {
                return true;
            }
        }
        return false;
    }
}
