
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class DeadFsm : FsmState<EntityBase>
    {
        public override void OnEnter(EntityBase owner)
        {
            base.OnEnter(owner);
            owner.Agent.orcaType = OrcaType.CloseOrca;
            owner.BKilled = true;

            if(owner is WallSoldier wall)
            {
                wall.SubWall1.BKilled = true;
                wall.SubWall2.BKilled = true;
                wall.SubWall1.Agent.orcaType = OrcaType.CloseOrca;
                wall.SubWall2.Agent.orcaType = OrcaType.CloseOrca;
            }

            if (owner.DeadSkillId != 0)
            {
                SkillFactory.instance.CreateSkill(owner.DeadSkillId, owner, owner, owner.GetAtkSkillShowPos(), owner.PlayerGroup);
            }
#if _CLIENTLOGIC_
            var cfgid = 30005; //
            SpecialEffectFactory.instance.CreateSpecialEffect(cfgid, owner.Fixv3LogicPosition);
            AudioMgr.instance.PlayOneShot(AudioConf.Soldier, owner.CfgId.ToString(), AudioType.DeadAudio);
#endif
        }

        public override void OnLeave(EntityBase owner)
        {
            base.OnLeave(owner);
        }
    }
}
