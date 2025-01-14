

using Battle;


//
public class SkillEffect_Summon_23001 : SkillEffectBase
{
    private int count;
    private Fix64 distance;

    public override void Start()
    {
        base.Start();

        count = (int)SkillEffectData.fix64Args[0];
        distance = SkillEffectData.fix64Args[1];

        FixVector3 pos = FixVector3.Zero;

        if (Origin == null)
            pos = SkillBase.Fixv3LogicPosition;
        else
        {
            EntityBase target = null;
            if (Origin.LockEntity == null)
            {
                target = Origin;
            }
            else
            {
                target = Origin.LockEntity;
            }

            pos = Origin.Fixv3LogicPosition + (target.Fixv3LogicPosition - Origin.Fixv3LogicPosition).GetNormalized() * distance;
        }         

        var entityIndex = SkillEffectData.entityIndex;
        if (GameData.instance.SummonDict.TryGetValue(entityIndex, out TroopsData troopData))
        {
            var entitys = BattleMgr.instance.CreateTroopEntitys(troopData, SkillBase.PlayerGroup, pos, true, true, null);
            foreach (var entity in entitys)
            {
                entity.StartLogic();
            }
        }

#if _CLIENTLOGIC_
        //0:
        SpecialEffectFactory.instance.CreateSpecialEffect(SkillBase.SkillData.animCfgIds, 0, pos);
#endif

        BKilled = true;
    }

    public override void Release()
    {
        ClassPool.instance.Push(this);
        base.Release();
    }
}