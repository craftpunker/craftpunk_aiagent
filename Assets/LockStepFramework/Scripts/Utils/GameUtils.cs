
using Battle;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

#if _CLIENTLOGIC_
using UnityEngine;
#endif

public class GameUtils
{
    //0     1    2    3      4
    //i*2   i*1  i*0  i*-1   i*-2
    //public static FixVector2 GetTroosEntityPos(int num)
    //{
    //    int column = num / (int)GameData.instance.TroosLine;
    //    Fix64 line = (Fix64)num % GameData.instance.TroosLine;

    //    Fix64 x =  GameData.instance.EntityInterval * 2 - column * GameData.instance.EntityInterval;
    //    Fix64 y = GameData.instance.EntityInterval * 2 - line * GameData.instance.EntityInterval;

    //    var pos = new FixVector2(x, y);

    //    return pos;
    //}

    //(Fix64)2 - (Fix64)0.5

    private static int id = 10000;
    public static int GetId()
    {
        return id++;
    }

    //，
    public static FixVector2 GetTroosEntityPos(int num, int totalUnits, PlayerGroup group)
    {
        int totalRows = (int)GameData.instance.TroosLine;
        int totalColumns = (int)Fix64.Ceiling((Fix64)totalUnits / totalRows);

        int row = num % totalRows;
        int column = num / totalRows; 

        int currentColumnUnits = totalRows;
        if (column == totalColumns - 1)
        {
            currentColumnUnits = totalUnits - (totalColumns - 1) * totalRows;
        }

        Fix64 yOffset;
        if (currentColumnUnits % 2 == 0)
        {
            yOffset = (currentColumnUnits / (Fix64)2 - (Fix64)0.5) * GameData.instance.EntityInterval;
        }
        else
        {
            yOffset = (currentColumnUnits / 2) * GameData.instance.EntityInterval;
        }

        Fix64 xOffset = (totalColumns - 1) * GameData.instance.EntityInterval / 2;

        Fix64 x = column * GameData.instance.EntityInterval - xOffset;
        Fix64 y = yOffset - row * GameData.instance.EntityInterval;

        var pos = new FixVector2(group == PlayerGroup.Player ? -x : x, y);
        return pos;
    }

    public static FixVector2 JsonPosToV2(string json)
    {
        var datas = JSONNode.Parse(json);
        var x = new Fix64(datas[0]) / 1000;
        var y = new Fix64(datas[1]) / 1000;
        return new FixVector2(x, y);
    }

    public static FixVector2 JsonPosToV2(JSONNode jsonNode)
    {
        var x = new Fix64(jsonNode[0]) / 1000;
        var y = new Fix64(jsonNode[1]) / 1000;
        return new FixVector2(x, y);
    }

    public static FixVector3 JsonPosToV3(string json)
    {
        var datas = JSONNode.Parse(json);
        var x = new Fix64(datas[0]) / 1000;
        var y = new Fix64(datas[1]) / 1000;
        var z = new Fix64(datas[2]) / 1000;
        return new FixVector3(x, y, z);
    }

    public static FixVector3 JsonPosToV3(JSONNode jsonNode)
    {
        var x = new Fix64(jsonNode[0]) / 1000;
        var y = new Fix64(jsonNode[1]) / 1000;
        var z = new Fix64(jsonNode[2]) / 1000;
        return new FixVector3(x, y, z);
    }

#if _CLIENTLOGIC_
    public static Vector3 JsonPosToUnityV3(string json)
    {
        var datas = JSONNode.Parse(json);
        var x = float.Parse(datas[0]);
        var y = float.Parse(datas[1]);
        var z = float.Parse(datas[2]);
        return new Vector3(x, y, z);
    }

    public static Vector3 JsonPosToUnityV3(JSONNode jsonNode)
    {
        var x = float.Parse(jsonNode[0]);
        var y = float.Parse(jsonNode[1]);
        var z = float.Parse(jsonNode[2]);
        return new Vector3(x, y, z);
    }

    public static Vector2 JsonPosToUnityV2(JSONNode jsonNode)
    {
        var x = float.Parse(jsonNode[0]);
        var y = float.Parse(jsonNode[1]);
        return new Vector2(x, y);
    }
#endif

