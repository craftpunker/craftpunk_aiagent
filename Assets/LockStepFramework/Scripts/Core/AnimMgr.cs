
#if _CLIENTLOGIC_
using Battle;
using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum AnimType
{
    Idle = 1,
    Move = 2,
    Atk = 3,
}

public class AminInfo
{
    public float CurrTime;
    public bool Loop;
    public float StartTime;
    public float EndTime;
    public float Speed;
    public float LifeTime;
    public Action PlayCompleteCallBack;

    public void Init(float startTime, float endTime, float speed, bool loop, float lifeTime)
    {
        Loop = loop;
        StartTime = startTime;
        EndTime = endTime;
        Speed = speed;
        CurrTime = startTime;
        LifeTime = lifeTime;
        PlayCompleteCallBack = null;
    }

    public void Release()
    {
        PlayCompleteCallBack = null;
        ClassPool.instance.Push(this);
    }
}

public class AnimMgr : Singleton<AnimMgr>
{
    private Dictionary<int, AnimData> animDict = new Dictionary<int, AnimData>();

    public void Init()
    {
        //var columnRow = GameData.instance.TableJsonDict["GlobalConf"]["GPUAnim"]["anyValue"];
        //Debug.Log(columnRowStr);
        //var columnRow = JSONNode.Parse(columnRowStr);

        //，json，startTimeendTime
        JSONNode datas = GameData.instance.TableJsonDict["AnimConf"];

        foreach (var d in datas)
        {
            AnimData animData = new AnimData();

            animData.Cfgid = int.Parse(d.Value["cfgId"]);
            var frameRowCol = d.Value["frameRowCol"][0];
            var rowColCountdown = d.Value["rowColCountdown"][0];
            var idieNode = d.Value["idleAnims"];
            var idleV2 = GetStartEndTimeByFrameId(idieNode[0], idieNode[1], frameRowCol[1], rowColCountdown[1]);
            animData.IdleStartTime = idleV2.x;
            animData.IdleEndTime = idleV2.y;
            animData.IdleSpeed = d.Value["idleSpeed"].AsFloat / 1000;

            var moveNode = d.Value["moveAnims"];
            // +1 
            var moveV2 = GetStartEndTimeByFrameId(moveNode[0], moveNode[1], frameRowCol[1], rowColCountdown[1]);
            animData.MoveStartTime = moveV2.x;
            animData.MoveEndTime = moveV2.y;
            animData.MoveSpeed = d.Value["moveSpeed"].AsFloat / 1000;

            var atkNode = d.Value["atkAnims"];
            var atkV2 = GetStartEndTimeByFrameId(atkNode[0], atkNode[1], frameRowCol[1], rowColCountdown[1]);
            animData.AtkStartTime = atkV2.x;
            animData.AtkEndTime = atkV2.y;
            animData.AtkSpeed = d.Value["atkSpeed"].AsFloat / 1000;

            animData.Prefab = d.Value["prefab"];
            animData.PrefabSize = GameUtils.JsonPosToUnityV3(d.Value["prefabSize"]);

            //animData.Time = d.Value["time"].AsFloat / 1000;
            animData.IdleTime = d.Value["idleTime"].AsFloat / 1000;
            animData.MoveTime = d.Value["moveTime"].AsFloat / 1000;
            animData.AtkTime = d.Value["atkTime"].AsFloat / 1000;

            animData.ShadowOffset = d.Value["shadowOffset"].AsFloat / 1000;

            animDict.Add(animData.Cfgid, animData);
        }
    }

    public Vector2 GetStartEndTimeByFrameId(int startFrame, int endFrame, int column, float columnDown)
    {
        //endFrame += 1;

        var startRow = startFrame / column;
        var startCol = startFrame % column;

        var endRow = endFrame / column;
        var endCol = endFrame % column;

        var startColStep = startCol * columnDown;
        var endColStep = endCol * columnDown;

        var startTime = startRow + startColStep;
        var endTime = endRow + endColStep;

        return new Vector2(startTime, endTime);
    }

    public AminInfo GetAnimInfo(int animCfgid, AnimType animType)
    {
        var animInfo = ClassPool.instance.Pop<AminInfo>();

        AnimData animData = animDict[animCfgid];
        switch (animType)
        {
            //idle
            case AnimType.Idle:
                animInfo.Init(animData.IdleStartTime, animData.IdleEndTime, animData.IdleSpeed, animData.IdleTime < 0, animData.IdleTime);
                break;
            case AnimType.Move:
                animInfo.Init(animData.MoveStartTime, animData.MoveEndTime, animData.MoveSpeed, animData.MoveTime < 0, animData.MoveTime);
                break;
            case AnimType.Atk:
                animInfo.Init(animData.AtkStartTime, animData.AtkEndTime, animData.AtkSpeed, animData.AtkTime < 0, animData.AtkTime);
                break;
        }

        return animInfo;
    }

    public AnimData GetAnimData(int animCfgid)
    {
        return animDict[animCfgid];
    }
}
#endif