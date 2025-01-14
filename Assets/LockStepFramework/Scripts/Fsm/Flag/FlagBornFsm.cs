#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class FlagBornFsm : FsmState<SoldierFlagBase>
    {
        public override void OnEnter(SoldierFlagBase owner)
        {
            base.OnEnter(owner);

            for (int i = owner.Entitys.Count - 1; i >= 0; i--)
            {
                var entity = owner.Entitys[i];
                entity.StartLogic();
            }

            owner.Fsm.ChangeFsmState<FlagMoveXFsm>();
        }
    }
}
