
namespace Battle
{
    //
    public class SkillEffect_MirrorConvey_10003 : SkillEffectBase
    {
        public override void Start()
        {
            base.Start();

            var startPos = Target.Fixv3LogicPosition;
            FixVector3 targetPos = new FixVector3(-Target.Fixv3LogicPosition.x, Target.Fixv3LogicPosition.y, Fix64.Zero);
            Simulator.instance.setAgentPosition(Target.Agent.id_, targetPos);
            Target.Fixv3LogicPosition = targetPos;

            Target.SoldierFlag.IsCtorEntitys = false;
            Target.SoldierFlag.Pos = GameUtils.FindNearestGridY(Target.Fixv3LogicPosition);
            BattleMgr.instance.ToSearchEnemyFsm(Target);

#if _CLIENTLOGIC_
            //0:
            SpecialEffectFactory.instance.CreateSpecialEffect(SkillEffectData.animCfgId, startPos);
            SpecialEffectFactory.instance.CreateSpecialEffect(SkillEffectData.animCfgId, targetPos);
#endif

            BKilled = true;
        }

        public override void Release()
        {
            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}
