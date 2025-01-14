
using System.Collections.Generic;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    //【skf1】【skf2】
    //
    public class Skill5_DoubleCircle : SkillBase
    {
        private int skfId1;
        private int skfId2;

        private Fix64 radius1;
        private Fix64 radius2;

        private FixVector3 targetPos;
        private PlayerGroup targetGroup;

        public override void Start()
        {
            base.Start();
            skfId1 = SkillData.skillEffectCfgIds[0];
            skfId2 = SkillData.skillEffectCfgIds[1];

            radius1 = SkillData.fix64Args[0];
            radius2 = SkillData.fix64Args[1];
            targetGroup = GameUtils.GetTargetGroup(PlayerGroup, SkillData.targetGroup);
            targetPos = Origin.Fixv3LogicPosition.RemoveZ();

            Fix64 rangeSq = radius1 * radius1;
            List<Agent> agents = new List<Agent>();
            var count = -1;//
            Simulator.instance.kdTree_.computeAgentNeighborsByPos(targetPos.ToFixVector2(), ref rangeSq, targetGroup, ref agents, count);

            foreach (Agent agent in agents)
            {
                SkillEffectFactory.instance.CreateSkillEffect(skfId1, this, Origin, agent.entity);
            }

            agents.Clear();

            rangeSq = radius2 * radius2;
            Simulator.instance.kdTree_.computeAgentNeighborsByPos(targetPos.ToFixVector2(), ref rangeSq, targetGroup, ref agents, count);

            foreach (Agent agent in agents)
            {
                SkillEffectFactory.instance.CreateSkillEffect(skfId2, this, Origin, agent.entity);
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
