
using Battle;
using System;

namespace BattleEditor
{
    public class BattleSoliderData
    {
        public int cfgId;
        public string name;
        public string model;
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
        public FixVector2 atkSkillShowPos;
        public int atkSkillId;
        public FixVector2 center;
        public int atkReadyTime;
        public int deadSkillId;
        public int bornSkillId;
        public FixVector2 pos;
    }

}
