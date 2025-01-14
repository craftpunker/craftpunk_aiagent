namespace Battle
{
    //X%
    public class SkillEffect_ThornedCounterstrike_10009 : SkillEffectBase
    {
        private Fix64 atkRange;
        private Fix64 value2; //

        public override void Start()
        {
            base.Start();
            atkRange = SkillEffectData.fix64Args[0];
            value2 = SkillEffectData.fix64Args[1];

            Target.BeAtkAction += BeAtk;
        }

        public void BeAtk(EntityBase atker, Fix64 value)
        {
            if (atker.AtkRange <= atkRange)
            {
                Fix64 atk = value * value2;
                BattleMgr.instance.ChangeHp(atk, atker);
            }
        }

        public override void Release()
        {
            base.Release();
        }
    }
}