    public static List<int> JsonToIds(string json)
    {
        List<int> list = new List<int>();

        //
        if (json == null)
            return list;

        var ids = JSONNode.Parse(json);
        for (int i = 0; i < ids.Count; i++)
        {
            list.Add((ids[i]));
        }

        return list;
    }

    public static List<int> JsonToIds(JSONNode jsonNode)
    {
        List<int> list = new List<int>();

        //
        if (jsonNode == null)
            return list;

        //var ids = JSONNode.Parse(json);
        foreach (var item in jsonNode)
        {
            list.Add(item.Value);
        }

        return list;
    }

    public static List<Fix64> JsonToFix64s(string json)
    {
        List<Fix64> args = new List<Fix64>();

        //
        if (json == null)
            return args;

        var values = JSONNode.Parse(json);
        for (int i = 0; i < values.Count; i++)
        {
            args.Add(new Fix64(values[i]) / 1000);
        }

        return args;
    }

    public static List<Fix64> JsonToFix64s(JSONNode jsonNode)
    {
        List<Fix64> args = new List<Fix64>();

        //
        if (jsonNode == null)
            return args;

        //var values = JSONNode.Parse(json);
        foreach (var item in jsonNode)
        {
            args.Add(new Fix64(item.Value.AsInt) / 1000);
        }

        return args;
    }

    public static List<string> JsonToStrs(JSONNode jsonNode)
    {

        List<string> list = new List<string>();

        //
        if (jsonNode == null)
            return list;

        var strs = jsonNode.AsStringList;

        if (strs == null)
            return list;

        for (int i = 0; i < strs.Count; i++)
        {
            list.Add((strs[i]));
        }

        return list;
    }

    public static bool EntityBeKill(EntityBase entity)
    {
        if (entity == null || entity.BKilled)
            return true;

        return false;
    }

    public static bool EntityBeKill(SoldierFlagBase entity)
    {
        if (entity == null || entity.BKilled)
            return true;

        return false;
    }

    public static PlayerGroup GetTargetGroup(PlayerGroup self, PlayerGroup group)
    {
        //
        if (group == PlayerGroup.Player)
        {
            return self;
        }
        else
        {
            if (self == PlayerGroup.Player)
            {
                return PlayerGroup.Enemy;
            }
            else
            {
                return PlayerGroup.Player;
            }
        }
    }

    public static FixVector2 CheckMapBorder(FixVector2 pos)
    {
        FixVector3 mapBorder = GameData.instance.AreaSize;

        if (pos.y > mapBorder.y)
        {
            pos.y = mapBorder.y;
        }
        else if (pos.y < mapBorder.z)
        {
            pos.y = mapBorder.z;
        }

        return pos;
    }

#if _CLIENTLOGIC_
    public static Vector2 CheckMapBorderUnity(Vector2 pos)
    {
        Vector3 mapBorder = GameData.instance.AreaSize.ToVector3();

        if (pos.y > mapBorder.y)
        {
            pos.y = mapBorder.y;
        }
        else if (pos.y < mapBorder.z)
        {
            pos.y = mapBorder.z;
        }

        return pos;
    }
#endif

    //X
    public static FixVector2 CheckMapBorderX(FixVector2 pos)
    {
        FixVector3 mapBorder = GameData.instance.AreaSize;

        if (pos.y > mapBorder.y)
        {
            pos.y = mapBorder.y;
        }
        else if (pos.y < mapBorder.z)
        {
            pos.y = mapBorder.z;
        }

        if (pos.x > -Fix64.One)
        {
            pos.x = -Fix64.One;
        }
        else if (pos.x < -mapBorder.x)
        {
            pos.x = -mapBorder.x;
        }

        return pos;
    }


    //，：player=x  enemy=-x
    public static FixVector3 GetGroupForward(PlayerGroup group, Fix64 offsetX)
    {
        Fix64 borderX = GameData.instance.AreaSize.x;

        if (group == PlayerGroup.Player)
        {
            borderX = borderX - offsetX;
        }
        else
        {
            borderX = -borderX + offsetX;
        }
        FixVector3 border = new FixVector3(borderX, Fix64.Zero, Fix64.Zero);

        return border - FixVector3.Zero;
    }

#if _CLIENTLOGIC_
    //GPU3D
    public static bool PrefabIs3D(AnimData animData)
    {
        if (animData.Prefab.Contains("GPU"))
            return false;
        else
            return true;
    }
#endif

