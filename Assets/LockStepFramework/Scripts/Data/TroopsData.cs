
using Battle;
using System;

[Serializable]
public class TroopsData : ICloneable
{
    public int cfgId;
    public int id;
    public string name;
    public string prefab;
    public int level;
    public Fix64 radius;
    public Fix64 moveSpeed;
    public Fix64 maxHp;
    public Fix64 atk;
    public int count;
    public Fix64 atkSpeed;
    public Fix64 atkRange;
    public Fix64 inAtkRange;
    public int soldierType;
    public FixVector3 atkSkillShowPos;
    public int atkSkillCfgId;
    public FixVector3 center;
    public Fix64 atkReadyTime;
    public Fix64 atkBackswing; //
    public int deadSkillCfgId;
    public int bornSkillCfgId;
    public FixVector3 pos;
    public int animCfgId; //ID
    public Fix64 freeSeekDis; //，
    public int grid;
    public int Job;

    public int SoldierFlagId; //ID

    public object Clone()
    {
        return MemberwiseClone();
    }
}
