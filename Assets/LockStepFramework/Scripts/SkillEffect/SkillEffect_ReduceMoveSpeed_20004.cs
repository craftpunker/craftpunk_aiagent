

using Battle;
#if _CLIENTLOGIC_
using UnityEngine;
#endif


// * 1
public class SkillEffect_ReduceMoveSpeed_20004 : SkillEffectBase
{
    private Fix64 value1;
    public override void Start()
    {
        base.Start();
        value1 = SkillEffectData.fix64Args[0];
        BattleMgr.instance.ChangeMoveSpeed(Target, -Target.MoveSpeed * value1);
    }

    public override void Release()
    {
        BattleMgr.instance.ChangeMoveSpeed(Target, Target.MoveSpeed * value1);
        ClassPool.instance.Push(this);
        base.Release();
    }
}