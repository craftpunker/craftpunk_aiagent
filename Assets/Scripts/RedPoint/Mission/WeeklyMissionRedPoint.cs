using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeeklyMissionRedPoint : RedPointBase
{
    protected override void InitData()
    {
        EventList = new string[] { "w_task", "w_atc", "wa_reward" };
        ParentList = new Type[] {typeof(MissionRedPoint)};
    }

    protected override bool OnCheckRed()
    {
        JSONNode taskCfg = GameData.instance.TableJsonDict["TaskRewardConf"];

        int act = GameData.instance.WeeklyData["w_atc"];

        for (int i = 6; i <= 10; i++)
        {
            string key = (i).ToString();
            JSONNode subCfg = taskCfg[key];
            JSONNode subData = null;

            if (GameData.instance.WeeklyData["wa_reward"] != null)
            {
                subData = GameData.instance.WeeklyData["wa_reward"][key];
            }

            if (subData == null && act >= subCfg["needActivePoint"])
            {
                return true;
            }
        }

        foreach (var task in GameData.instance.WeeklyData["w_task"].Values)
        {
            if (task["cur"] >= task["need"] && task["draw"] == 0)
            {
                return true;
            }
        }

        return false;
    }
}
