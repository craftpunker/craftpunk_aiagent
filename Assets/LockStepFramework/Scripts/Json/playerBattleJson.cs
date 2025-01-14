
using System;
using System.Collections.Generic;

[Serializable]
public class playerBattleJson
{
    public int cfgid;
    public string name;
    public int winRate; //
    public string icon;
    public int score;
    public List<TroopsData> troops;
}