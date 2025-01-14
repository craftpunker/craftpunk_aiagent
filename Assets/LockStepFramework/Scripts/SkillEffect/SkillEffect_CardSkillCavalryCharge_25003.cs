
using Battle;
using System.Collections.Generic;

public class CavalryChargeData : IRelease
{
    public EntityBase Entity;
    public Fix64 Atk;
    public FixVector3 EndPos;
    public FixVector3 Forward;
    public FixVector3 StartPos;
    public Dictionary<EntityBase, int> HitEneitys = new Dictionary<EntityBase, int>(); //，ID entity:cfgid
    public List<Agent> Agents = new List<Agent>();
    public Fix64 AtkRadiusSq;
    public PlayerGroup TargetGroup;
    public Fix64 AddSpeed;

    public Fix64 Time;
    public Fix64 MaxTime;

    //public SpecialEffect SpecialEffect;

    public void Init()
    {
        Time = Fix64.Zero;
        MaxTime = Fix64.Zero;
    }

    private void ToSearchEnemyFsm(EntityBase entity)
    {
        if (entity == null)
        {
            return;
        }

        if (entity.SoldierFlag != null)
        {
            entity.SoldierFlag.Pos = GameUtils.FindNearestGridY(entity.Fixv3LogicPosition);
            entity.SoldierFlag = null;
        }

        if (entity.Agent != null)
        {
            Simulator.instance.setAgentPosition(entity.Agent.id_, entity.Fixv3LogicPosition);
            Simulator.instance.setAgentPrefVelocity(entity.Agent.id_, FixVector2.Zero);
        }
        entity.TargetPos = FixVector2.None;

        BattleMgr.instance.ToSearchEnemyFsm(entity);
    }

    public void Release()
    {
        BattleMgr.instance.ChangeMoveSpeed(Entity, -AddSpeed);
        ToSearchEnemyFsm(Entity);
        HitEneitys.Clear();
        Agents.Clear();
        Entity = null;

        //if (SpecialEffect != null)
        //{
        //    SpecialEffect.Release();
        //}

        ClassPool.instance.Push(this);
    }
}

public class SkillEffect_CardSkillCavalryCharge_25003 : SkillEffectBase
{
    private Fix64 value1; //
    private Fix64 value2; //
    private SoldierJob job;
    private Fix64 value3; //
    private Fix64 value4; //
    private Fix64 atkRadiusSq; //

    private FixVector3 centerPos;

    private List<CavalryChargeData> cavalryChargeDatas = new List<CavalryChargeData>();

    public override void Init()
    {
        base.Init();
        cavalryChargeDatas.Clear();
    }

    public override void Start()
    {
        base.Start();

        value1 = SkillEffectData.fix64Args[0];
        value2 = SkillEffectData.fix64Args[1];
        job = (SoldierJob)(int)SkillEffectData.fix64Args[2];
        value3 = SkillEffectData.fix64Args[3];
        value4 = SkillEffectData.fix64Args[4];
        atkRadiusSq = value4 * value4;
        centerPos = SkillBase.Fixv3LogicPosition;
        foreach (var item in BattleMgr.instance.SoldierList)
        {
            if (item.Job == job && item.PlayerGroup == SkillBase.PlayerGroup)
            {
                var data = ClassPool.instance.Pop<CavalryChargeData>();
                data.Init();
                data.Entity = item;
                data.Atk = item.Atk * value2;
                //data.Atk = (Fix64)99999999;
                data.EndPos = GameUtils.CheckMapBorder(GetEndPos()).ToFixVector3();
                data.Forward = data.EndPos - item.Fixv3LogicPosition;
                data.StartPos = item.Fixv3LogicPosition;
                data.AtkRadiusSq = atkRadiusSq;
                data.TargetGroup = GameUtils.GetTargetGroup(item.PlayerGroup, PlayerGroup.Enemy);
                data.AddSpeed = item.MoveSpeed * value1;
                BattleMgr.instance.ChangeMoveSpeed(item, data.AddSpeed);
                data.MaxTime = FixVector3.Magnitude(data.Forward) / item.CurrAttrValue.MoveSpeed;
                if (item.SoldierFlag != null)
                {
                    item.SoldierFlag.IsCtorEntitys = false; //
                }
                //data.SpecialEffect = SpecialEffectFactory.instance.CreateSpecialEffect(30022, item.Fixv3LogicPosition, (go) =>
                //{
                //    go.transform.Find("GPUAnim").localScale = new UnityEngine.Vector3((float)value4 * 2, (float)value4 * 2, 0);
                //});
                BattleMgr.instance.ToSkefCtorFsm(item, this);
#if _CLIENTLOGIC_
                item.DoAnim(AnimType.Move);
#endif
                cavalryChargeDatas.Add(data);
            }
        }
    }

    public override void Update()
    {
        base.Update();

        DoCharge();
    }

    private void DoCharge()
    {
        for (int i = cavalryChargeDatas.Count - 1; i >= 0; i--)
        {
            var item = cavalryChargeDatas[i];

            item.Time += GameData.instance._FixFrameLen;
            var t = item.Time / item.MaxTime;

            item.Entity.Fixv3LogicPosition = item.StartPos + MathUtils.MoveStraight(t, item.Forward);
            BattleMgr.instance.FaceMovePos(item.Entity);

            DoHit(item);

            //if (item.SpecialEffect.Trans != null)
            //{
            //    item.SpecialEffect.Trans.position = item.Entity.Fixv3LogicPosition.ToVector3();
            //}

            if (t >= Fix64.One)
            {
                item.Release();
                cavalryChargeDatas.Remove(item);
            }
        }

        if (cavalryChargeDatas.Count == 0)
        {
            BKilled = true;
        }
    }

    private void DoHit(CavalryChargeData data)
    {
        var entity = data.Entity;
        var agent = entity.Agent;
        var rangesq = data.AtkRadiusSq;

        //Simulator.instance.kdTree_.computeAgentNeighbors(agent, ref rangesq, true);
        data.Agents.Clear();
        Simulator.instance.kdTree_.computeAgentNeighborsByPos(entity.Fixv3LogicPosition.ToFixVector2(), ref rangesq, data.TargetGroup, ref data.Agents, -1);
        foreach (var nb in data.Agents)
        {
            var targetEntity = nb.entity;
            if (entity.PlayerGroup != targetEntity.PlayerGroup)
            {
                if (!data.HitEneitys.ContainsKey(targetEntity))
                {
                    BattleMgr.instance.Atk(data.Atk, entity, targetEntity);
                    data.HitEneitys.Add(targetEntity, targetEntity.CfgId);
                }
            }
        }
    }

    private FixVector2 GetEndPos()
    {
        var ranX = GameData.instance.sRandom.Range(centerPos.x - value3, centerPos.x + value3);
        var ranY = GameData.instance.sRandom.Range(centerPos.y - value3, centerPos.y + value3);

        return new FixVector2(ranX, ranY);
    }

    public override void Release()
    {
        for (int i = cavalryChargeDatas.Count - 1; i >= 0; i--)
        {
            var item = cavalryChargeDatas[i];
            item.Release();
        }

        cavalryChargeDatas.Clear();
        base.Release();
    }
}