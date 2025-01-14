using Battle;

//（：X。：X。）x【skr1】
public class Skill9_SpawnInFrontAtDistance : SkillBase
{
    private Fix64 distance;
    private FixVector3 dir;
    private int skfId1;

    public override void Start()
    {
        base.Start();
        distance = SkillData.fix64Args[0];
        dir = GameUtils.GetAttackDirection(Origin == null ? PlayerGroup.Player : Origin.PlayerGroup);
        FixVector3 forward = dir * distance;
        skfId1 = SkillData.skillEffectCfgIds[0];

        FixVector3 pos = Fixv3LogicPosition + forward;
        SkillEffectFactory.instance.CreateSkillEffect(skfId1, this, Origin, Target);
    }
}