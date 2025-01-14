

namespace Battle
{
    //12,3
    public class SkillEffect_FlyAway_10002 : SkillEffectBase
    {
        private Fix64 distance;
        private Fix64 hight;
        private Fix64 maxTime;

        private Fix64 time;
        private FixVector3 startPos;
        private FixVector3 hightPos;
        private FixVector3 endPos;

        public override void Start()
        {
            base.Start();

            //
            if (BattleMgr.instance.CheckStage(Target, StageConst.FlyAway) || BattleMgr.instance.CheckStage(Target, StageConst.ImmuneFlyAway))
            {
                BKilled = true;
                return;
            }

            //

            distance = SkillEffectData.fix64Args[0];
            hight = SkillEffectData.fix64Args[1];
            maxTime = SkillEffectData.fix64Args[2];

            time = Fix64.Zero;
            startPos = Target.Fixv3LogicPosition;

            var dir = (Target.Fixv3LogicPosition - Origin.Fixv3LogicPosition).GetNormalized();
            endPos = startPos + dir * distance;
            hightPos = startPos + (endPos - startPos) / (Fix64)2 + GameData.instance._Hight * hight;

            if (Target != null && Target.Agent != null)
            {
                BattleMgr.instance.AddStage(Target, StageConst.FlyAway);
                Target.Agent.orcaType = OrcaType.CloseOrca;
                Simulator.instance.setAgentPrefVelocity(Target.Agent.id_, FixVector2.Zero);
            }

            BattleMgr.instance.ToIdleFsm(Target);
        }

        public override void Update()
        {
            base.Update();
            if (GameUtils.EntityBeKill(Target))
            {
                BKilled = true;
                return;
            }

            time += GameData.instance._FixFrameLen;
            var t = time / maxTime;

            var v3 = MathUtils.Bezier2(startPos, hightPos, endPos, t);
            var z = v3.z;
            var v2 = GameUtils.CheckMapBorder(v3.ToFixVector2());
            Target.Fixv3LogicPosition = new FixVector3(v2.x, v2.y, z);

            if (t > 1)
            {
                Target.Fixv3LogicPosition = Target.Fixv3LogicPosition.RemoveZ();
                Simulator.instance.setAgentPosition(Target.Agent.id_, Target.Fixv3LogicPosition);
                //Target.Fsm.ChangeFsmState<SearchEnemyFsm>();
                BattleMgr.instance.ToSearchEnemyFsm(Target);
                BKilled = true;
                BattleMgr.instance.RemoveStage(Target, StageConst.FlyAway);
                Target.Agent.orcaType = OrcaType.AllOrca;
            }
        }

        public override void Release()
        {

            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}