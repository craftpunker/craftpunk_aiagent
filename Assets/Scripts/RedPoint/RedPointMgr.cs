using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RedPointMgr
{
    public static Dictionary<Type, RedPointBase> DictRedPoint => _DictRedPoint;

    static Dictionary<Type, RedPointBase> _DictRedPoint;

    public static void Init() {
        _DictRedPoint = new Dictionary<Type, RedPointBase>();

        Regist<MissionRedPoint>();
            Regist<DailyMissionRedPoint>();
            Regist<WeeklyMissionRedPoint>();
            Regist<AchieveMissionRedPoint>();

        Regist<MailRedPoint>();
        Regist<ShopRedPoint>();

        Regist<PvpRedPoint>();
            Regist<PvpRecordRedPoint>();

        Regist<PutSoldierRedPoint>();
        Regist<PowerChestRedPoint>();
    }

    static void Regist<T>() where T : RedPointBase, new() 
    {
        T redPoint = new T();
        redPoint.Init();
        _DictRedPoint.Add(typeof(T), redPoint);
    }

    public static RedPointBase GetRedPoint(Type type) {
        return _DictRedPoint[type];
    }

    public static RedPointBase GetRedPoint<T>() where T : RedPointBase
    {
        return _DictRedPoint[typeof(T)];
    }

    public static void ClearAllRedPoint() {
        foreach (var k in _DictRedPoint.Values) {
            k.ClearRedPoint();
        }
    }
}
