
namespace Battle
{
    // * 1
    public class SkillEffect_AddAtkSpeed_20005 : SkillEffectBase
    {
        private Fix64 value1;
        public override void Start()
        {
            base.Start();
            value1 = SkillEffectData.fix64Args[0];
            BattleMgr.instance.ChangeAtkSpeed(Target, Target.AtkSpeed * value1);
        }

        public override void Release()
        {
            BattleMgr.instance.ChangeAtkSpeed(Target, -Target.AtkSpeed * value1);
            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}