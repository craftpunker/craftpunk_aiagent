using Battle;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public static class GuideMgr
{
    public static Dictionary<string, Dictionary<int, JSONNode>> guideCfgMap;
    public static List<GuideNode> guideNodeList;
    public static void Init() {
        guideCfgMap = new Dictionary<string, Dictionary<int, JSONNode>>();
        JSONNode guideCfg = GameData.instance.TableJsonDict["GuideConf"];

        foreach (JSONNode cfg in guideCfg)
        {
            if (!guideCfgMap.TryGetValue(cfg["guideId"], out Dictionary<int, JSONNode> stepMap)) {
                stepMap = new Dictionary<int, JSONNode>();
                guideCfgMap.Add(cfg["guideId"], stepMap);
            }
            stepMap.Add(cfg["step"], cfg);
        }

        guideNodeList = new List<GuideNode>();

        //AddGuide("140001");

        //AddGuide("141001");
        
       //EventDispatcher<string>.instance.AddEvent(EventName.S2C_SYNCH_DATA_END, On_S2C_SYNCH_DATA_END);
       EventDispatcher<CmdEventData>.instance.AddEvent("guide_info", OnGuideInfo);
    }

    //static void On_S2C_SYNCH_DATA_END(string evtName, string[] ints) {

    //    //DebugUtils.Log("dddddddddddddddddddddOn_S2C_SYNCH_DATA_END");
    //    //DebugUtils.Log(GameData.instance.PermanentData["guide_info"]);

    //    //if (GameData.instance.PermanentData["guide_info"] == null)
    //    //{
    //    //    SendGuideInfo(new string[] { "10001" });
    //    //}

    //    //SendGuideInfo(new string[] { "140001" });
    //}

    static void OnGuideInfo(string evtName, CmdEventData[] dataList) {
        JSONNode guideInfo = GameData.instance.PermanentData["guide_info"];
        JSONNode guideData = JSONNode.Parse(guideInfo);

        Debug.Log(guideData);

        List<GuideNode> newGuideNodeList = new List<GuideNode>();

        if (guideNodeList.Count > 0)
        {
            for (int i = guideNodeList.Count - 1; i >= 0; i--)
            {
                GuideNode item = guideNodeList[i];
                if (item.cfg["isExtra"] == 1)
                {
                    guideNodeList.RemoveAt(i);
                    newGuideNodeList.Add(item);
                }
            }

            foreach (string guideCfgId in guideData["guides"].Values)
            {
                bool isAdd = false;
                for (int i = guideNodeList.Count - 1; i >= 0; i--)
                {
                    GuideNode item = guideNodeList[i];
                    if (item.cfg["cfgId"].ToString() == guideCfgId)
                    {
                        guideNodeList.RemoveAt(i);
                        newGuideNodeList.Add(item);
                        isAdd = true;
                        break;
                    }
                }
                if (!isAdd) {

                    newGuideNodeList.Add(new GuideNode(guideCfgId));
                }
            }

            GuideView guideView = UiMgr.GetView<GuideView>();
            if (guideView != null)
            {
                foreach (var item in guideNodeList)
                {
                    if (guideView.guideNode == item)
                    {
                        UiMgr.Close<GuideView>();
                        item.stage = GuideNode.Stage.Finish;
                    }
                }
            }
        }
        else {
            foreach (string guideCfgId in guideData["guides"].Values)
            {
                newGuideNodeList.Add(new GuideNode(guideCfgId));
            }
        }

        guideNodeList.Clear();
        guideNodeList = newGuideNodeList;
    }

    public static void SendGuideInfo(string[] guides) {
        JSONObject guidesJson = new JSONObject();
        guidesJson.Add("guides", guides);

        JSONObject jsObj = new JSONObject();
        jsObj.Add("cmd", "C2S_SET_GUIDE_INFO");
        JSONObject data = new JSONObject();

        //data.Add("text", guidesJson);
        data.Add("text", guidesJson.ToString());
        jsObj.Add("data", data);

        WebSocketMain.instance.SendWebSocketMessage(jsObj);

        //DebugUtils.Log(jsObj);
    }

    public static void Update() {

        if (guideNodeList.Count > 0) {
            for (int i = guideNodeList.Count - 1; i >= 0; i--)
            {
                guideNodeList[i].Update();
            }
        }
    }

    public static void Clear() {
        guideNodeList.Clear();
    }

    public static void RemoveGuide(GuideNode guideNode) {
        if (guideNodeList.Contains(guideNode)) {
            guideNodeList.Remove(guideNode);
        }
    }

    public static void AddGuide(string cfgId)
    {
        guideNodeList.Add(new GuideNode(cfgId));
    }

    public static void SaveGuideData() {
        List<string> guides = new List<string>();
        foreach (var guideNode in guideNodeList)
        {
            if (guideNode.cfg["isExtra"] == 0) {
                guides.Add(guideNode.cfg["guideId"]);
            }
        }
        SendGuideInfo(guides.ToArray());
    }
}
