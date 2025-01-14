



namespace Battle
{
    public class MoveToEnemyFlagFsm : FsmState<EntityBase>
    {
        private Fix64 findEnemyTime = GameData.instance._FixFrameLen * 2;
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

            DoCheckLockEntityDistance();
        }

        private void DoCheckLockEntityDistance()
        {
            if(GameUtils.EntityBeKill(o))
                return;

            if (o.Fsm.GetCurrState() != o.Fsm.GetFsmState<MoveToEnemyFlagFsm>())
                return;

            var agent = o.Agent;
            var rangesq = o.AtkRange * o.AtkRange;

            Simulator.instance.kdTree_.computeAgentNeighbors(agent, ref rangesq, true);

            foreach (var nb in agent.agentNeighbors_)
            {
                var entity = nb.Value.entity;
                if (entity.PlayerGroup != o.PlayerGroup)
                {
                    o.LockEntity = nb.Value.entity;
                    o.TargetPos = o.LockEntity.Fixv3LogicPosition.ToFixVector2();
                    o.Fsm.ChangeFsmState<AtkFsm>();
                    return;
                }
            }
            TimeMgr.instance.SetTimeAction(findEnemyTime, DoCheckLockEntityDistance);
        }

        public override void OnUpdate(EntityBase owner)
        {
            base.OnUpdate(owner);

            //if(!GameUtils.EntityBeKill(owner.LockEntity))
            //{
            //    BattleMgr.instance.FaceEnemy(owner, owner.LockEntity);

            //    owner.TargetPos = owner.LockEntity.Fixv3LogicPosition.ToFixVector2();
            //    var pos = Simulator.instance.getAgentPositionV3(owner.Agent.id_);
            //    owner.Fixv3LogicPosition = pos;

            //    if (FixVector3.SqrMagnitude(owner.Fixv3LogicPosition - owner.TargetPos.ToFixVector3()) <= owner.AtkRangeSq + owner.LockEntity.RadiusSq)
            //    {
            //        owner.Fsm.ChangeFsmState<AtkFsm>();
            //    }
            //}
            //else
            //{
            //    owner.Fsm.ChangeFsmState<SearchEnemyFsm>();
            //}

            if (owner.SoldierFlag.LockTarget != null)
            {
                BattleMgr.instance.FaceFlag(owner, owner.SoldierFlag.LockTarget);
                owner.TargetPos = owner.SoldierFlag.LockTarget.Pos.ToFixVector2();
                var pos = Simulator.instance.getAgentPositionV3(owner.Agent.id_);
                owner.Fixv3LogicPosition = pos;
            }
        }

        public override void OnLeave(EntityBase owner)
        {
            o = null;
            //owner.Agent.isORCA = false;
            base.OnLeave(owner);
        }
    }
}
