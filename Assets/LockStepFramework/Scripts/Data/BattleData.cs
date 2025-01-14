

using Battle;
using System.Collections.Generic;

public class BattleData
{
    public int uid;
    public string name;
    public float winRate;
    public string avatar;
    public int score;
    public Fix64 costInit;
    public Fix64 costMax;
    public Fix64 costRecover;
    public Fix64 maxTick;
    public MapData mapData;
    public SortedDictionary<int, CardData> cards = new SortedDictionary<int, CardData>();
    public SortedDictionary<int, TroopsData> troops = new SortedDictionary<int, TroopsData>();
    public SortedDictionary<int, TroopsData> summons = new SortedDictionary<int, TroopsData>();

    public void Release()
    {
        foreach (var item in troops)
        {
            ClassPool.instance.Push(item.Value);
        }

        troops.Clear();
        cards.Clear();
    }
}