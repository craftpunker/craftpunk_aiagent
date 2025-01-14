

namespace Battle
{
    public class MoveXFsm : FsmState<EntityBase>
    {
        private Fix64 findEnemyTime = GameData.instance._FixFrameLen * 5;
        //private Fix64 maxX = (Fix64)0.5;
        private EntityBase o;

        public override void OnEnter(EntityBase owner)
        {
            base.OnEnter(owner);
            o = owner;
            owner.Agent.orcaType = OrcaType.AllOrca;
#if _CLIENTLOGIC_
            owner.DoAnim(AnimType.Move);
#endif
            TimeMgr.instance.SetTimeAction(findEnemyTime, DoCheckLockEntityDistance);
        }

        private void DoCheckLockEntityDistance()
        {
            if (GameUtils.EntityBeKill(o))
                return;

            if (o.Fsm.GetCurrState() != o.Fsm.GetFsmState<MoveXFsm>())
                return;

            var agent = o.Agent;
            var rangesq = (Fix64)1000;

            Simulator.instance.kdTree_.computeAgentNeighbors(agent, ref rangesq, true);

            foreach (var nb in agent.agentNeighbors_)
            {
                var entity = nb.Value.entity;
                if (entity.PlayerGroup != o.PlayerGroup)
                {
                    o.LockEntity = nb.Value.entity;
                    o.TargetPos = new FixVector2(o.LockEntity.Fixv3LogicPosition.x, o.Fixv3LogicPosition.y);
                    break;
                }
            }

            TimeMgr.instance.SetTimeAction(findEnemyTime, DoCheckLockEntityDistance);
        }

        public override void OnUpdate(EntityBase owner)
        {
            base.OnUpdate(owner);

            if (!GameUtils.EntityBeKill(owner.LockEntity))
            {
                BattleMgr.instance.FaceEnemy(owner, owner.LockEntity);

                owner.TargetPos = new FixVector2(owner.LockEntity.Fixv3LogicPosition.x, owner.Fixv3LogicPosition.y);
                var pos = Simulator.instance.getAgentPosition(owner.Agent.id_);
                owner.Fixv3LogicPosition = pos.ToFixVector3();

                if (Fix64.Abs(owner.Fixv3LogicPosition.x - owner.LockEntity.Fixv3LogicPosition.x) <= owner.AtkRangeSq)
                {
                    owner.Fsm.ChangeFsmState<MoveFsm>();
                    //owner.IsMoveX = false;
                }
            }
            else
            {
                owner.Fsm.ChangeFsmState<SearchEnemyFsm>();
            }
        }

        public override void OnLeave(EntityBase owner)
        {
            o = null;
            base.OnLeave(owner);
            //owner.Agent.isORCA = false;
        }
    }
}
