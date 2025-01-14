
namespace Battle
{

    //1*
    public class SkillEffect_AtkMaxHp_10006 : SkillEffectBase
    {
        private Fix64 value1;
        public override void Start()
        {
            base.Start();
            value1 = SkillEffectData.fix64Args[0];
            BattleMgr.instance.Atk(Target.MaxHp * value1, Origin, Target);

            BKilled = true;
        }

        public override void Release()
        {
            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}