

//,，
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class SkillEffect_Stealth_10004 : SkillEffectBase
    {
        public override void Start()
        {
            base.Start();
            BattleMgr.instance.AddStage(Target, StageConst.Stealth);
#if _CLIENTLOGIC_
        Target.SetAlpha(0.6f);
#endif
        }

        public override void Release()
        {
            if (Target != null)
                BattleMgr.instance.RemoveStage(Target, StageConst.Stealth);

#if _CLIENTLOGIC_
        Target.SetAlpha(1);
#endif
            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}