    public static string CalcMd5(string source)
    {
        MD5 md5 = MD5.Create();
        byte[] bytes_src = Encoding.UTF8.GetBytes(source);
        byte[] bytes_md5 = md5.ComputeHash(bytes_src);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bytes_md5.Length; i++)
        {
            sb.Append(bytes_md5[i].ToString("X2"));
            if (i != bytes_md5.Length - 1)
            {
                sb.Append("-");
            }
        }
        return sb.ToString();
    }

    public static string SumBattleValue()
    {
        Fix64 sum = Fix64.Zero;

        for (int i = BattleMgr.instance.SoldierFlagDict.Count - 1; i >= 0; i--)
        {
            var item = BattleMgr.instance.SoldierFlagDict.ElementAt(i).Value;
            sum += item.Pos.x;
            sum += item.Pos.y;
        }

        for (int i = BattleMgr.instance.SoldierList.Count - 1; i >= 0; i--)
        {
            var item = BattleMgr.instance.SoldierList[i];
            sum += item.Fixv3LogicPosition.x;
            sum += item.Fixv3LogicPosition.y;
            sum += item.Hp;
            sum += item.AtkCD;
            sum += item.MoveSpeed;
        }

        for (int i = BattleMgr.instance.SkillList.Count - 1; i >= 0; i--)
        {
            var item = BattleMgr.instance.SkillList[i];
            sum += item.Fixv3LogicPosition.x;
            sum += item.Fixv3LogicPosition.y;
        }

        sum += BattleMgr.instance.CurrCost;

        return sum.ToString();
    }

    public static string StepDetail()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"{GameData.instance._UGameLogicFrame} : ");

        for (int i = BattleMgr.instance.SoldierFlagDict.Count - 1; i >= 0; i--)
        {
            //var item = BattleMgr.instance.SoldierFlagDict.ElementAt(i).Value;
            //sum += item.Pos.x;
            //sum += item.Pos.y;
        }

        for (int i = BattleMgr.instance.SoldierList.Count - 1; i >= 0; i--)
        {
            var item = BattleMgr.instance.SoldierList[i];
            sb.AppendLine($"soldier {item.CfgId}, pos : {item.Fixv3LogicPosition} , hp: {item.Hp}");
            //sum += item.Fixv3LogicPosition.x;
            //sum += item.Fixv3LogicPosition.y;
            //sum += item.Hp;
            //sum += item.AtkCD;
            //sum += item.MoveSpeed;
        }

        for (int i = BattleMgr.instance.SkillList.Count - 1; i >= 0; i--)
        {
            var item = BattleMgr.instance.SkillList[i];
            sb.AppendLine($"skill {item.SkillData.cfgId}, pos : {item.Fixv3LogicPosition}");
            //var item = BattleMgr.instance.SkillList[i];
            //sum += item.Fixv3LogicPosition.x;
            //sum += item.Fixv3LogicPosition.y;
        }

        return sb.ToString();
    }

#if _CLIENTLOGIC_
    //
    private static Vector3 FindNearestPoint(float maxDistance, Vector3 currPos, List<Vector3> points)
    {
        Vector3 nearestPoint = new Vector3();
        float closestDistanceSqr = maxDistance * maxDistance;

        Vector3 currentPosition = currPos;

        foreach (Vector3 point in points)
        {
            float distanceSqr = (point - currentPosition).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                nearestPoint = point;
            }
        }

        return nearestPoint;
    }
    public static Vector3 FindNearestGridPoint(Vector3 dragSoliderhitPos)
    {
        List<Vector3> posList = new List<Vector3>();
        var gridConf = GameData.instance.TableJsonDict["GridConf"];

        foreach (var item in gridConf.Values)
        {
            var pos = GameUtils.JsonPosToV2(item["center"]).ToVector3();
            posList.Add(pos);
        }

        Vector3 nearestPos = FindNearestPoint(1.3f, dragSoliderhitPos, posList);
        return nearestPos;
    }


    public static int FindNearestGrid(Vector3 currPos)
    {
        var maxDistance = 1.3f;
        var gridTable = GameData.instance.TableJsonDict["GridConf"];

        KeyValuePair<string, JSONNode> nearestGrid = new KeyValuePair<string, JSONNode>();
        float closestDistanceSqr = maxDistance * maxDistance;

        Vector2 currentPosition = currPos;

        foreach (var item in gridTable)
        {
            float distanceSqr = (JsonPosToV2(item.Value["center"]).ToVector2() - currentPosition).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                nearestGrid = item;
            }
        }

        if (nearestGrid.Key == null)
        {
            return -1;
        }

        return int.Parse(nearestGrid.Key);

    }

    //
    public static TroopsMono FindNearestTroopsMono(int id, float maxDistance, Vector3 currPos)
    {
        TroopsMono nearestTroopsMono = null;

        float closestDistanceSqr = maxDistance * maxDistance; // 

        Vector3 currentPosition = currPos;
        var troops = GameData.instance.TroopsMonos;

        foreach (var item in troops)
        {
            var troop = item.Value;
            if (troop.Id == id)
                continue;

            float distanceSqr = (troop.transform.position - currentPosition).sqrMagnitude; // 

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                nearestTroopsMono = troop;
            }
        }

        return nearestTroopsMono;
    }

