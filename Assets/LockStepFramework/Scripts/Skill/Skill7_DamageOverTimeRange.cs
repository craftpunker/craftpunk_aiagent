using Battle;
using System.Collections.Generic;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

//【skf1】
public class Skill7_DamageOverTimeRange : SkillBase
{
    private int skfId1;

    private Fix64 radius;
    private int count;
    private Fix64 interval;//
    private Fix64 height;
    private int frequency; //

    private PlayerGroup targetGroup;

    private int currFrequency;
    private Fix64 time;

    private bool isWaitRelease;
#if _CLIENTLOGIC_
    private SpecialEffect HeadObj;
#endif


    public override void Start()
    {
        base.Start();

        skfId1 = SkillData.skillEffectCfgIds[0];

        radius = SkillData.fix64Args[0];
        count = (int)SkillData.fix64Args[1];
        interval = SkillData.fix64Args[2];
        height = SkillData.fix64Args[3];
        frequency = (int)SkillData.fix64Args[4];

        currFrequency = 0;
        time = interval;
        isWaitRelease = false;

        targetGroup = GameUtils.GetTargetGroup(PlayerGroup, SkillData.targetGroup);

#if _CLIENTLOGIC_
        //CreateFromPrefab(AnimData.Prefab, (go) =>
        //{
        //    SetMeshBlock();
        //    DoAnim(AnimType.Idle);
        //    Debug.Log(Fixv3LogicPosition);
        //    Trans.position = Fixv3LogicPosition.ToVector3();
        //    go.SetActive(true);
        //});

        HeadObj = SpecialEffectFactory.instance.CreateSpecialEffect(SkillData.animCfgIds, 1, new FixVector3(Fixv3LogicPosition.x, Fixv3LogicPosition.y, -height));
#endif
    }

    public override void Update()
    {
        base.Update();

        if (!isWaitRelease)
        {
            var t = time / interval;
            time += GameData.instance._FixFrameLen;
            if (t >= Fix64.One)
            {
                if (currFrequency >= frequency)
                {
                    isWaitRelease = true;
                    TimeMgr.instance.SetTimeAction(Fix64.One, () =>
                    {
#if _CLIENTLOGIC_
                        HeadObj.BKilled = true;
#endif
                        BKilled = true;
                    });
                    return;
                }

                DoSkill();
                time = Fix64.Zero;
                currFrequency++;

            }
        }
    }

    private void DoSkill()
    {
#if _CLIENTLOGIC_
        SpecialEffectFactory.instance.CreateSpecialEffect(SkillData.animCfgIds, 0, Fixv3LogicPosition);
#endif
        Fix64 rangeSq = radius * radius;
        List<Agent> agents = new List<Agent>();
        Simulator.instance.kdTree_.computeAgentNeighborsByPos(Fixv3LogicPosition.ToFixVector2(), ref rangeSq, targetGroup, ref agents, count);

        foreach (Agent agent in agents)
        {
            SkillEffectFactory.instance.CreateSkillEffect(skfId1, this, Origin, agent.entity);
        }
    }

    public override void Release()
    {
#if _CLIENTLOGIC_
        HeadObj = null;
#endif
        base.Release();
    }
}