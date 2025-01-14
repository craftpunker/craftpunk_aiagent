
using System.Collections.Generic;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    //,【skf1】 ()
    //
    public class Skill4_GravityParabola : SkillBase
    {
        private int skfId1;
        private Fix64 speed;
        private Fix64 hight;
        private Fix64 radius;
        private int count;
        private PlayerGroup targetGroup;

        private Fix64 time;
        private Fix64 moveTime;
        private FixVector3 targetPos;
        private FixVector3 startPos;
        private FixVector3 start2Target;

        public override void Start()
        {
            base.Start();

            skfId1 = SkillData.skillEffectCfgIds[0];
            speed = SkillData.fix64Args[0];
            hight = SkillData.fix64Args[1];
            radius = SkillData.fix64Args[2];
            count = (int)SkillData.fix64Args[3];
            targetGroup = GameUtils.GetTargetGroup(PlayerGroup, SkillData.targetGroup);

            time = Fix64.Zero;
            targetPos = Target.Fixv3LogicPosition.RemoveZ();
            startPos = Origin.Fixv3LogicPosition.RemoveZ();
            moveTime = FixVector3.Distance(targetPos, startPos) / speed;

            start2Target = targetPos - startPos;

#if _CLIENTLOGIC_
            CreateFromPrefab(AnimData.Prefab, (go) =>
            {
                SetMeshBlock();
                DoAnim(AnimType.Idle);
                BattleMgr.instance.FaceEnemy(Origin, Target);
                go.SetActive(true);
            });

            SpecialEffectFactory.instance.CreateSpecialEffect(SkillData.animCfgIds, 0, targetPos);
#endif

        }

        public override void Update()
        {
            base.Update();

            time += GameData.instance._FixFrameLen;
            var t = time / moveTime;

            if (t > Fix64.One)
            {
                Fixv3LogicPosition = targetPos;
                Fix64 rangeSq = radius * radius;
                List<Agent> agents = new List<Agent>();
                Simulator.instance.kdTree_.computeAgentNeighborsByPos(targetPos.ToFixVector2(), ref rangeSq, targetGroup, ref agents, count);
                foreach (Agent agent in agents)
                {
                    SkillEffectFactory.instance.CreateSkillEffect(skfId1, this, Origin, agent.entity);
                }
                BKilled = true;

#if _CLIENTLOGIC_
                //1:
                SpecialEffectFactory.instance.CreateSpecialEffect(SkillData.animCfgIds, 0, targetPos);
#endif
                return;
            }

        }

        public override void Release()
        {
            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}
