

namespace Battle
{
    public class SearchEnemyFsm : FsmState<EntityBase>
    {
        //private Fix64 findEnemyTime = GameData.instance._FixFrameLen * 3;
        private EntityBase o;

        public override void OnEnter(EntityBase owner)
        {
            base.OnEnter(owner);
            o = owner;
#if _CLIENTLOGIC_
            owner.DoAnim(AnimType.Move);
#endif
            owner.Agent.orcaType = OrcaType.AllOrca;
        }

        public override void OnUpdate(EntityBase owner)
        {
            base.OnUpdate(owner);

            //owner.TargetPos = owner.LockEntity.Fixv3LogicPosition.ToFixVector2();
            //
            var pos = Simulator.instance.getAgentPositionV3(owner.Agent.id_);
            owner.Fixv3LogicPosition = pos;

            DoFindEnemy();
        }

        private void DoFindEnemy()
        {
            if (o == null || o.BKilled)
                return;

            //if (o.Fsm.GetCurrState() != o.Fsm.GetFsmState<SearchEnemyFsm>())
            //    return;

            var agent = o.Agent;
            var rangesq = (Fix64)1000;

            //o.Agent.maxNeighbors_ = BattleMgr.instance.SoldierList.Count;

            Simulator.instance.kdTree_.computeAgentNeighbors(agent, ref rangesq, true);

            foreach (var nb in agent.agentNeighbors_)
            {
                var entity = nb.Value.entity;

                if (entity.PlayerGroup != o.PlayerGroup)
                {
                    o.LockEntity = nb.Value.entity;
                    o.TargetPos = o.LockEntity.Fixv3LogicPosition.ToFixVector2();

                    o.Fsm.ChangeFsmState<MoveFsm>();
                    return;
                }
            }

            //o.Fsm.ChangeFsmState<MoveToEnemyFlagFsm>();


            // TimeMgr.instance.SetTimeAction(findEnemyTime, DoFindEnemy);
        }

        public override void OnLeave(EntityBase owner)
        {
            o.Agent.maxNeighbors_ = GameData.instance.MaxNeighbors;
            o = null;
            base.OnLeave(owner);
        }
    }
}
