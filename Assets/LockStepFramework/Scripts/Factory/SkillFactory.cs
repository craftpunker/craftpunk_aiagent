
using SimpleJSON;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{

    public enum SkillType
    {
        None = 0,
        Skill1_LockTargetTrigger = 1, //锁定目标瞬间触发
        Skill2_Bezier2Landing = 2, //二阶贝塞尔曲线
        Skill3_LockTargetRange = 3, //对锁定目标单位的半径内触发技能
        Skill5_DoubleCircle = 5, //双圈技能

        Skill6_ObjectLockTargetRange = 6, //有道具发射和运动过程，对目标半径内触发【skf1】(万箭齐发)
        Skill7_DamageOverTimeRange = 7, //目标范围持续触发【skf1】
        Skill8_StraightLineAtk = 8, //直线追踪【skf1】
        Skill9_SpawnInFrontAtDistance = 9,//在自身前方（左方：X轴正方向。右方：X轴负方向。）的距离x处使用【skr1】
        Skill10_Link = 10,//闪电链

    }

    public class SkillFactory : Singleton<SkillFactory>
    {
        public SkillBase CreateSkill(int cfgid, EntityBase origin, EntityBase target, FixVector3 startPos, PlayerGroup group, params object[] args)
        {
            if (GameData.instance.SkillDict.TryGetValue(cfgid, out SkillData skillData))
            {
                //SkillData skillData = GameData.instance.SkillDict[cfgid];
                SkillBase skillBase;

                switch (skillData.type)
                {
                    case SkillType.Skill1_LockTargetTrigger:
                        skillBase = ClassPool.instance.Pop<Skill1_LockTargetTrigger>();
                        break;
                    case SkillType.Skill2_Bezier2Landing:
                        skillBase = ClassPool.instance.Pop<Skill2_Bezier2Landing>();
                        break;
                    case SkillType.Skill3_LockTargetRange:
                        skillBase = ClassPool.instance.Pop<Skill3_LockTargetRange>();
                        break;
                    case SkillType.Skill5_DoubleCircle:
                        skillBase = ClassPool.instance.Pop<Skill5_DoubleCircle>();
                        break;
                    case SkillType.Skill6_ObjectLockTargetRange:
                        skillBase = ClassPool.instance.Pop<Skill6_ObjectLockTargetRange>();
                        break;
                    case SkillType.Skill7_DamageOverTimeRange:
                        skillBase = ClassPool.instance.Pop<Skill7_DamageOverTimeRange>();
                        break;
                    case SkillType.Skill8_StraightLineAtk:
                        skillBase = ClassPool.instance.Pop<Skill8_StraightLineAtk>();
                        break;
                    case SkillType.Skill9_SpawnInFrontAtDistance:
                        skillBase = ClassPool.instance.Pop<Skill9_SpawnInFrontAtDistance>();
                        break;
                    case SkillType.Skill10_Link:
                        skillBase = ClassPool.instance.Pop<Skill10_Link>();
                        break;
                    default:
                        skillBase = ClassPool.instance.Pop<SkillBase>();
                        break;
                }

                skillBase.Init();
                skillBase.SkillData = skillData;
                skillBase.Origin = origin;
                skillBase.Target = target;
                skillBase.PlayerGroup = group;
                if (origin != null)
                    skillBase.OriginAttrValue = origin.GetEntityAttrValueCopy();
                skillBase.Fixv3LogicPosition = startPos;
                skillBase.Fixv3LastPosition = startPos;

                if (skillData.animCfgIds.Count != 0)
                {
                    skillBase.MainCfgid = skillData.animCfgIds[0];
#if _CLIENTLOGIC_
                    skillBase.AnimData = AnimMgr.instance.GetAnimData(skillBase.MainCfgid);
#endif
                }
#if _CLIENTLOGIC_
                skillBase.UpdateRenderPosition(0);
#endif
                skillBase.Start();

                BattleMgr.instance.SkillList.Add(skillBase);

                return skillBase;
            }

            return null;
        }
    }
}
