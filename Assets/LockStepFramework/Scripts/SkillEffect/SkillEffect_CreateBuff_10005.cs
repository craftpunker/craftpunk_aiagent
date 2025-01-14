

using Battle;

//buff
public class SkillEffect_CreateBuff_10005 : SkillEffectBase
{
    public override void Start()
    {
        base.Start();

        //BuffFactory.instance.CreateBuff(SkillEffectData.buffCfgId, Target);
        Target.BuffBag.PushBuff(SkillEffectData.skillBuffCfgId);

        BKilled = true;
    }

    public override void Release()
    {
        ClassPool.instance.Push(this);
        base.Release();
    }
}