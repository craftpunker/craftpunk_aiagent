

using Battle;
#if _CLIENTLOGIC_
using UnityEngine;
#endif


// * 1
public class SkillEffect_ReduceAtk_20002 : SkillEffectBase
{
    private Fix64 value1;
    public override void Start()
    {
        base.Start();
        value1 = SkillEffectData.fix64Args[0];
        BattleMgr.instance.ChangeAtk(Target, -Target.Atk * value1);
    }

    public override void Release()
    {
        BattleMgr.instance.ChangeAtk(Target, Target.Atk * value1);
        ClassPool.instance.Push(this);
        base.Release();
    }
}