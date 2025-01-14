
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class WallSoldier : EntityBase
    {
        public SubWallSoldier SubWall1;
        public SubWallSoldier SubWall2;
        private Fix64 dir; //01

        public override void Start()
        {
            base.Start();

            Fsm = ClassPool.instance.Pop<FsmCompent<EntityBase>>();
            Fsm.CreateFsm(this,
                ClassPool.instance.Pop<BornFsm>(),
                ClassPool.instance.Pop<DeadFsm>()
                //ClassPool.instance.Pop<IdleFsm>()
             );

            dir = MoveSpeed;
            FixVector3 subWall1StartPos;
            FixVector3 subWall2StartPos;

            if (dir == Fix64.Zero)
            {
                subWall1StartPos = new FixVector3(Fixv3LogicPosition.x - Radius, Fixv3LogicPosition.y, Fix64.Zero);
                subWall2StartPos = new FixVector3(Fixv3LogicPosition.x + Radius, Fixv3LogicPosition.y, Fix64.Zero);
            }
            else
            {
                subWall1StartPos = new FixVector3(Fixv3LogicPosition.x, Fixv3LogicPosition.y - Radius, Fix64.Zero);
                subWall2StartPos = new FixVector3(Fixv3LogicPosition.x, Fixv3LogicPosition.y + Radius, Fix64.Zero);
            }

            TroopsData.soldierType = (int)SoldierType.SubWallSoldier;

            SubWall1 = SoldierFactory.instance.CreateSolider(TroopsData, subWall1StartPos, PlayerGroup) as SubWallSoldier;
            SubWall2 = SoldierFactory.instance.CreateSolider(TroopsData, subWall2StartPos, PlayerGroup) as SubWallSoldier;

            TroopsData.soldierType = (int)SoldierType.WallSoldier;

            SubWall1.MainWallSoldier = this;
            SubWall2.MainWallSoldier = this;

            SkillFactory.instance.CreateSkill(BornSkillId, SubWall1, SubWall1, SubWall1.GetAtkSkillShowPos(), SubWall1.PlayerGroup);
            SkillFactory.instance.CreateSkill(BornSkillId, SubWall2, SubWall2, SubWall2.GetAtkSkillShowPos(), SubWall2.PlayerGroup);

            //Agent.isORCA = false;

#if _CLIENTLOGIC_
            CreateFromPrefab(AnimData.Prefab, (go) =>
            {
                SetMeshBlock();
                Trans.position = Fixv3LogicPosition.ToVector3();
                if (dir == Fix64.Zero)
                    DoAnim(AnimType.Idle);
                else
                    DoAnim(AnimType.Move);

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
            SubWall1 = null;
            SubWall2 = null;

            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}