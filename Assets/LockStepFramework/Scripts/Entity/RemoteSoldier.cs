#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class RemoteSoldier : EntityBase
    {

        public override void Start()
        {
            base.Start();

            Fsm = ClassPool.instance.Pop<FsmCompent<EntityBase>>();
            Fsm.CreateFsm(this,
                ClassPool.instance.Pop<AtkFsm>(),
                ClassPool.instance.Pop<BornFsm>(),
                ClassPool.instance.Pop<DeadFsm>(),
                ClassPool.instance.Pop<IdleFsm>(),
                ClassPool.instance.Pop<MoveToFlagOffestPosFsm>(),
                ClassPool.instance.Pop<SearchEnemyFsm>(),
                ClassPool.instance.Pop<MoveFsm>(),
                ClassPool.instance.Pop<SkefCtorFsm>()
                //ClassPool.instance.Pop<RemoteSearchFlagLockFlagEntitysFsm>()
                );

#if _CLIENTLOGIC_
            CreateFromPrefab(AnimData.Prefab, (go) =>
            {
                Trans.position = Fixv3LogicPosition.ToVector3();
                SetMeshBlock();

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
