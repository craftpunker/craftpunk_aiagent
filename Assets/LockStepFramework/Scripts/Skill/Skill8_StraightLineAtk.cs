using Battle;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

//【skf1】
public class Skill8_StraightLineAtk : SkillBase
{
    private Fix64 speed;
    private FixVector3 forward;
    private Fix64 time;
    private Fix64 maxTime;
    private FixVector3 startPos;
    private int skfId1;

    public override void Start()
    {
        base.Start();

        skfId1 = SkillData.skillEffectCfgIds[0];
        speed = SkillData.fix64Args[0];
        forward = Target.Fixv3LogicPosition - Origin.Fixv3LogicPosition;
        maxTime = FixVector3.Magnitude(forward) / speed;
        time = Fix64.Zero;
        startPos = Fixv3LogicPosition;

#if _CLIENTLOGIC_
        Is3D = GameUtils.PrefabIs3D(AnimData);

        CreateFromPrefab(AnimData.Prefab, (go) =>
        {
            if (!Is3D)
            {
                SetMeshBlock();
                DoAnim(AnimType.Idle);
                BattleMgr.instance.FaceEnemy(Origin, Target);
            }
            else
            {
                Trans.forward = (Target.Fixv3LogicPosition - Origin.Fixv3LogicPosition).ToVector3();
            }
            
            go.SetActive(true);
        });
#endif
    }

    public override void Update()
    {
        base.Update();
        forward = Target.Fixv3LogicPosition - Origin.Fixv3LogicPosition;

#if _CLIENTLOGIC_
        if (Is3D && Trans != null)
        {
            Trans.forward = forward.ToVector3();
        }
#endif

        time += GameData.instance._FixFrameLen;
        var t = time / maxTime;

        var pos = FixMath.MoveStraight(t, forward);

        if (t >= Fix64.One)
        {
            pos = FixMath.MoveStraight(Fix64.One, forward);
            SkillEffectFactory.instance.CreateSkillEffect(skfId1, this, Origin, Target);
            BKilled = true;
            return;
        }

        Fixv3LogicPosition = startPos + pos;
    }

    public override void Release()
    {
        ClassPool.instance.Push(this);
        base.Release();
    }
}