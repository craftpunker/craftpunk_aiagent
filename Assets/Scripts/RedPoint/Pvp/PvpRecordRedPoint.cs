using Battle;
using System.Collections;
using System.Collections.Generic;
using System;

public class PvpRecordRedPoint : RedPointBase
{
    protected override void InitData()
    {
        EventList = new string[] { EventName.record, EventName.recordAll };
        ParentList = new Type[] {typeof(PvpRedPoint) };
    }

    protected override bool OnCheckRed()
    {

        foreach (var item in GameData.instance.RecordsData.Values)
        {
            if (item["read"] == 0 && item["revenge"] == 0 && item["tp"] == "def")
            {
                return true;
            }
        }
        //return GameData.instance.RecordsData.Count > 0;
        return false;
    }
}
