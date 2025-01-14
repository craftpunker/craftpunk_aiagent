using Battle;

//寻找军旗锁定的军旗的士兵
public class RemoteSearchFlagLockFlagEntitysFsm : FsmState<EntityBase>
{
    public override void OnEnter(EntityBase owner)
    {
        base.OnEnter(owner);
#if _CLIENTLOGIC_
        owner.DoAnim(AnimType.Move);
#endif
        owner.Agent.orcaType = OrcaType.CloseOrca;

        owner.TargetPos = FixVector2.None;
        Simulator.instance.setAgentPrefVelocity(owner.Agent.id_, FixVector2.Zero);
    }

    public override void OnUpdate(EntityBase owner)
    {
        base.OnUpdate(owner);

        var pos = Simulator.instance.getAgentPositionV3(owner.Agent.id_);
        owner.Fixv3LogicPosition = pos;

        var lockFlag = owner.SoldierFlag.LockTarget;

        if (lockFlag == null || lockFlag.BKilled)
        {
            return;
        }

        var lockFlagEntitys = lockFlag.Entitys;

        if (lockFlagEntitys.Count == 0)
            return;

        var index = GameData.instance.sRandom.Range(0, lockFlagEntitys.Count);
        var lockEntity = lockFlagEntitys[index];
        owner.LockEntity = lockEntity;
        owner.Fsm.ChangeFsmState<RemoteAtkFsm>();
    }

    public override void OnLeave(EntityBase owner)
    {
        base.OnLeave(owner);
    }
}
