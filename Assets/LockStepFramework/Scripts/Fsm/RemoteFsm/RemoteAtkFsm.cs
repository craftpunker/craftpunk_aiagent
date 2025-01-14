

namespace Battle
{
    public class RemoteAtkFsm : FsmState<EntityBase>
    {
        private int stage; //0: 1: 2:

        public override void OnEnter(EntityBase owner)
        {
            base.OnEnter(owner);
#if _CLIENTLOGIC_
            owner.DoAnim(AnimType.Idle);
#endif
            stage = 0;
            owner.Agent.orcaType = OrcaType.AllOrca;
            owner.TargetPos = FixVector2.None;
            Simulator.instance.setAgentPrefVelocity(owner.Agent.id_, FixVector2.Zero);
#if _CLIENTLOGIC_
            AudioMgr.instance.PlayOneShot(AudioConf.Soldier, owner.CfgId.ToString(), AudioType.AttackAudio);
#endif
        }

        public override void OnUpdate(EntityBase owner)
        {
            base.OnUpdate(owner);

            var pos = Simulator.instance.getAgentPositionV3(owner.Agent.id_);
            owner.Fixv3LogicPosition = pos;

            if (stage == 0)
            {
                if (owner.AtkCD >= owner.CurrAttrValue.AtkSpeed - owner.CurrAttrValue.AtkReadyTime)
                {
#if _CLIENTLOGIC_
                    owner.DoAnim(AnimType.Atk);
#endif
                    stage = 1;
                    owner.AtkCD = Fix64.Zero;
                    BattleMgr.instance.FaceEnemy(owner, owner.LockEntity);
                }

                //CheckLockEnemy(owner);
            }
            else if (stage == 1)
            {
                if (owner.AtkCD >= owner.CurrAttrValue.AtkReadyTime)
                {
                    stage = 2;
                    owner.AtkCD = Fix64.Zero;
                    SkillFactory.instance.CreateSkill(owner.AtkSkillId, owner, owner.LockEntity, owner.GetAtkSkillShowPos(), owner.PlayerGroup);

                    //CheckLockEnemy(owner);
                }
            }
            else if (stage == 2)
            {
                if (owner.AtkCD >= owner.CurrAttrValue.AtkBackswing)
                {
                    stage = 0;
                    owner.AtkCD = Fix64.Zero;

                    if (!GameUtils.EntityBeKill(owner.LockEntity))
                    {
                        owner.Fsm.ChangeFsmState<RemoteAtkFsm>();
                    }
                    else
                    {
                        owner.Fsm.ChangeFsmState<RemoteSearchFlagLockFlagEntitysFsm>();
                    }
                }
            }
        }
        public override void OnLeave(EntityBase owner)
        {
            base.OnLeave(owner);
        }
    }
}