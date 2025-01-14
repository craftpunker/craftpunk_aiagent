using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyMissionRedPoint : RedPointBase
{

    protected override void InitData()
    {
        EventList = new string[] { "d_task", "d_atc", "da_reward" };
        ParentList = new Type[] { typeof(MissionRedPoint) };
    }

    protected override bool OnCheckRed()
    {

        JSONNode taskCfg = GameData.instance.TableJsonDict["TaskRewardConf"];

        int act = GameData.instance.DailyData["d_atc"];
        for (int i = 1; i <= 5; i++)
        {
            string key = (i).ToString();
            JSONNode subCfg = taskCfg[key];
            JSONNode subData = null;

            if (GameData.instance.DailyData["da_reward"] != null) {
                subData = GameData.instance.DailyData["da_reward"][key];
            }

            if (subData == null && act >= subCfg["needActivePoint"]) {
                return true;
            }
        }

        foreach (var d_task in GameData.instance.DailyData["d_task"].Values)
        {
            if (d_task["cur"] >= d_task["need"] && d_task["draw"] == 0) {
                return true;
            }
        }

        return false;
    }
}
