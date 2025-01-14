using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleEditor
{
    public class PlayerData
    {
        public int cfgid;
        public string name;
        public int winRate;
        public string icon;
        public int score;
        public List<BattleSoliderDataJson> troops;
    }
}

