#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    //
    public class MoveToFlagOffestPosFsm : FsmState<EntityBase>
    {
        private Fix64 sqrMagnitude = (Fix64)4;
        private Fix64 addMoveSpeedFactor = (Fix64)2;
        private Fix64 addMoveSpeed;

        private Fix64 findEnemyTime = GameData.instance._FixFrameLen * 2;
        private EntityBase o;

        public override void OnEnter(EntityBase owner)
        {
            base.OnEnter(owner);
            addMoveSpeed = Fix64.Zero;
            owner.Agent.orcaType = OrcaType.NoTeamOrca;
#if _CLIENTLOGIC_
            owner.DoAnim(AnimType.Move);
#endif
            o = owner;

            TimeMgr.instance.SetTimeAction(findEnemyTime, DoCheckLockEntityDistance);
        }

        private void DoCheckLockEntityDistance()
        {
            if (GameUtils.EntityBeKill(o))
                return;

            if (o.Fsm.GetCurrState() != o.Fsm.GetFsmState<MoveToFlagOffestPosFsm>())
                return;

            //o.Agent.maxNeighbors_ = GameData.instance.MaxNeighbors;//BattleMgr.instance.SoldierList.Count;
            var agent = o.Agent;
            var rangesq = o.AtkRangeSq + 1.2;
            Simulator.instance.kdTree_.computeAgentNeighbors(agent, ref rangesq, true);

            foreach (var nb in agent.agentNeighbors_)
            {
                var entity = nb.Value.entity;
                if (entity.PlayerGroup != o.PlayerGroup)
                {
                    o.LockEntity = entity;
                    o.TargetPos = o.LockEntity.Fixv3LogicPosition.ToFixVector2();
                    //o.Fsm.ChangeFsmState<AtkFsm>();
                    o.Fsm.ChangeFsmState<MoveFsm>();
                    o.SoldierFlag.RemoveSoldier(o);
                    o.SoldierFlag = null;
                    return;
                }
            }

            TimeMgr.instance.SetTimeAction(findEnemyTime, DoCheckLockEntityDistance);
        }

        public override void OnUpdate(EntityBase owner)
        {
            base.OnUpdate(owner);

            if (owner.SoldierFlag == null)
            {
                owner.Fsm.ChangeFsmState<SearchEnemyFsm>();
                return;
            }
            owner.TargetPos = owner.SoldierFlag.Pos.ToFixVector2() + owner.FlagOffsetPos;

            var pos = Simulator.instance.getAgentPositionV3(owner.Agent.id_);
            owner.Fixv3LogicPosition = pos;

            //if (FixVector2.SqrMagnitude(owner.TargetPos - owner.Fixv3LogicPosition.ToFixVector2()) > sqrMagnitude)
            //{
            //    if (addMoveSpeed == Fix64.Zero)
            //    {
            //        addMoveSpeed = owner.MoveSpeed * addMoveSpeedFactor;
            //        owner.CurrAttrValue.MoveSpeed += addMoveSpeed;
            //    }
            //}
            //else
            //{
            //    if (addMoveSpeed != Fix64.Zero)
            //    {
            //        owner.CurrAttrValue.MoveSpeed -= addMoveSpeed;
            //        addMoveSpeed = Fix64.Zero;
            //    }
            //}

            BattleMgr.instance.FaceMovePos(owner);
        }

        public override void OnLeave(EntityBase owner)
        {
            owner.CurrAttrValue.MoveSpeed -= addMoveSpeed;
            base.OnLeave(owner);
        }
    }
}
