using Battle;
using System;
using System.Collections.Generic;

[Serializable]
public class CardData
{
    public int CfgId;
    public string Name;
    public Fix64 Cost;
    public List<int> SoldierJob;
    public int TargetType; //0 1 2 -1
    public Fix64 Range; //-1 
    public int skillCfgId;
}