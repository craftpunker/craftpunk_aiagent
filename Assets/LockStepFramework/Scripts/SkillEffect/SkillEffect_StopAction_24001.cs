

using Battle;
#if _CLIENTLOGIC_
using UnityEngine;
#endif


// * 1
public class SkillEffect_StopAction_24001 : SkillEffectBase
{
    public override void Start()
    {
        base.Start();
        BattleMgr.instance.AddStage(Target, StageConst.StopAction);
        BattleMgr.instance.ToIdleFsm(Target);
    }

    public override void Release()
    {
        BattleMgr.instance.RemoveStage(Target, StageConst.StopAction);
        BattleMgr.instance.ToSearchEnemyFsm(Target);
        ClassPool.instance.Push(this);
        base.Release();
    }
}