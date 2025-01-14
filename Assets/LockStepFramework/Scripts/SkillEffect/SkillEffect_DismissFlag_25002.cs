
//
namespace Battle
{
    public class SkillEffect_DismissFlag_25002 : SkillEffectBase
    {
        public override void Start()
        {
            base.Start();

            Target.SoldierFlag.IsCtorEntitys = false;
            BattleMgr.instance.ToSearchEnemyFsm(Target);
        }
    }
}