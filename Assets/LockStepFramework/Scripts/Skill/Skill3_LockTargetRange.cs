
using System.Collections.Generic;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    //【skf1】
    //
    public class Skill3_LockTargetRange : SkillBase
    {
        private int skfId1;

        private Fix64 radius;
        private int count;

        private FixVector3 targetPos;
        private PlayerGroup targetGroup;

        public override void Start()
        {
            base.Start();
            skfId1 = SkillData.skillEffectCfgIds[0];

            radius = SkillData.fix64Args[0];
            count = (int)SkillData.fix64Args[1];
            var selfOrLockEnemy = (int)SkillData.fix64Args[2];
            targetGroup = GameUtils.GetTargetGroup(PlayerGroup, SkillData.targetGroup);

            targetPos = selfOrLockEnemy == 0 ? (Origin == null ? Fixv3LogicPosition : Origin.Fixv3LogicPosition.RemoveZ()) : Target.Fixv3LogicPosition.RemoveZ();

            Fix64 rangeSq = radius * radius;
            List<Agent> agents = new List<Agent>();
            Simulator.instance.kdTree_.computeAgentNeighborsByPos(targetPos.ToFixVector2(), ref rangeSq, targetGroup, ref agents, count);
            foreach (Agent agent in agents)
            {
                SkillEffectFactory.instance.CreateSkillEffect(skfId1, this, Origin, agent.entity);
            }

#if _CLIENTLOGIC_
            //0:
            SpecialEffectFactory.instance.CreateSpecialEffect(SkillData.animCfgIds, 0, targetPos);
#endif

            BKilled = true;
        }

        public override void Release()
        {
            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}
