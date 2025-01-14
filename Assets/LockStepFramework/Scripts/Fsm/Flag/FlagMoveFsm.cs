
namespace Battle
{
    public class FlagMoveFsm : FsmState<SoldierFlagBase>
    {
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
                    //entity.Fsm.ChangeFsmState<MoveToFlagOffestPosFsm>();
                }
            }
        }

        public override void OnUpdate(SoldierFlagBase owner)
        {
            base.OnUpdate(owner);

            if(owner.LockTarget == null || owner.LockTarget.BKilled)
            {
                owner.Fsm.ChangeFsmState<FlagActionFsm>();
                return;
            }

            var distance = owner.LockTarget.Pos - owner.Pos;
            owner.Forward = distance.GetNormalized();
            //owner.Right = FixVector3.Cross(owner.Forward, GameData.instance._Hight);
            owner.Pos += owner.Forward * owner.MoveSpeed * GameData.instance._FixFrameLen;
            //owner.obj.transform.position = owner.Pos.ToVector3();

            if (FixVector3.SqrMagnitude(distance) <= owner.ActionDistance * owner.ActionDistance)
            {
                owner.Fsm.ChangeFsmState<FlagActionFsm>();
            }
        }

        public override void OnLeave(SoldierFlagBase owner)
        {
            base.OnLeave(owner);
        }
    }
}
