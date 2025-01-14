using Battle;

public class SkillEffect_LinkHealUp_15002 : SkillEffectBase
{
    private Fix64 addUp; //
    private Fix64 value; // 0：， >0  
    private int count; //

    public override void Start()
    {
        base.Start();

        addUp = SkillEffectData.fix64Args[0];
        value = SkillEffectData.fix64Args[1];
        count = 1;

        if (Args.Length > 0)
        {
            count = (int)Args[0];
        }

        if (value == Fix64.Zero)
        {
            value = Origin.CurrAttrValue.Atk;
        }

        BattleMgr.instance.Heal(value * (Fix64.One + addUp * count), Target);
    }
}