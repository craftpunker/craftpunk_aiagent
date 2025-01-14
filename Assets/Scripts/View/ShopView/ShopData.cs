using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShopData
{
    public static Dictionary<string, float> DictLessTimeRecord = new Dictionary<string, float>();

    public static void Init() {
        EventDispatcher<CmdEventData>.instance.AddEvent("rhg_record", OnEventRecord);
    
    }

    public static int GetTreasureBoughtCount(string cfgId) { 
        int count = 0;
        JSONNode shop_boxc = GameData.instance.DailyData["shop_boxc"];
        if (shop_boxc != null && shop_boxc[cfgId] != null)
        {
            count = shop_boxc[cfgId];
        }
        return count;
    }

    public static void C2S_SHOP_REFRESH_CARD()
    {
        JSONObject jsObj = new JSONObject();
        jsObj.Add("cmd", "C2S_SHOP_REFRESH_CARD");
        JSONObject data = new JSONObject();
        jsObj.Add("data", data);

        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public static void C2S_SHOP_BUY_CARD(int index)
    {
        JSONObject jsObj = new JSONObject();
        jsObj.Add("cmd", "C2S_SHOP_BUY_CARD");
        JSONObject data = new JSONObject();
        data.Add("index", index);
        jsObj.Add("data", data);

        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public static void C2S_SHOP_BUY_BOX(string cfgId)
    {
        JSONObject jsObj = new JSONObject();
        jsObj.Add("cmd", "C2S_SHOP_BUY_BOX");
        JSONObject data = new JSONObject();
        data.Add("cfgId", cfgId);
        jsObj.Add("data", data);

        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public static void C2S_SHOP_BUY_GOLD(string cfgId)
    {
        JSONObject jsObj = new JSONObject();
        jsObj.Add("cmd", "C2S_SHOP_BUY_GOLD");
        JSONObject data = new JSONObject();
        data.Add("cfgId", cfgId);
        jsObj.Add("data", data);

        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }
    

    public static void OnEventRecord(string evtName, CmdEventData[] data) {
        CmdEventData subData = data[0];
        JSONNode jsonData = subData.JsonNode.Value;

        if (subData.EventAction == CmdEventAction.Update)
        {
            foreach (var item in jsonData)
            {
                float lessEnd = item.Value["less"] + Time.time;
                if (!DictLessTimeRecord.TryAdd(item.Key, lessEnd)) {
                    DictLessTimeRecord[item.Key] = lessEnd;
                }
            }
        }
        else if (subData.EventAction == CmdEventAction.None) {
            if (jsonData["data"] != null && jsonData["data"]["permanent"] != null && jsonData["data"]["permanent"]["rhg_record"] != null) {
                foreach (var item in jsonData["data"]["permanent"]["rhg_record"])
                {
                    float lessEnd = item.Value["less"] + Time.time;
                    if (!DictLessTimeRecord.TryAdd(item.Key, lessEnd))
                    {
                        DictLessTimeRecord[item.Key] = lessEnd;
                    }
                }
            }
        }

        //foreach (var item in DictLessTimeRecord)
        //{
        //    Debug.Log($"{item.Key}   {item.Value}");
        //}
    }


    public static void GetRecord(string cfgId) {

        //Debug.Log(GameData.instance.UserData);
        //http://xxx.xxx.xxx.xxx:8002/api/recharge/ready?uid=10901&cfgId=1001&payChannel=local&payType=local
        string url = $"{AppConfig.URL}/api/recharge/ready?uid={GameData.instance.UserData["uid"]}&cfgId={cfgId}&payChannel=local&payType=local";
        Debug.Log(url);
        WebSocketMain.instance.StartCoroutine(IEGetRecord(url));
    }

    static IEnumerator IEGetRecord(string url)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "Get"))
        {
            //var account2SessionJsonnode = GetAccount2SessionData(account);
            //byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(account2SessionJsonnode.ToString());
            //webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postBytes);

            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                UiMgr.ShowTips("network error");

                yield break;
                //WebSocketMain.instance.StopCoroutine(IEGetRecord(url));
                //Debug.Log(webRequest.error);
            }
            else
            {
                JSONNode data = JSONNode.Parse(webRequest.downloadHandler.text);
                JSONNode order = data["data"]["order"];
                if (order["payChannel"] == "local" && order["payType"] == "local") {
                    GetRechargeSettle(order);
                }
            }
        }
    }

    static void GetRechargeSettle(JSONNode orderData) {
        string orderId = ((string)orderData["orderId"]).Replace("\'", "");
        string url = $"{AppConfig.URL}/api/recharge/settle?uid={GameData.instance.UserData["uid"]}&orderId={orderId}&receiptData=&signtureData=&signture=";
        WebSocketMain.instance.StartCoroutine(IEGetRechargeSettle(url));
    }


    static IEnumerator IEGetRechargeSettle(string url)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "Get"))
        {
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                UiMgr.ShowTips("network error");
                yield break;
                //WebSocketMain.instance.StopCoroutine(IEGetRechargeSettle(url));
                //Debug.Log(webRequest.error);
            }
            else
            {
                JSONNode data = JSONNode.Parse(webRequest.downloadHandler.text);

                if (data["msg"] == "success") {
                    //UiMgr.ShowTips("success");

                }
            }
        }
    }
}
