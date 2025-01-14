


//

namespace Battle
{

    public class SkillEffect_ImmuneFlyAway_30001 : SkillEffectBase
    {
        //private Fix64 value1;
        public override void Start()
        {
            base.Start();
            //value1 = SkillEffectData.fix64Args[0];
            BattleMgr.instance.AddStage(Target, StageConst.ImmuneFlyAway);
        }

        public override void Release()
        {
            if (Target != null)
                BattleMgr.instance.RemoveStage(Target, StageConst.ImmuneFlyAway);

            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}