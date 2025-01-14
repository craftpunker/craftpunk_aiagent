using Battle;

public class SkefCtorFsm : FsmState<EntityBase>
{
    public override void OnEnter(EntityBase owner)
    {
        base.OnEnter(owner);

        //if(owner.SoldierType != 3) //
        Simulator.instance.setAgentPosition(owner.Agent.id_, owner.Fixv3LogicPosition);
        Simulator.instance.setAgentPrefVelocity(owner.Agent.id_, FixVector2.Zero);
        owner.TargetPos = FixVector2.None;
#if _CLIENTLOGIC_
        owner.DoAnim(AnimType.Idle);
#endif
        owner.Agent.orcaType = OrcaType.CloseOrca;
        owner.LockEntity = null;
    }
}