using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailRedPoint : RedPointBase
{
    protected override void InitData()
    {
        EventList = new string[] { EventName.mail, EventName .mailData};
        ParentList = new Type[0];
    }
    
    protected override bool OnCheckRed()
    {
        foreach (var data in GameData.instance.MailsData.Values) {
            if (data["read"] == 0) {
                return true;
            }
        }
        return false;
    }
}
