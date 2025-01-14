#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class BomerSoldier : EntityBase
    {
        public override void Start()
        {
            base.Start();

            Fsm = ClassPool.instance.Pop<FsmCompent<EntityBase>>();
            Fsm.CreateFsm(this,
                ClassPool.instance.Pop<BornFsm>(),
                ClassPool.instance.Pop<BomerChargeFsm>(),
                ClassPool.instance.Pop<DeadFsm>(),
                ClassPool.instance.Pop<IdleFsm>()
             );

#if _CLIENTLOGIC_
            CreateFromPrefab(AnimData.Prefab, (go) =>
            {
                SetMeshBlock();
                Trans.position = Fixv3LogicPosition.ToVector3();
                go.SetActive(true);
            });
#endif
        }

        public override void StartLogic()
        {
            base.StartLogic();

            Fsm.OnStart<BornFsm>();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            Fsm.OnUpdate(this);

            //if (text != null)
            //{
            //    text.text = Fsm.GetCurrState().ToString();
            //}
        }

        public override void Release()
        {
            ClassPool.instance.Push(this);


            base.Release();
        }
    }
}