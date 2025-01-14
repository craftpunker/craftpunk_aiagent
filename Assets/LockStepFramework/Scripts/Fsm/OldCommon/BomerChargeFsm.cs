#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    //ը����棬��������ˣ��嵽��
    public class BomerChargeFsm : FsmState<EntityBase>
    {
        private EntityBase o;
        private Fix64 findEnemyTime = GameData.instance._FixFrameLen * 3;

        public override void OnEnter(EntityBase owner)
        {
            base.OnEnter(owner);
            o = owner;
            owner.Agent.orcaType = OrcaType.AllOrca;
#if _CLIENTLOGIC_
            owner.DoAnim(AnimType.Move);
#endif
            TimeMgr.instance.SetTimeAction(findEnemyTime, DoCheckLockEntityDistance);

            owner.TargetPos = owner.PlayerGroup == PlayerGroup.Player ?
                new FixVector2(GameData.instance.AreaSize.x, owner.Fixv3LogicPosition.y) :
                new FixVector2(-GameData.instance.AreaSize.x, owner.Fixv3LogicPosition.y);

            owner.FaceDir = owner.PlayerGroup == PlayerGroup.Player ? -1 : 1;
        }

        private void DoCheckLockEntityDistance()
        {
            if (o == null || o.BKilled)
                return;

            if (o.Fsm.GetCurrState() != o.Fsm.GetFsmState<BomerChargeFsm>())
                return;

            var agent = o.Agent;
            var rangesq = (Fix64)1000;

            Simulator.instance.kdTree_.computeAgentNeighbors(agent, ref rangesq, true);

            foreach (var nb in agent.agentNeighbors_)
            {
                if (nb.Value.entity.PlayerGroup != o.PlayerGroup)
                {
                    var target = nb.Value.entity;
                    if (nb.Key <= o.AtkRangeSq + target.RadiusSq)
                    {
                        //o.Fsm.ChangeFsmState<DeadFsm>();
                        BattleMgr.instance.Atk(o.MaxHp, null, o);
                        break;
                    }
                }
            }

            TimeMgr.instance.SetTimeAction(findEnemyTime, DoCheckLockEntityDistance);
        }

        public override void OnUpdate(EntityBase owner)
        {
            base.OnUpdate(owner);

            var pos = Simulator.instance.getAgentPositionV3(owner.Agent.id_);
            owner.Fixv3LogicPosition = pos;

            if (FixVector2.SqrMagnitude(owner.Fixv3LogicPosition.ToFixVector2() - owner.TargetPos) <= owner.AtkRangeSq)
            {
                //o.Fsm.ChangeFsmState<DeadFsm>();
                BattleMgr.instance.Atk(o.MaxHp, null, o);
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

