
using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum CmdEventAction
{
    None = 0,
    Update = 1,
    Delete = 2
}

public class CmdEventData
{
    public string Name;
    public CmdEventAction EventAction;
    public KeyValuePair<string, JSONNode> JsonNode;
}

public class Cmd : Singleton<Cmd>
{
    public Dictionary<string, Action<JSONObject>> C2SCmdDict = new Dictionary<string, Action<JSONObject>>();
    public Dictionary<string, Action<JSONNode>> S2CCmdDict = new Dictionary<string, Action<JSONNode>>();

    private Queue<CmdEventData> cmdEventDataQueue = new Queue<CmdEventData>();

    public void Init()
    {
        //-----------------------C2S-----------------------------

        C2SCmdDict.Add("C2S_USER_LOGIN", C2S_USER_LOGIN);
        C2SCmdDict.Add("C2S_USER_PING", C2S_USER_PING);
        C2SCmdDict.Add("C2S_LOAD_DATA", C2S_LOAD_DATA);
        C2SCmdDict.Add("C2S_CREATE_TROOP", C2S_CREATE_TROOP);
        C2SCmdDict.Add("C2S_PUT_TROOP_ON", C2S_PUT_TROOP_ON);
        C2SCmdDict.Add("C2S_PUT_TROOP_OFF", C2S_PUT_TROOP_OFF);
        C2SCmdDict.Add("C2S_ADJUST_TROOP", C2S_ADJUST_TROOP);
        C2SCmdDict.Add("C2S_UP_TROOP_LEVEL", C2S_UP_TROOP_LEVEL);
        C2SCmdDict.Add("C2S_CHALLENGE_PVE", C2S_CHALLENGE_PVE);
        C2SCmdDict.Add("C2S_BATTLE_END", C2S_BATTLE_END);
        C2SCmdDict.Add("C2S_CHALLENGE_PVP", C2S_CHALLENGE_PVP);
        C2SCmdDict.Add("C2S_CHOOSE_DEPLOY", C2S_CHOOSE_DEPLOY);
        C2SCmdDict.Add("C2S_PVP_REFRESH_OPPONENT", C2S_PVP_REFRESH_OPPONENT);
        C2SCmdDict.Add("C2S_PVP_RANK", C2S_PVP_RANK);
        C2SCmdDict.Add("C2S_PVP_RECORD_READ", C2S_PVP_RECORD_READ);
        C2SCmdDict.Add("C2S_PVP_REVENGE", C2S_PVP_REVENGE);

        C2SCmdDict.Add("C2S_READ_ONE_MAIL", C2S_READ_ONE_MAIL);
        C2SCmdDict.Add("C2S_DRAW_ONE_MAIL", C2S_DRAW_ONE_MAIL);
        C2SCmdDict.Add("C2S_DELETE_ONE_MAIL", C2S_DELETE_ONE_MAIL);
        C2SCmdDict.Add("C2S_READ_DRAW_ALL_MAIL", C2S_READ_DRAW_ALL_MAIL);
        C2SCmdDict.Add("C2S_DELETE_ALL_MAIL", C2S_DELETE_ALL_MAIL);

        C2SCmdDict.Add("C2S_CARD_ACTIVE", C2S_CARD_ACTIVE);
        C2SCmdDict.Add("C2S_CARD_LEVEL_UP", C2S_CARD_LEVEL_UP);
        C2SCmdDict.Add("C2S_CARD_PUT_ON", C2S_CARD_PUT_ON);
        C2SCmdDict.Add("C2S_CARD_PUT_OFF", C2S_CARD_PUT_OFF);
        

        //C2SCmdDict.Add("C2S_FINISH_DAILY_TASK", C2S_FINISH_DAILY_TASK);
        //C2SCmdDict.Add("C2S_GET_DAILY_ACT_REWARD", C2S_GET_DAILY_ACT_REWARD);


        //C2SCmdDict.Add("C2S_FINISH_WEEKLY_TASK", C2S_FINISH_WEEKLY_TASK);
        //C2SCmdDict.Add("C2S_GET_WEEKLY_ACT_REWARD", C2S_GET_WEEKLY_ACT_REWARD);


        //------------------------S2C----------------------------

        S2CCmdDict.Add("S2C_USER_LOGIN", S2C_USER_LOGIN);
        S2CCmdDict.Add("S2C_USER_PONG", S2C_USER_PONG);
        S2CCmdDict.Add("S2C_SYNCH_DATA_BEGIN", S2C_SYNCH_DATA_BEGIN);
        S2CCmdDict.Add("S2C_USER_DATA", S2C_USER_DATA);
        S2CCmdDict.Add("S2C_SYNCH_DATA_END", S2C_SYNCH_DATA_END);
        S2CCmdDict.Add("S2C_USER_BATTLE_INFO", S2C_USER_BATTLE_INFO);
        S2CCmdDict.Add("S2C_BATTLE_BEGIN", S2C_BATTLE_BEGIN);
        S2CCmdDict.Add("S2C_BATTLE_RESULT", S2C_BATTLE_RESULT);

        S2CCmdDict.Add("S2C_MAIL_DATA", S2C_MAIL_DATA);
        S2CCmdDict.Add("S2C_MAIL_NEW", S2C_MAIL_NEW);
        S2CCmdDict.Add("S2C_MAIL_UPDATE", S2C_MAIL_UPDATE);
        S2CCmdDict.Add("S2C_MAIL_DELETE", S2C_MAIL_DELETE);
        S2CCmdDict.Add("S2C_MAIL_REWARD", S2C_MAIL_REWARD);
        S2CCmdDict.Add("S2C_USER_KICK", S2C_USER_KICK);
        

        S2CCmdDict.Add("S2C_PVP_RANK", S2C_PVP_RANK);
        S2CCmdDict.Add("S2C_PVP_RECORD_DATA", S2C_PVP_RECORD_DATA);
        S2CCmdDict.Add("S2C_PVP_RECORD_NEW", S2C_PVP_RECORD_NEW);
        S2CCmdDict.Add("S2C_PVP_RECORD_READ", S2C_PVP_RECORD_READ);
        S2CCmdDict.Add("S2C_PVP_REVENGE", S2C_PVP_REVENGE);

        S2CCmdDict.Add("S2C_FINISH_DAILY_TASK", S2C_FINISH_DAILY_TASK);
        S2CCmdDict.Add("S2C_FINISH_WEEKLY_TASK", S2C_FINISH_WEEKLY_TASK);
        S2CCmdDict.Add("S2C_GET_DAILY_ACT_REWARD", S2C_GET_DAILY_ACT_REWARD);
        S2CCmdDict.Add("S2C_GET_WEEKLY_ACT_REWARD", S2C_GET_WEEKLY_ACT_REWARD);
        S2CCmdDict.Add("S2C_FINISH_ACHIEVE_TASK", S2C_FINISH_ACHIEVE_TASK);
        

        S2CCmdDict.Add("S2C_RECHARGE_FINISH", S2C_RECHARGE_FINISH);
        S2CCmdDict.Add("S2C_SHOP_BUY_CARD", S2C_SHOP_BUY_CARD);
        S2CCmdDict.Add("S2C_SHOP_BUY_BOX", S2C_SHOP_BUY_BOX);
        S2CCmdDict.Add("S2C_SHOP_BUY_GOLD", S2C_SHOP_BUY_GOLD);

        S2CCmdDict.Add("S2C_GET_POWER_REWARD", S2C_GET_POWER_REWARD);

        S2CCmdDict.Add("S2C_GET_TIME_REWARD", S2C_GET_TIME_REWARD);

        S2CCmdDict.Add("S2C_CARD_ACTIVE", S2C_CARD_ACTIVE);
        S2CCmdDict.Add("S2C_CARD_LEVEL_UP", S2C_CARD_LEVEL_UP);

        //------------------------S2C_Data----------------------------

        S2CCmdDict.Add("S2C_PERMANENT_DATA", S2C_PERMANENT_DATA);
        S2CCmdDict.Add("S2C_PERMANENT_DATA_UPDATE", S2C_PERMANENT_DATA_UPDATE);
        S2CCmdDict.Add("S2C_PERMANENT_DATA_DELETE", S2C_PERMANENT_DATA_DELETE);
        S2CCmdDict.Add("S2C_PERMANENT_DATA_UPDATE1", S2C_PERMANENT_DATA_UPDATE1);
        S2CCmdDict.Add("S2C_PERMANENT_DATA_DELETE1", S2C_PERMANENT_DATA_DELETE1);
        S2CCmdDict.Add("S2C_PERMANENT_DATA_UPDATE2", S2C_PERMANENT_DATA_UPDATE2);
        S2CCmdDict.Add("S2C_PERMANENT_DATA_DELETE2", S2C_PERMANENT_DATA_DELETE2);
        S2CCmdDict.Add("S2C_DAILY_DATA", S2C_DAILY_DATA);
        S2CCmdDict.Add("S2C_DAILY_DATA_UPDATE", S2C_DAILY_DATA_UPDATE);
        S2CCmdDict.Add("S2C_DAILY_DATA_DELETE", S2C_DAILY_DATA_DELETE);
        S2CCmdDict.Add("S2C_DAILY_DATA_UPDATE1", S2C_DAILY_DATA_UPDATE1);
        S2CCmdDict.Add("S2C_DAILY_DATA_DELETE1", S2C_DAILY_DATA_DELETE1);
        S2CCmdDict.Add("S2C_DAILY_DATA_UPDATE2", S2C_DAILY_DATA_UPDATE2);
        S2CCmdDict.Add("S2C_DAILY_DATA_DELETE2", S2C_DAILY_DATA_DELETE2);
        S2CCmdDict.Add("S2C_DAILY_DATA_CLEAR", S2C_DAILY_DATA_CLEAR);
        S2CCmdDict.Add("S2C_WEEKLY_DATA", S2C_WEEKLY_DATA);
        S2CCmdDict.Add("S2C_WEEKLY_DATA_UPDATE", S2C_WEEKLY_DATA_UPDATE);
        S2CCmdDict.Add("S2C_WEEKLY_DATA_DELETE", S2C_WEEKLY_DATA_DELETE);
        S2CCmdDict.Add("S2C_WEEKLY_DATA_UPDATE1", S2C_WEEKLY_DATA_UPDATE1);
        S2CCmdDict.Add("S2C_WEEKLY_DATA_DELETE1", S2C_WEEKLY_DATA_DELETE1);
        S2CCmdDict.Add("S2C_WEEKLY_DATA_UPDATE2", S2C_WEEKLY_DATA_UPDATE2);
        S2CCmdDict.Add("S2C_WEEKLY_DATA_DELETE2", S2C_WEEKLY_DATA_DELETE2);
        S2CCmdDict.Add("S2C_WEEKLY_DATA_CLEAR", S2C_WEEKLY_DATA_CLEAR);

        
        

        //------------------------S2C_Logic----------------------------
        S2CCmdDict.Add("S2C_SHOW_TIP", S2C_SHOW_TIP);
        S2CCmdDict.Add("S2C_GM_OPERATE", S2C_GM_OPERATE);
    }

