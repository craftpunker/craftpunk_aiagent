
#if _CLIENTLOGIC_
using UnityEngine;
#endif


namespace Battle
{
    public class BornFsm : FsmState<EntityBase>
    {
        private Fix64 bornTime = (Fix64)0.4;
        private Fix64 time;

        public override void OnEnter(EntityBase owner)
        {
            base.OnEnter(owner);

            time = Fix64.Zero;

            if (owner.BornSkillId != 0)
            {
                SkillFactory.instance.CreateSkill(owner.BornSkillId, owner, owner, owner.GetAtkSkillShowPos(), owner.PlayerGroup);
            }

            if (owner.IsSummon)
            {
                TimeMgr.instance.SetTimeAction(bornTime, () =>
                {
#if _CLIENTLOGIC_
                    owner.SetAlpha(1);
#endif
                    if(!GameUtils.EntityBeKill(owner))
                        owner.Fsm?.ChangeFsmState<SearchEnemyFsm>();
                });
            }

            //if (owner.SoldierType == SoldierType.BomerSoldier)
            //{
            //    owner.Fsm.ChangeFsmState<BomerChargeFsm>();
            //}
            //else if (owner.SoldierType == SoldierType.WallSoldier) //
            //{
            //    //owner.Fsm.ChangeFsmState<IdleFsm>();
            //}
            //else
            //{
            //    owner.Fsm.ChangeFsmState<SearchEnemyFsm>();
            //}
        }

        public override void OnUpdate(EntityBase owner)
        {
            base.OnUpdate(owner);

#if _CLIENTLOGIC_
            if (!owner.IsSummon)
                return;

            time += GameData.instance._FixFrameLen;
            var t = time / bornTime;

            if (t >= Fix64.One)
                return;

            owner.SetAlpha((float)t);
#endif
        }
    }
}
