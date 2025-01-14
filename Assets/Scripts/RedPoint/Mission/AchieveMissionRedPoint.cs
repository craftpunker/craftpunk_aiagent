using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;


public class AchieveMissionRedPoint : RedPointBase
{
    protected override void InitData()
    {
        EventList = new string[] { "a_task", "point" };
        ParentList = new Type[] { typeof(MissionRedPoint) };
    }

    protected override bool OnCheckRed()
    {
        JSONNode data = GameData.instance.PermanentData["a_task"];

        //DebugUtils.Log("eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");

        foreach (var item in data.Values) { 
            //DebugUtils.Log(item);
            if (item["cur"] >= item["need"] && item["draw"] == 0) {
                return true;
            }
        }
        return false;
    }
}
