
using SimpleJSON;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{

    public enum SkillType
    {
        None = 0,
        Skill1_LockTargetTrigger = 1, //����Ŀ��˲�䴥��
        Skill2_Bezier2Landing = 2, //���ױ���������
        Skill3_LockTargetRange = 3, //������Ŀ�굥λ�İ뾶�ڴ�������
        Skill5_DoubleCircle = 5, //˫Ȧ����

        Skill6_ObjectLockTargetRange = 6, //�е��߷�����˶����̣���Ŀ��뾶�ڴ�����skf1��(����뷢)
        Skill7_DamageOverTimeRange = 7, //Ŀ�귶Χ����������skf1��
        Skill8_StraightLineAtk = 8, //ֱ��׷�١�skf1��
        Skill9_SpawnInFrontAtDistance = 9,//������ǰ�����󷽣�X���������ҷ���X�Ḻ���򡣣��ľ���x��ʹ�á�skr1��
        Skill10_Link = 10,//������

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