    public void Update()
    {
        if(cmdEventDataQueue.Count > 0)
        {
            var cmdEventData = cmdEventDataQueue.Dequeue();
            EventDispatcher<CmdEventData>.instance.TriggerEvent(cmdEventData.Name, cmdEventData);
        }
    }

    private void SendCmdEvent(string name, KeyValuePair<string, JSONNode> kv, CmdEventAction cmdEventAction = CmdEventAction.None)
    {
        CmdEventData cmdEventData = ClassPool.instance.Pop<CmdEventData>();
        cmdEventData.Name = name;
        cmdEventData.EventAction = cmdEventAction;
        cmdEventData.JsonNode = kv;
        cmdEventDataQueue.Enqueue(cmdEventData);
    }

    private void SendCmdEvent(string name, JSONNode jsonNode, CmdEventAction cmdEventAction = CmdEventAction.None)
    {
        KeyValuePair<string, JSONNode> kv = new KeyValuePair<string, JSONNode>(name, jsonNode);
        SendCmdEvent(name, kv, cmdEventAction);
    }

    //---------------C2S--------------------------------------------------------------------------------

    //,,websocket。。
    public void C2S_USER_LOGIN(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_USER_LOGIN");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    //,。。
    public void C2S_LOAD_DATA(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_LOAD_DATA");
        jsObj.Add("data", new JSONObject());
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    //,5。
    private void SendPing(JSONObject jsonObj)
    {
        MonoTimeMgr.instance.SetTimeAction(5, () =>
        {
            C2S_USER_PING(jsonObj);
            SendPing(jsonObj);
        });
    }

    private void C2S_USER_PING(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_USER_PING");
        jsObj.Add("data", new JSONObject());
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    // {"cmd":"C2S_CREATE_TROOP","data":{"cfgId":10101}}
    //datakeypermanent.troop_info。
    public void C2S_CREATE_TROOP(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_CREATE_TROOP");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    //
    //{"cmd":"C2S_PUT_TROOP_ON","data":{"pos":[100,200],"id":1}}
    //--id id
    //--pos 
    //datakeypermanent.battle_troop。
    public void C2S_PUT_TROOP_ON(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_PUT_TROOP_ON");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    //
    //{"cmd": "C2S_PUT_TROOP_OFF","data": {"id":1}}
    //--id id
    //datakeypermanent.battle_troop。
    public void C2S_PUT_TROOP_OFF(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_PUT_TROOP_OFF");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }
    
    //
    //{"cmd": "C2S_ADJUST_TROOP","data": {"id":1,"pos":[1000, 2000]
    //}}
    //--id id
    //--pos 
    public void C2S_ADJUST_TROOP(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_ADJUST_TROOP");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    //
    //{"cmd": "C2S_UP_TROOP_LEVEL","data": {"id":1}}
    //--id id
    //datakeypermanent.battle_troop。
    public void C2S_UP_TROOP_LEVEL(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_UP_TROOP_LEVEL");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    //
    //{"cmd": "C2S_GET_TIME_REWARD","data": {}}
    //--id id
    //datakeypermanent.time_chest 。
    public void C2S_GET_TIME_REWARD(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_GET_TIME_REWARD");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_CHALLENGE_PVE(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_CHALLENGE_PVE");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_BATTLE_END(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_BATTLE_END");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_CHALLENGE_PVP(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_CHALLENGE_PVP");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_CHOOSE_DEPLOY(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_CHOOSE_DEPLOY");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_READ_ONE_MAIL(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_READ_ONE_MAIL");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_DRAW_ONE_MAIL(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_DRAW_ONE_MAIL");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_DELETE_ONE_MAIL(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_DELETE_ONE_MAIL");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_READ_DRAW_ALL_MAIL(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_READ_DRAW_ALL_MAIL");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_DELETE_ALL_MAIL(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_DELETE_ALL_MAIL");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_FINISH_DAILY_TASK(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_FINISH_DAILY_TASK");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_GET_DAILY_ACT_REWARD(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_GET_DAILY_ACT_REWARD");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_FINISH_WEEKLY_TASK(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_FINISH_WEEKLY_TASK");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }


    public void C2S_GET_WEEKLY_ACT_REWARD(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_GET_WEEKLY_ACT_REWARD");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    } 

    public void C2S_PVP_REFRESH_OPPONENT(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_PVP_REFRESH_OPPONENT");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_PVP_RANK(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_PVP_RANK");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_PVP_RECORD_READ(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_PVP_RECORD_READ");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_PVP_REVENGE(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_PVP_REVENGE");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_CARD_ACTIVE(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_CARD_ACTIVE");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_CARD_LEVEL_UP(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_CARD_LEVEL_UP");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_CARD_PUT_ON(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_CARD_PUT_ON");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void C2S_CARD_PUT_OFF(JSONObject jsObj)
    {
        jsObj.Add("cmd", "C2S_CARD_PUT_OFF");
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    

    //
    //public void C2S_checkBattleVaild(JSONObject j)
    //{
    //    StartCoroutine(IEC2S_checkBattleVaild(j));
    //}

    //--------------S2C------------------------------------------------------------------------------------------

    public void S2C_USER_LOGIN(JSONNode jsNode)
    {
        var data = jsNode["data"];
        if (data["code"] != 0)
        {
            SendCmdEvent(EventName.networkAborted, data);
            return;
        }

        GameData.instance.ServerTime = jsNode["data"]["serverTime"];
        GameData.instance.UerrLoginTime = Time.time;

        EventDispatcher<bool>.instance.TriggerEvent(EventName.UI_S2C_USER_LOGIN);
        var jsonObj = new JSONObject();
        jsonObj.Add("cmd", "C2S_USER_PING");
        jsonObj.Add("data", new JSONObject());
        C2S_LOAD_DATA(new JSONObject());
        //C2S_USER_PING(jsonObj);
        SendPing(jsonObj);
    }

    public void S2C_USER_PONG(JSONNode jsonNode)
    {

    }

    //,S2C_SYNCH_DATA_BEGIN,
    public void S2C_SYNCH_DATA_BEGIN(JSONNode jsonNode)
    {

    }

    //
    public void S2C_USER_DATA(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["user"];
        foreach (var item in values)
        {
            GameData.instance.UserData.Add(item.Key, item.Value);
        }
    }

    //,S2C_SYNCH_DATA_END,。。
    public void S2C_SYNCH_DATA_END(JSONNode jsonNode)
    {
        EventDispatcher<float>.instance.TriggerEvent(EventName.UI_LoadingChange, 1);
        WebSocketMain.instance.SendNewestNoSendC2S();

        if (UiMgr.IsOpenView<PopUpConfirmView>())
            UiMgr.Close<PopUpConfirmView>();

        EventDispatcher<string>.instance.TriggerEvent(EventName.Scene_ToSceneMainFsm);
        EventDispatcher<string>.instance.TriggerEvent(EventName.S2C_SYNCH_DATA_END);
    }

    public void S2C_USER_BATTLE_INFO(JSONNode jsonNode)
    {
        GameData.instance.PlayerBattleJsonObj = jsonNode.AsObject;
        SendCmdEvent("battleInfo", jsonNode);
        //eventNameQueue.Enqueue("battleInfo");
    }

    public void S2C_BATTLE_BEGIN(JSONNode jsonNode)
    {
        GameData.instance.EnemyBattleJsonObj = jsonNode.AsObject;
        SendCmdEvent("enemyBattleInfo", jsonNode);
        //eventNameQueue.Enqueue("enemyBattleInfo");
    }

    public void S2C_BATTLE_RESULT(JSONNode jsonNode)
    {


        EventDispatcher<JSONNode>.instance.TriggerEvent(EventName.Bat_BattleEnd, jsonNode);

        if (GuideMgr.guideNodeList.Count > 0)
        {
            foreach (var item in GuideMgr.guideNodeList)
            {
                //if (item.IsWaitForBattleFinish && item.stage == GuideNode.Stage.Guiding) 
                if (item.IsWaitForBattleFinish)
                {
                    item.NextStep();
                    break;
                }
            }
        }
    }

    //mails ,  ,。
    public void S2C_MAIL_DATA(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["mails"];
        foreach (var item in values)
        {
            GameData.instance.MailsData.Add(item.Key, item.Value);
        }

        GameUtils.JsonObjectQuickSort(GameData.instance.MailsData, 0, GameData.instance.MailsData.Count - 1);
        SendCmdEvent(EventName.mailData, values);
        ResetMailData();
    }

    private void ResetMailData()
    {
        var list = GameData.instance.MailsData.AsArray;
        JSONObject newMailsData = new JSONObject();
        foreach (var kv in GameData.instance.MailsData)
        {
            newMailsData.Add(kv.Value["id"], kv.Value);
        }
        GameData.instance.MailsData = newMailsData;
    }

    //, 
    public void S2C_MAIL_NEW(JSONNode jsonNode)
    {
        var item = jsonNode["data"]["mail"];
        GameData.instance.MailsData.Add(item["id"], item);
        GameUtils.JsonObjectQuickSort(GameData.instance.MailsData, 0, GameData.instance.MailsData.Count - 1);
        ResetMailData();

        SendCmdEvent(EventName.mail, item);
    }

    // update ,id,
    public void S2C_MAIL_UPDATE(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["update"];
        foreach (var item in values)
        {
            if (GameData.instance.MailsData.HasKey(item.Key))
            {
                var mail = GameData.instance.MailsData[item.Key];
                foreach (var item1 in item.Value)
                {
                    mail[item1.Key] = item1.Value;
                }
            }
        }
        SendCmdEvent(EventName.mail, values);
    }

    // delete id,id, ,
    public void S2C_MAIL_DELETE(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["delete"];
        foreach (var item in values)
        {
            GameData.instance.MailsData.Remove(item.Key);
        }
        GameUtils.JsonObjectQuickSort(GameData.instance.MailsData, 0, GameData.instance.MailsData.Count - 1);
        ResetMailData();
        SendCmdEvent(EventName.mail, values);
    }

    public void S2C_MAIL_REWARD(JSONNode jsonNode)
    {
        ReceiveShopItem(jsonNode);

        //var values = jsonNode["data"]["rewardItems"];
        SendCmdEvent(EventName.mailReward, jsonNode["data"]);
    }

    public void S2C_USER_KICK(JSONNode jsonNode)
    {
        var value = jsonNode["data"];
        SendCmdEvent(EventName.networkAborted, value);
    }

    //pvp
    public void S2C_PVP_RANK(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["rank"];
        SendCmdEvent(EventName.ranks, values);
    }

    //pvp
    public void S2C_PVP_RECORD_DATA(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["records"];
        foreach (var item in values)
        {
            GameData.instance.RecordsData.Add(item.Value["battleId"], item.Value);
        }

        //SendCmdEvent(EventName.record, values);
        SendCmdEvent(EventName.recordAll, values);
    }

    //
    public void S2C_PVP_RECORD_NEW(JSONNode jsonNode)
    {
        var value = jsonNode["data"]["record"];

        GameData.instance.RecordsData.Add(value["battleId"], value);
        SendCmdEvent(EventName.record, value);
    }

    //battleId id, 。
    public void S2C_PVP_RECORD_READ(JSONNode jsonNode)
    {
        var value = jsonNode["data"];
        GameData.instance.RecordsData[value["battleId"].ToString()]["read"] = 1;
        SendCmdEvent(EventName.record, value);
    }

    //battleId id, 。
    public void S2C_PVP_REVENGE(JSONNode jsonNode)
    {
        var value = jsonNode["data"];
        string battleId = value["battleId"];
        var record = GameData.instance.RecordsData[battleId];
        record["revenge"] = 1;
        SendCmdEvent(EventName.record, value);
    }

    // 

    public void S2C_FINISH_DAILY_TASK(JSONNode jsonNode)
    {
        SendCmdEvent(EventName.finish_daily_task, jsonNode);
    }

    public void S2C_FINISH_WEEKLY_TASK(JSONNode jsonNode)
    {
        SendCmdEvent(EventName.finish_week_task, jsonNode);
    }

    public void S2C_GET_DAILY_ACT_REWARD(JSONNode jsonNode)
    {
        SendCmdEvent(EventName.daily_act_reward, jsonNode);
    }

    public void S2C_GET_WEEKLY_ACT_REWARD(JSONNode jsonNode)
    {
        SendCmdEvent(EventName.week_act_reward, jsonNode);
    }

    public void S2C_FINISH_ACHIEVE_TASK(JSONNode jsonNode)
    {
        SendCmdEvent(EventName.finish_achieve_task, jsonNode);
    }

    public void ReceiveShopItem(JSONNode jsonNode) {
        JSONNode rewardItems = jsonNode["data"]["rewardItems"];
        List<PopUpReceiveView.ItemData> dataList = new List<PopUpReceiveView.ItemData>();

        foreach (var item in rewardItems)
        {
            dataList.Add(new PopUpReceiveView.ItemData() { ItemCfgId = item.Key, count = item.Value });
        }
        UiMgr.Open<PopUpReceiveView>(null, dataList);
    }

    public void S2C_RECHARGE_FINISH(JSONNode jsonNode)
    {
        ReceiveShopItem(jsonNode);

        JSONNode data = jsonNode["data"];
        SendCmdEvent(EventName.recharge_finish, data);
    }

    public void S2C_SHOP_BUY_CARD(JSONNode jsonNode)
    {
        ReceiveShopItem(jsonNode);
    }
    public void S2C_GET_POWER_REWARD(JSONNode jsonNode)
    {
        ReceiveShopItem(jsonNode);

        JSONNode data = jsonNode["data"];
        SendCmdEvent(EventName.power_reward, data);
    }

    public void S2C_GET_TIME_REWARD(JSONNode jsonNode)
    {
        var value = jsonNode["data"];
        SendCmdEvent(EventName.time_reward, value);
    }

    public void S2C_CARD_ACTIVE(JSONNode jsonNode)
    {
        var value = jsonNode["data"];
        SendCmdEvent(EventName.card_info, value);
    }

    public void S2C_CARD_LEVEL_UP(JSONNode jsonNode)
    {
        var value = jsonNode["data"];
        SendCmdEvent(EventName.card_info, value);
    }


    public void S2C_SHOP_BUY_BOX(JSONNode jsonNode)
    {
        JSONNode rewardItems = jsonNode["data"]["rewardItems"];
        List<PopUpReceiveView.ItemData> dataList = new List<PopUpReceiveView.ItemData>();

        foreach (var item in rewardItems)
        {
            for (int i = 0; i < item.Value; i++)
            {
                dataList.Add(new PopUpReceiveView.ItemData() { ItemCfgId = item.Key, count = 1 });
            }

            //dataList.Add(new PopUpReceiveView.ItemData() { ItemCfgId = item.Key, count = item.Value });
        }

        dataList.Sort((a, b) =>
        {
            JSONNode itemCfgA = GameData.instance.TableJsonDict["ItemConf"][a.ItemCfgId];
            JSONNode itemCfgB = GameData.instance.TableJsonDict["ItemConf"][b.ItemCfgId];

            if (itemCfgA["quality"] != itemCfgB["quality"]) { 
                return itemCfgA["quality"] > itemCfgB["quality"] ? -1 : 1;
            }
            return itemCfgA["cfgId"] < itemCfgB["cfgId"] ? -1 : 1;
        });

        ShopBoxOpenView view = UiMgr.GetView<ShopBoxOpenView>();
        if (view != null) {
            view.RewardRecieve(dataList);
        }
    }

    public void S2C_SHOP_BUY_GOLD(JSONNode jsonNode)
    {
        ReceiveShopItem(jsonNode);

        var value = jsonNode["data"];
        SendCmdEvent(EventName.shop_buy_coin, value);
    }

    //------------------------------------------------------------------------------------------------------------
    //:
    //userdata : ，name, avatar。
    //permanentdata : 
    //dailydata : 
    //weeklydata : 

    //permanentdata ： 、。
    // ,。,key, value。
    public void S2C_PERMANENT_DATA(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["permanent"];
        foreach (var item in values)
        {
            GameData.instance.PermanentData.Add(item.Key, item.Value);
            //eventNameQueue.Enqueue(item.Key);
            SendCmdEvent(item.Key, jsonNode);
        }
    }

    //, ,key, value。
    public void S2C_PERMANENT_DATA_UPDATE(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["update"];
        foreach (var item in values)
        {
            GameData.instance.PermanentData.Add(item.Key, item.Value);
            //eventNameQueue.Enqueue(item.Key);
            SendCmdEvent(item.Key, item, CmdEventAction.Update);
        }
    }

    //, ,key
    public void S2C_PERMANENT_DATA_DELETE(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["delete"];
        foreach (var item in values)
        {
            GameData.instance.PermanentData.Remove(item.Key);
            //eventNameQueue.Enqueue(item.Key);
            SendCmdEvent(item.Key, item, CmdEventAction.Delete);
        }
    }

    //key, , keyvalue,value
    public void S2C_PERMANENT_DATA_UPDATE1(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["update"];
        foreach (var items in values)
        {
            var dp1 = GameData.instance.PermanentData[items.Key];
            if (dp1 == null)
            {
                dp1 = new JSONObject();
                GameData.instance.PermanentData[items.Key] = dp1;
            }

            foreach (var item in items.Value)
            {
                dp1.Add(item.Key, item.Value);
            }

            //eventNameQueue.Enqueue(items.Key);
            SendCmdEvent(items.Key, items, CmdEventAction.Update);
        }
    }

    //key, , keyvalue, value, valuekey
    public void S2C_PERMANENT_DATA_DELETE1(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["delete"];
        foreach (var items in values)
        {
            var dp1 = GameData.instance.PermanentData[items.Key];
            foreach (var item in items.Value)
            {
                dp1.Remove(item.Key);
            }

            //eventNameQueue.Enqueue(items.Key);
            SendCmdEvent(items.Key, items, CmdEventAction.Delete);
        }
    }

    //key, , keyvalue, valuevalue1, value1
    public void S2C_PERMANENT_DATA_UPDATE2(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["update"];
        foreach (var itemss in values)
        {
            var dp1 = GameData.instance.PermanentData[itemss.Key];

            if (dp1 == null)
            {
                dp1 = new JSONObject();
                GameData.instance.PermanentData[itemss.Key] = dp1;
            }

            foreach (var items in itemss.Value)
            {
                var dp2 = dp1[items.Key];

                if (dp2 == null)
                {
                    dp2 = new JSONObject();
                    dp1[items.Key] = dp2;
                }

                foreach (var item in items.Value)
                {
                    dp2.Add(item.Key, item.Value);
                }
            }

            //eventNameQueue.Enqueue(itemss.Key);
            SendCmdEvent(itemss.Key, itemss, CmdEventAction.Update);
        }
    }

    //key, , keyvalue, valuevalue1, value1key
    public void S2C_PERMANENT_DATA_DELETE2(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["delete"];
        foreach (var itemss in values)
        {
            var dp1 = GameData.instance.PermanentData[itemss.Key];
            foreach (var items in itemss.Value)
            {
                var dp2 = dp1[items.Key];
                foreach (var item in items.Value)
                {
                    dp2.Remove(item.Key);
                }
            }

            //eventNameQueue.Enqueue(itemss.Key);
            SendCmdEvent(itemss.Key, itemss, CmdEventAction.Delete);
        }
    }

    //dailydata ： , 。
    //,。,key, value。
    public void S2C_DAILY_DATA(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["daily"];
        foreach (var item in values)
        {
            GameData.instance.DailyData.Add(item.Key, item.Value);
            //eventNameQueue.Enqueue(item.Key);
            SendCmdEvent(item.Key, item);
        }
    }

    //, ,key, value。
    public void S2C_DAILY_DATA_UPDATE(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["update"];
        foreach (var item in values)
        {
            GameData.instance.DailyData.Add(item.Key, item.Value);
            //eventNameQueue.Enqueue(item.Key);
            SendCmdEvent(item.Key, item, CmdEventAction.Update);
        }
    }

    //, ,key
    public void S2C_DAILY_DATA_DELETE(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["delete"];
        foreach (var item in values)
        {
            GameData.instance.DailyData.Remove(item.Key);
            //eventNameQueue.Enqueue(item.Key);
            SendCmdEvent(item.Key, item, CmdEventAction.Delete);
        }
    }

    // key, , keyvalue,value
    public void S2C_DAILY_DATA_UPDATE1(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["update"];
        foreach (var items in values)
        {
            var dp1 = GameData.instance.DailyData[items.Key];

            if (dp1 == null)
            {
                dp1 = new JSONObject();
                GameData.instance.DailyData[items.Key] = dp1;
            }

            foreach (var item in items.Value)
            {
                dp1.Add(item.Key, item.Value);
            }

            //eventNameQueue.Enqueue(items.Key);
            SendCmdEvent(items.Key, items, CmdEventAction.Update);
        }
    }

    //key, , keyvalue, value, valuekey
    public void S2C_DAILY_DATA_DELETE1(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["delete"];
        foreach (var items in values)
        {
            var dp1 = GameData.instance.DailyData[items.Key];
            foreach (var item in items.Value)
            {
                dp1.Remove(item.Key);
            }

            //eventNameQueue.Enqueue(items.Key);
            SendCmdEvent(items.Key, items, CmdEventAction.Delete);
        }
    }

    //key, , keyvalue, valuevalue1, value1
    public void S2C_DAILY_DATA_UPDATE2(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["update"];
        foreach (var itemss in values)
        {
            var dp1 = GameData.instance.DailyData[itemss.Key];

            if (dp1 == null)
            {
                dp1 = new JSONObject();
                GameData.instance.DailyData[itemss.Key] = dp1;
            }

            foreach (var items in itemss.Value)
            {
                var dp2 = dp1[items.Key];

                if (dp2 == null)
                {
                    dp2 = new JSONObject();
                    dp1[items.Key] = dp2;
                }

                foreach (var item in items.Value)
                {
                    dp2.Add(item.Key, item.Value);
                }
            }

            //eventNameQueue.Enqueue(itemss.Key);
            SendCmdEvent(itemss.Key, itemss, CmdEventAction.Update);
        }
    }

    // key, , keyvalue, valuevalue1, value1key
    public void S2C_DAILY_DATA_DELETE2(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["delete"];
        foreach (var itemss in values)
        {
            var dp1 = GameData.instance.DailyData[itemss.Key];
            foreach (var items in itemss.Value)
            {
                var dp2 = dp1[items.Key];
                foreach (var item in items.Value)
                {
                    dp2.Remove(item.Key);
                }
            }

            //eventNameQueue.Enqueue(itemss.Key);
            SendCmdEvent(itemss.Key, itemss, CmdEventAction.Delete);
        }
    }

    //,,
    public void S2C_DAILY_DATA_CLEAR(JSONNode jsonNode)
    {
        GameData.instance.DailyData.Clear();
    }

    //weeklydata ： , 。
    //,。,key, value。
    public void S2C_WEEKLY_DATA(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["weekly"];
        foreach (var item in values)
        {
            GameData.instance.WeeklyData.Add(item.Key, item.Value);
            //eventNameQueue.Enqueue(item.Key);
            SendCmdEvent(item.Key, item);
        }
    }

    //, ,key, value。
    public void S2C_WEEKLY_DATA_UPDATE(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["update"];
        foreach (var item in values)
        {
            GameData.instance.WeeklyData.Add(item.Key, item.Value);
            //eventNameQueue.Enqueue(item.Key);
            SendCmdEvent(item.Key, item, CmdEventAction.Update);
        }
    }

    //, ,key
    public void S2C_WEEKLY_DATA_DELETE(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["delete"];
        foreach (var item in values)
        {
            GameData.instance.WeeklyData.Remove(item.Key);
            //eventNameQueue.Enqueue(item.Key);
            SendCmdEvent(item.Key, item, CmdEventAction.Delete);
        }
    }

    //key, , keyvalue,value
    public void S2C_WEEKLY_DATA_UPDATE1(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["update"];
        foreach (var items in values)
        {
            var dp1 = GameData.instance.WeeklyData[items.Key];

            if (dp1 == null)
            {
                dp1 = new JSONObject();
                GameData.instance.WeeklyData[items.Key] = dp1;
            }

            foreach (var item in items.Value)
            {
                dp1.Add(item.Key, item.Value);
            }

            //eventNameQueue.Enqueue(items.Key);
            SendCmdEvent(items.Key, items, CmdEventAction.Update);
        }
    }

    //key, , keyvalue, value, valuekey
    public void S2C_WEEKLY_DATA_DELETE1(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["delete"];
        foreach (var items in values)
        {
            var dp1 = GameData.instance.WeeklyData[items.Key];
            foreach (var item in items.Value)
            {
                dp1.Remove(item.Key);
            }

            //eventNameQueue.Enqueue(items.Key);
            SendCmdEvent(items.Key, items, CmdEventAction.Delete);
        }
    }

    //key, , keyvalue, valuevalue1, value1
    public void S2C_WEEKLY_DATA_UPDATE2(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["update"];
        foreach (var itemss in values)
        {
            var dp1 = GameData.instance.WeeklyData[itemss.Key];

            if (dp1 == null)
            {
                dp1 = new JSONObject();
                GameData.instance.WeeklyData[itemss.Key] = dp1;
            }

            foreach (var items in itemss.Value)
            {
                var dp2 = dp1[items.Key];

                if (dp2 == null)
                {
                    dp2 = new JSONObject();
                    dp1[items.Key] = dp2;
                }

                foreach (var item in items.Value)
                {
                    dp2.Add(item.Key, item.Value);
                }
            }

            //eventNameQueue.Enqueue(itemss.Key);
            SendCmdEvent(itemss.Key, itemss, CmdEventAction.Update);
        }
    }

    //key, , keyvalue, valuevalue1, value1key
    public void S2C_WEEKLY_DATA_DELETE2(JSONNode jsonNode)
    {
        var values = jsonNode["data"]["delete"];
        foreach (var itemss in values)
        {
            var dp1 = GameData.instance.WeeklyData[itemss.Key];
            foreach (var items in itemss.Value)
            {
                var dp2 = dp1[items.Key];
                foreach (var item in items.Value)
                {
                    dp2.Remove(item.Key);
                }
            }

            //eventNameQueue.Enqueue(itemss.Key);
            SendCmdEvent(itemss.Key, itemss, CmdEventAction.Delete);
        }
    }

    //,,
    public void S2C_WEEKLY_DATA_CLEAR(JSONNode jsonNode)
    {
        GameData.instance.WeeklyData.Clear();
    }

    //------------------------------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------------------
    //
    public void S2C_SHOW_TIP(JSONNode jsonNode)
    {
        var content = jsonNode["data"]["content"];
        UiMgr.ShowTips(content);
    }

    public void S2C_GM_OPERATE(JSONNode jsonNode)
    {
        string msg = jsonNode["data"]["msg"];
        EventDispatcher<string>.instance.TriggerEvent(EventName.UI_GMOperateRet, msg);
    }
}