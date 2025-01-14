

namespace Battle
{
    public class FlagMoveXFsm : FsmState<SoldierFlagBase>
    {
        private Fix64 findEnemyTime = GameData.instance._FixFrameLen;
        private SoldierFlagBase o;

        public override void OnEnter(SoldierFlagBase owner)
        {
            base.OnEnter(owner);
            if (owner.IsCtorEntitys)
            {
                for (int i = 0; i < owner.Entitys.Count; i++)
                {
                    var entity = owner.Entitys[i];
                    var offsetPos = GameUtils.GetTroosEntityPos(i, owner.Entitys.Count, owner.PlayerGroup);
                    entity.FlagOffsetPos = offsetPos;
                    BattleMgr.instance.ToMoveToFlagOffestPosFsm(entity);
                }
            }

            //o = owner;
            //TimeMgr.instance.SetTimeAction(findEnemyTime, DoCheckLockEntityDistance);
        }

        //private void DoCheckLockEntityDistance()
        //{
        //    if (GameUtils.EntityBeKill(o))
        //        return;

        //    if (o.Fsm.GetCurrState() != o.Fsm.GetFsmState<FlagMoveXFsm>())
        //        return;

        //    var rangesq = o.ActionDistance * o.ActionDistance;

        //    Simulator.instance.soldierFlagkdTree_.queryAgentTreeRecursive(o, ref rangesq, 0, true);

        //    foreach (var nb in o.agentNeighbors_)
        //    {
        //        var entity = nb.Value;
        //        if (entity.PlayerGroup != o.PlayerGroup)
        //        {
        //            BattleMgr.instance.LockFlag(o, entity);
        //            o.Fsm.ChangeFsmState<FlagMoveFsm>();
        //            return;
        //        }
        //    }
        //    TimeMgr.instance.SetTimeAction(findEnemyTime, DoCheckLockEntityDistance);
        //}

        public override void OnUpdate(SoldierFlagBase owner)
        {
            base.OnUpdate(owner);

            owner.Pos += owner.RowForward * owner.MoveSpeed * GameData.instance._FixFrameLen;
            //owner.obj.transform.position = owner.Pos.ToVector3();

            if (owner.PlayerGroup == PlayerGroup.Player)
            {
                if(owner.Pos.x > owner.FreeSeekDis)
                {
                    owner.Fsm.ChangeFsmState<FlagActionFsm>();
                    owner.IsFinishMoveX = true;
                }
            }
            else
            {
                if (owner.Pos.x < owner.FreeSeekDis)
                {
                    owner.Fsm.ChangeFsmState<FlagActionFsm>();
                    owner.IsFinishMoveX = true;
                }
            }
        }

        public override void OnLeave(SoldierFlagBase owner)
        {
            o = null;
            base.OnLeave(owner);
        }
    }
}
