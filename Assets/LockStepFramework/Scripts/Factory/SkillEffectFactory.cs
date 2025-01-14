#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{

    public enum SkillEffectType
    {
        None = 0,
        SkillEffect_Atk1_10001 = 10001,
        SkillEffect_FlyAway_10002 = 10002,
        SkillEffect_MirrorConvey_10003 = 10003,
        SkillEffect_Stealth_10004 = 10004,
        SkillEffect_CreateBuff_10005 = 10005,
        SkillEffect_AtkMaxHp_10006 = 10006,
        SkillEffect_AtkValue_10007 = 10007,
        SkillEffect_LinkAtkUp_10008 = 10008,
        SkillEffect_ThornedCounterstrike_10009 = 10009,
        SkillEffect_Heal_15001 = 15001,

        SkillEffect_AddAtk_20001 = 20001,
        SkillEffect_ReduceAtk_20002 = 20002,
        SkillEffect_AddMoveSpeed_20003 = 20003,
        SkillEffect_ReduceMoveSpeed_20004 = 20004,
        SkillEffect_AddAtkSpeed_20005 = 20005,
        SkillEffect_ReduceAtkSpeed_20006 = 20006,

        SkillEffect_Summon_23001 = 23001,
        SkillEffect_StopAction_24001 = 24001,

        SkillEffect_Charge_25001 = 25001,
        SkillEffect_DismissFlag_25002 = 25002,
        SkillEffect_CardSkillCavalryCharge_25003 = 25003,

        SkillEffect_ImmuneFlyAway_30001 = 30001,
    }

    public class SkillEffectFactory : Singleton<SkillEffectFactory>
    {
        public SkillEffectBase CreateSkillEffect(int cfgid, SkillBase skillBase, EntityBase origin, EntityBase target, params object[] args)
        {
            if (GameData.instance.SkillEffectDict.TryGetValue(cfgid, out SkillEffectData skillEffectData))
            {
                //SkillEffectData skillEffectData = GameData.instance.SkillEffectDict[cfgid];
                SkillEffectBase skillEffect;

                switch (skillEffectData.type)
                {
                    case SkillEffectType.SkillEffect_Atk1_10001:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_Atk1_10001>();
                        break;
                    case SkillEffectType.SkillEffect_FlyAway_10002:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_FlyAway_10002>();
                        break;
                    case SkillEffectType.SkillEffect_MirrorConvey_10003:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_MirrorConvey_10003>();
                        break;
                    case SkillEffectType.SkillEffect_Stealth_10004:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_Stealth_10004>();
                        break;
                    case SkillEffectType.SkillEffect_CreateBuff_10005:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_CreateBuff_10005>();
                        break;
                    case SkillEffectType.SkillEffect_AtkMaxHp_10006:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_AtkMaxHp_10006>();
                        break;
                    case SkillEffectType.SkillEffect_AtkValue_10007:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_AtkValue_10007>();
                        break;
                    case SkillEffectType.SkillEffect_LinkAtkUp_10008:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_LinkAtkUp_10008>();
                        break;
                    case SkillEffectType.SkillEffect_ThornedCounterstrike_10009:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_ThornedCounterstrike_10009>();
                        break;
                    case SkillEffectType.SkillEffect_Heal_15001:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_Heal_15001>();
                        break;

                    case SkillEffectType.SkillEffect_AddAtk_20001:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_AddAtk_20001>();
                        break;
                    case SkillEffectType.SkillEffect_ReduceAtk_20002:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_ReduceAtk_20002>();
                        break;
                    case SkillEffectType.SkillEffect_AddMoveSpeed_20003:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_AddMoveSpeed_20003>();
                        break;
                    case SkillEffectType.SkillEffect_ReduceMoveSpeed_20004:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_ReduceMoveSpeed_20004>();
                        break;
                    case SkillEffectType.SkillEffect_AddAtkSpeed_20005:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_AddAtkSpeed_20005>();
                        break;
                    case SkillEffectType.SkillEffect_ReduceAtkSpeed_20006:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_ReduceAtkSpeed_20006>();
                        break;
                    case SkillEffectType.SkillEffect_StopAction_24001:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_StopAction_24001>();
                        break;

                    case SkillEffectType.SkillEffect_Charge_25001:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_Charge_25001>();
                        break;
                    case SkillEffectType.SkillEffect_DismissFlag_25002:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_DismissFlag_25002>();
                        break;
                    case SkillEffectType.SkillEffect_CardSkillCavalryCharge_25003:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_CardSkillCavalryCharge_25003>();
                        break;

                    case SkillEffectType.SkillEffect_Summon_23001:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_Summon_23001>();
                        break;
                    case SkillEffectType.SkillEffect_ImmuneFlyAway_30001:
                        skillEffect = ClassPool.instance.Pop<SkillEffect_ImmuneFlyAway_30001>();
                        break;
                    default:
                        skillEffect = ClassPool.instance.Pop<SkillEffectBase>();
                        break;
                }

                skillEffect.Init();
                skillEffect.SkillEffectData = skillEffectData;
                skillEffect.Origin = origin;
                skillEffect.Target = target;
                skillEffect.Args = args;
#if _CLIENTLOGIC_
                if (skillEffectData.animCfgId != 0)
                    skillEffect.AnimData = AnimMgr.instance.GetAnimData(skillEffectData.animCfgId);
#endif
                //从buff创建的silleffect是不传skillBase的
                if (skillBase != null)
                {
                    skillEffect.SkillBase = skillBase;
                    skillEffect.OriginAttrValue = skillBase.OriginAttrValue;
                }
                skillEffect.Start();

                BattleMgr.instance.SkillEffectList.Add(skillEffect);

                return skillEffect;
            }
            return null;
        }
    }
}
