
using Battle;

public class SkillEffect_Heal_15001 : SkillEffectBase
{
    private Fix64 value;

    public override void Start()
    {
        base.Start();

        value = SkillEffectData.fix64Args[0];

        BattleMgr.instance.Heal(value * Origin.CurrAttrValue.Atk, Target);
    }
}