#endif
    //
    public static FixVector3 FindNearestGridY(FixVector3 pos)
    {
        //var table = GameData.instance.TableJsonDict["GlobalConf"];
        var gridYs = GameData.instance.gridYs;

        Fix64 closestDistance = (Fix64)1000;
        Fix64 posY = pos.y;
        FixVector3 currentPosition = pos;

        foreach (var kv in gridYs)
        {
            Fix64 y = (Fix64)(kv / 1000);
            Fix64 distance = Fix64.Abs(y - posY);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentPosition = new FixVector3(pos.x, y, Fix64.Zero);
            }
        }

        return currentPosition;
    }

    //
    public static bool IsGridLock(int gridId)
    {
        var passPve = GameData.instance.PermanentData["pass_pve"];
        var gridTable = GameData.instance.TableJsonDict["GridConf"];
        var gridData = gridTable[gridId.ToString()];
        var unlockLevel = gridData["unlockLevel"];
        if (unlockLevel > passPve)
        {
            return true;
        }

        return false;
    }

    #region ,
    public static void JsonObjectQuickSort(JSONObject list, int left, int right)
    {
        if (left < right)
        {
            int pivotIndex = Partition(list, left, right);
            JsonObjectQuickSort(list, left, pivotIndex - 1);  // 
            JsonObjectQuickSort(list, pivotIndex + 1, right); // 
        }
    }

    // 
    private static int Partition(JSONObject list, int left, int right)
    {
        int pivot = list[right]["timestamp"]; // 
        int i = left - 1;

        for (int j = left; j < right; j++)
        {
            if (list[j]["timestamp"] > pivot)
            {
                i++;
                Swap(list, i, j);
            }
        }

        Swap(list, i + 1, right); // 
        return i + 1; // 
    }

    // 
    private static void Swap(JSONObject list, int a, int b)
    {
        JSONObject temp = list[a].AsObject;
        list[a] = list[b];
        list[b] = temp;

        //Debug.Log(list[a]);
    }
    #endregion

    //
    public static FixVector3 GetAttackDirection(PlayerGroup group)
    {
        if (group == PlayerGroup.Player)
        {
            return GameData.instance._Right;
        }

        return GameData.instance._Right * -Fix64.One;
    }

    //md5
    public static void OutPutMd5File(StringBuilder sb)
    {
#if _CLIENTLOGIC_
        string path = $"C:\\GameMd5\\Client";
#else
        string path = $"C:\\GameMd5\\Check";
#endif

        string filePath = $"{path}\\{DateTime.Now.ToString("yyyy-MM-dd-ss-hh-mm-ss-ffff")}.txt"; // 

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // File.WriteAllText
        File.WriteAllText(filePath, sb.ToString());
    }

    public static void LoadFile(string relativePath)
    {
        string absolutePath = Path.Combine(Environment.CurrentDirectory, relativePath);

        // 
        if (File.Exists(absolutePath))
        {
            // 
            string content = File.ReadAllText(absolutePath);
            Console.WriteLine(content);
        }
        else
        {
            Console.WriteLine("");
        }
    }
}
