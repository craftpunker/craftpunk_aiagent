#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class FlagSearchEnemyFlagFsm : FsmState<SoldierFlagBase>
    {
        public override void OnEnter(SoldierFlagBase owner)
        {
            base.OnEnter(owner);
        }

        //public override void OnUpdate(SoldierFlagBase owner)
        //{
        //    base.OnUpdate(owner);
        //    var rangesq = (Fix64)1000;
        //    owner.agentNeighbors_.Clear();
        //    Simulator.instance.soldierFlagkdTree_.queryAgentTreeRecursive(owner, ref rangesq, 0, true);

        //    foreach (var nb in owner.agentNeighbors_)
        //    {
        //        var entity = nb.Value;
        //        if (entity.PlayerGroup != owner.PlayerGroup)
        //        {
        //            //owner.LockTarget = entity;
        //            BattleMgr.instance.LockFlag(owner, entity);
        //            owner.Fsm.ChangeFsmState<FlagMoveFsm>();
        //            return;
        //        }
        //    }
        //}

        public override void OnLeave(SoldierFlagBase owner)
        {
            base.OnLeave(owner);
        }
    }
}
