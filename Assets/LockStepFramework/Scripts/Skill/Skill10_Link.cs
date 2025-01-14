using Battle;
using System.Collections.Generic;
using System.Linq;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

#if _CLIENTLOGIC_
public class LinkGameObj : IRelease
{
    public string ObjPath;
    public EntityBase LastTarget;
    public EntityBase NextTarget;
    public ParticleSystemRenderer ParticleSystem;

    private Vector3 LastPos;
    private Vector3 NextPos;

    public bool Bkill;

    public void Init()
    {
        Bkill = false;
    }

    public void Start()
    {
        LastPos = LastTarget.Fixv3LogicPosition.ToVector3();
        NextPos = NextTarget.Fixv3LogicPosition.ToVector3();

        ResMgr.instance.LoadGameObjectAsync(ObjPath, (go) =>
        {
            ParticleSystem = go.GetComponent<ParticleSystemRenderer>();
            ParticleSystem.lengthScale = 0;
            Update();
        });
    }

    public void Update()
    {
        if (ParticleSystem == null)
            return;


        if (!GameUtils.EntityBeKill(LastTarget))
        {
            LastPos = LastTarget.Fixv3LogicPosition.ToVector3();
        }

        if (!GameUtils.EntityBeKill(NextTarget))
        {
            NextPos = NextTarget.Fixv3LogicPosition.ToVector3();
        }

        ParticleSystem.transform.position = LastPos;
        var forword = NextPos - LastPos;
        ParticleSystem.transform.forward = forword;

        var length = forword.magnitude;
        ParticleSystem.lengthScale = -length;
    }

    public void Release()
    {
        LastTarget = null;
        NextTarget = null;

        ResMgr.instance.ReleaseGameObject(ParticleSystem.gameObject);

        ParticleSystem = null;

        ClassPool.instance.Push(this);
    }
}
#endif

//
public class Skill10_Link : SkillBase
{
    private Fix64 radius;
    private int maxCount;
    private int value4; // 0: 1：
    private PlayerGroup targetGroup; //
    private int skfId1;

    private Fix64 radiusSq;

    private FixVector3 startPos;
    private List<EntityBase> targets = new List<EntityBase>();

#if _CLIENTLOGIC_
    private string objPath;
    private List<LinkGameObj> LinkGameObjs = new List<LinkGameObj>();
#endif

    public override void Start()
    {
        base.Start();
        radius = SkillData.fix64Args[0];
        radiusSq = radius * radius;
        maxCount = (int)SkillData.fix64Args[1];
        value4 = (int)SkillData.fix64Args[2];
        skfId1 = SkillData.skillEffectCfgIds[0];
#if _CLIENTLOGIC_
        objPath = SkillData.stringArgs[0];
#endif

        targetGroup = GameUtils.GetTargetGroup(Origin == null ? PlayerGroup : Origin.PlayerGroup, 
            value4 == 0 ? PlayerGroup.Player : PlayerGroup.Enemy);

        FindTargets();

        for(int i = 0; i < targets.Count; i++)
        {
            SkillEffectFactory.instance.CreateSkillEffect(skfId1, this, Origin, targets[i], i);
        }

        TimeMgr.instance.SetTimeAction((Fix64)0.3, () =>
        {   
            BKilled = true;
        });
    }

    private void FindTargets()
    {
        Fix64 rangeSq = radiusSq;

        List<EntityBase> remainingTargets = new List<EntityBase>(BattleMgr.instance.SoldierList); //
        List<EntityBase> ememys = new List<EntityBase>(); //

        foreach (var item in remainingTargets)
        {
            if (item.PlayerGroup == targetGroup)
            {
                ememys.Add(item);
            }
        }

        targets.Add(Target);

        if (ememys.Count > 0)
        {
            FindCandidates(Target, ememys);
        }

#if _CLIENTLOGIC_
        for (int i = 0; i < targets.Count; i++)
        {
            var item = targets[i];
            var linkObj = ClassPool.instance.Pop<LinkGameObj>();
            linkObj.Init();
            linkObj.ObjPath = objPath;
            if (i == 0)
                linkObj.LastTarget = Origin;
            else
                linkObj.LastTarget = targets[i - 1];

            linkObj.NextTarget = item;

            linkObj.Start();

            LinkGameObjs.Add(linkObj);
        }
#endif
    }

    private void FindCandidates(EntityBase lastEntity, List<EntityBase> ememys)
    {
        if (ememys.Count == 0)
            return;

        List<EntityBase> candidates = ememys
                .Where(target => FixVector3.SqrMagnitude(lastEntity.Fixv3LogicPosition - target.Fixv3LogicPosition) <= radiusSq)
                .ToList();

        if (candidates.Count > 0)
        {
            var index = GameData.instance.sRandom.Range(0, candidates.Count);
            var target = candidates[index];
            targets.Add(target);
            ememys.Remove(target);

            if (targets.Count < maxCount)
            {
                FindCandidates(target, ememys);
            }
        }
    }

    public override void Release()
    {
        targets.Clear();

#if _CLIENTLOGIC_
        for (int i = LinkGameObjs.Count - 1; i >= 0; i--)
        {
            var item = LinkGameObjs[i];
            item.Release();
        }

        LinkGameObjs.Clear();
#endif
        base.Release();
    }
}