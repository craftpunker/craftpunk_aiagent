using Battle;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class SkillEffectBase
    {
        public SkillBase SkillBase;
        public SkillEffectData SkillEffectData;
#if _CLIENTLOGIC_
    public AnimData AnimData;
#endif

        public EntityBase Origin;
        public EntityBase Target;

        public EntityAttrValue OriginAttrValue;

        public Fix64 NextTriggerTime; //，update

        public bool BKilled;

        public object[] Args;

        public virtual void Init()
        {
            BKilled = false;
            NextTriggerTime = Fix64.Zero;
        }

        public virtual void Start()
        {
            NextTriggerTime = GameData.instance._CumTime + SkillEffectData.frequency;

            if (SkillEffectData.skillBuffCfgId != 0)
            {
                Target.BuffBag.PushBuff(SkillEffectData.skillBuffCfgId);
            }

            if (SkillEffectData.skillCfgId != 0)
            {
                SkillFactory.instance.CreateSkill(SkillEffectData.skillCfgId, Origin, Target, Origin.GetAtkSkillShowPos(), Origin.PlayerGroup);
            }

            if (SkillEffectData.skillEffectCfgId != 0)
            {
                SkillEffectFactory.instance.CreateSkillEffect(SkillEffectData.skillEffectCfgId, SkillBase, Origin, Target);
            }
        }

        public virtual void Update()
        {
            if (SkillEffectData.frequency > 0)
                NextTriggerTime += SkillEffectData.frequency;
        }

        public virtual void Release()
        {
            Origin = null;
            Target = null;
            SkillEffectData = null;
            SkillBase = null;
#if _CLIENTLOGIC_
        AnimData = null;
#endif
        }
    }
}