
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;

public class RedPointBase
{
    protected string[] EventList;
    protected Type[] ParentList;

    Dictionary<RedPointBase, bool> DictChildRedPoints;

    public bool IsRed => _isRed;
    bool _isRed;

    RedPointBase[] eventDataList;
    public void Init() {
        InitData();

        foreach (var evtName in EventList) {
            EventDispatcher<CmdEventData>.instance.AddEvent(evtName, OnCmdEvent);
        }

        DictChildRedPoints = new Dictionary<RedPointBase, bool>();
        _isRed = false;

        eventDataList = new RedPointBase[] {this};
    }

    public void ClearRedPoint() {
        _isRed = false;
        DictChildRedPoints.Clear();
    }

    // override =====================================

    protected virtual void InitData() {
        EventList = new string[0];
        ParentList = new Type[0];
    }

    protected virtual bool OnCheckRed()
    {
        return false;
    }

    //============================================

    public void CheckRed() {
        SetIsRed(OnCheckRed(), this);
    }

    void OnCmdEvent(string evtName, CmdEventData[] evt)
    {
        CheckRed();
    }

    void SetIsRed(bool isRed, RedPointBase redPoint) {
        if (!DictChildRedPoints.TryAdd(redPoint, isRed)) {
            if (DictChildRedPoints[redPoint] == isRed) {
                return;
            }
            DictChildRedPoints[redPoint] = isRed;
        }

        isRed = false;
        foreach (bool isChildRed in DictChildRedPoints.Values)
        {
            if (isChildRed) {
                isRed = true;
                break;
            }
        }

        if (_isRed != isRed) {
            _isRed = isRed;
            EventDispatcher<RedPointBase>.instance.TriggerEvent(EventName.Red_Point, eventDataList);
            foreach (var type in ParentList)
            {
                RedPointBase parent = RedPointMgr.GetRedPoint(type);
                parent.SetIsRed(IsRed, this);
            }
        }
    }
}
