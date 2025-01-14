
namespace Battle
{
    //，
    public class FlagActionFsm : FsmState<SoldierFlagBase>
    {
        public override void OnEnter(SoldierFlagBase owner)
        {
            base.OnEnter(owner);
            if (owner.IsCtorEntitys)
            {
                for (int i = 0; i < owner.Entitys.Count; i++)
                {
                    var entity = owner.Entitys[i];
                    BattleMgr.instance.ToSearchEnemyFsm(entity);
                    entity.SoldierFlag = null;
                    //if (owner.SoldierType == SoldierType.NormalSoldier)
                    //{

                    //    BattleMgr.instance.ToSearchEnemyFsm(entity);
                    //}
                    //else if (owner.SoldierType == SoldierType.RemoteSoldier)
                    //{
                    //    BattleMgr.instance.ToRemoteSearchFlagLockFlagEntitysFsm(entity);
                    //}
                }
            }

            //，
            //owner.BKilled = true;
        }

        public override void OnUpdate(SoldierFlagBase owner)
        {
            base.OnUpdate(owner);

            //if (owner.LockTarget == null || owner.LockTarget.BKilled)
            //{
            //    if (owner.IsFinishMoveX)
            //    {
            //        owner.Fsm.ChangeFsmState<FlagSearchEnemyFlagFsm>();
            //    }
            //    else
            //    {
            //        owner.Fsm.ChangeFsmState<FlagMoveXFsm>();
            //    }

            //    for (int i = 0; i < owner.Entitys.Count; i++)
            //    {
            //        var entity = owner.Entitys[i];
            //        var offsetPos = GameUtils.GetTroosEntityPos(i, owner.Entitys.Count, owner.PlayerGroup);
            //        entity.FlagOffsetPos = offsetPos;
            //        BattleMgr.instance.ToMoveToFlagOffestPosFsm(entity);
            //    }

            //    return;
            //}
        }

        public override void OnLeave(SoldierFlagBase owner)
        {
            base.OnLeave(owner);
        }
    }
}
