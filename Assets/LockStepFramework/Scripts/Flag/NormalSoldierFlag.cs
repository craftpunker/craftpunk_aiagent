

#if _CLIENTLOGIC_
using UnityEngine;
#endif
namespace Battle
{
    public class NormalSoldierFlag : SoldierFlagBase
    {
        public override void Init()
        {
            base.Init();
        }

        public override void Start()
        {
            base.Start();

            Fsm = ClassPool.instance.Pop<FsmCompent<SoldierFlagBase>>();
            Fsm.CreateFsm(this,
                 //ClassPool.instance.Pop<FlagMoveFsm>(),
                 ClassPool.instance.Pop<FlagActionFsm>(),
                 ClassPool.instance.Pop<FlagBornFsm>(),
                 //ClassPool.instance.Pop<FlagSearchEnemyFlagFsm>(),
                 ClassPool.instance.Pop<FlagMoveXFsm>()
                );

            //obj = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            //obj.transform.position = Pos.ToVector3();
            //obj.transform.localScale = Vector3.one * 0.5f;
        }

        public override void StartLogic()
        {
            base.StartLogic();
            Fsm.OnStart<FlagBornFsm>();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Release()
        {
            //GameObject.Destroy(obj.gameObject);
            //obj = null;
            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}