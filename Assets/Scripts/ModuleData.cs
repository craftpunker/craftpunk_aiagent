using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleData
{
    public static float DayEnd;
    public static float WeekEnd;
    public static float MonthEnd;

    public static void Init() {
        ShopData.Init();


        EventDispatcher<CmdEventData>.instance.AddEvent("less_day", OnLess_day);
        EventDispatcher<CmdEventData>.instance.AddEvent("less_week", OnLess_week);
        EventDispatcher<CmdEventData>.instance.AddEvent("less_month", OnLess_month);
    }


    static void OnLess_day(string evtName, CmdEventData[] data) {
        DayEnd = GameData.instance.PermanentData["less_day"] + Time.time;
    }

    static void OnLess_week(string evtName, CmdEventData[] data)
    {
        WeekEnd = GameData.instance.PermanentData["less_week"] + Time.time;
    }

    static void OnLess_month(string evtName, CmdEventData[] data)
    {
        MonthEnd = GameData.instance.PermanentData["less_month"] + Time.time;
    }

    public static bool CheckResEnought(string resType, int count) { 
        int res = GameData.instance.PermanentData["bag_info"][resType]["num"];
        return res >= count;
    }
}
