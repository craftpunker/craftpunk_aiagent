using Battle;
using System.Collections.Generic;


public class SkillEffect_Charge_25001 : SkillEffectBase
{
    private Fix64 value1; //
    private Fix64 value2; //
    private Fix64 value3; //

    private Fix64 totalAddSpeed;
    private Fix64 stepAddSpeed;
    private Fix64 maxAddSpeed;

    //private Fix64 findEnemyTime = GameData.instance._FixFrameLen * 3;

    private Fix64 distance = (Fix64)1.5;
    private Fix64 dotValue = (Fix64)0.5; //cos60

    private FixVector3 forward;
    private PlayerGroup targetGroup;
    private List<Agent> agents;

#if _CLIENTLOGIC_
    private SpecialEffect chargeSpecEff; //
#endif

    public override void Start()
    {
        base.Start();
        //Target.SoldierFlag.SoldierBKill(Target); //
        Target.SoldierFlag.IsCtorEntitys = false; //
        BattleMgr.instance.ToSkefCtorFsm(Target, this);

        value1 = SkillEffectData.fix64Args[0];
        value2 = SkillEffectData.fix64Args[1];
        value3 = SkillEffectData.fix64Args[2];

        stepAddSpeed = Target.MoveSpeed * value1 * GameData.instance._FixFrameLen;
        maxAddSpeed = Target.MoveSpeed * value3;
        totalAddSpeed = Fix64.Zero;

        targetGroup = GameUtils.GetTargetGroup(Target.PlayerGroup, PlayerGroup.Enemy);
        forward = GameUtils.GetGroupForward(Target.PlayerGroup, (Fix64)1);
#if _CLIENTLOGIC_
        Target.DoAnim(AnimType.Move);

        chargeSpecEff = SpecialEffectFactory.instance.CreateSpecialEffect(SkillEffectData.animCfgId, Target.Fixv3LogicPosition);
        chargeSpecEff.FaceDir = Target.PlayerGroup == PlayerGroup.Player ? -1 : 1;
#endif
        BattleMgr.instance.FacePos(Target, forward);
        Target.TargetPos = new FixVector2(forward.x, Target.Fixv3LogicPosition.y);

        agents = new List<Agent>();
    }

    private bool DoCheckDistance()
    {
        var agent = Target.Agent;
        var rangesq = distance;

        //Simulator.instance.kdTree_.computeAgentNeighbors(agent, ref rangesq, true);
        agents.Clear();
        Simulator.instance.kdTree_.computeAgentNeighborsByPos(Target.Fixv3LogicPosition.ToFixVector2(), ref rangesq, targetGroup, ref agents, 1);
        foreach (var nb in agents)
        {
            var entity = nb.entity;
            if (entity.PlayerGroup != Target.PlayerGroup)
            {
                if (CheckFace(entity))
                {
                    BattleMgr.instance.Atk(value2 * Target.Atk, Target, entity);
                    return true;
                }
            }
        }

        return false;
    }

    private bool CheckFace(EntityBase target)
    {
        var dir = forward.GetNormalized();
        var m2tDir = (target.Fixv3LogicPosition - Target.Fixv3LogicPosition).GetNormalized();
        var dot = FixVector3.DotNoZ(dir, m2tDir);

        if (dot >= dotValue)
        {
            return true;
        }

        return false;
    }

    public override void Update()
    {
        base.Update();

        if (GameUtils.EntityBeKill(Target))
        {
            BKilled = true;
            return;
        }

        var addSpeed = totalAddSpeed + stepAddSpeed;
        if (addSpeed <= maxAddSpeed)
        {
            totalAddSpeed = addSpeed;
            BattleMgr.instance.ChangeMoveSpeed(Target, stepAddSpeed);
        }

        var pos = Simulator.instance.getAgentPositionV3(Target.Agent.id_);
        Target.Fixv3LogicPosition = pos;

#if _CLIENTLOGIC_
        chargeSpecEff.Fixv3LogicPosition = Target.Fixv3LogicPosition;
#endif

        if (DoCheckDistance())
        {
            ToSearchEnemyFsm();
            return;
        }

        if (CheckMoveOverX(Target))
        {
            ToSearchEnemyFsm();
            return;
        }
    }

    //XX
    private bool CheckMoveOverX(EntityBase target)
    {
        if (target.PlayerGroup == PlayerGroup.Player)
        {
            if (target.Fixv3LogicPosition.x > forward.x)
            {
                return true;
            }
        }
        else
        {
            if (target.Fixv3LogicPosition.x < forward.x)
            {
                return true;
            }
        }

        return false;
    }

    private void ToSearchEnemyFsm()
    {
        if (Target.SoldierFlag != null)
        {
            Target.SoldierFlag.Pos = GameUtils.FindNearestGridY(Target.Fixv3LogicPosition);
            Target.SoldierFlag = null;
        }

        Simulator.instance.setAgentPosition(Target.Agent.id_, Target.Fixv3LogicPosition);
        Simulator.instance.setAgentPrefVelocity(Target.Agent.id_, FixVector2.Zero);
        Target.TargetPos = FixVector2.None;

        BattleMgr.instance.ToSearchEnemyFsm(Target);
        BKilled = true;
    }

    public override void Release()
    {
#if _CLIENTLOGIC_
        chargeSpecEff.BKilled = true;
        chargeSpecEff = null;
#endif
        BattleMgr.instance.ChangeMoveSpeed(Target, -totalAddSpeed);
        ClassPool.instance.Push(this);
        base.Release();
    }
}