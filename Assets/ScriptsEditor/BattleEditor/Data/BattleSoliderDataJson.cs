
using Battle;
using System;
using UnityEngine;

namespace BattleEditor
{
    [System.Serializable]
    public class BattleSoliderDataJson
    {
        public int cfgid;
        public string name;
        public string model;
        public int level;
        public int radius;
        public int moveSpeed;
        public int maxHp;
        public int atk;
        public int count;
        public int atkSpeed;
        public int atkRange;
        public int inAtkRange;
        public int soldierType;
        public string atkSkillShowPos;
        public int atkSkillId;
        public string center;
        public int atkReadyTime;
        public int deadSkillId;
        public int bornSkillId;
        public string pos;


        public BattleSoliderDataJson(SoliderPrefab prefab)
        {
            cfgid = prefab.cfgId;
            name = prefab.name;
            model = prefab.model;
            level = prefab.level;
            radius = prefab.radius;
            moveSpeed = prefab.moveSpeed;
            maxHp = prefab.maxHp;
            atk = prefab.atk;
            count = prefab.count;
            atkSpeed = prefab.atkSpeed;
            atkRange = prefab.atkRange;
            inAtkRange = prefab.inAtkRange;
            soldierType = prefab.soldierType;
            atkSkillShowPos = EditorCommon.Vector2ToFixVector2String(prefab.atkSkillShowPos);
            atkSkillId = prefab.atkSkillId;
            center = EditorCommon.Vector2ToFixVector2String(prefab.center);
            atkReadyTime = prefab.atkReadyTime;
            deadSkillId = prefab.deadSkillId;
            bornSkillId = prefab.bornSkillId;
            pos = EditorCommon.Vector2ToFixVector2String(prefab.pos);
        }
    }
}

