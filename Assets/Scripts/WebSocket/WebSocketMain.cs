
using UnityEngine;
using NativeWebSocket;
using UnityEngine.Networking;
using System.Collections;
using SimpleJSON;
using System;
using Battle;
using UnityEngine.Playables;

public class WebSocketMain : MonoSingleton<WebSocketMain>
{
    private WebSocket websocket;
    private JSONNode connectJsNode;
    private JSONObject newestNoSendC2S; //保存最新的因为断线没发出去的消息

    // 在首帧更新前调用
    void Start()
    {
        Cmd.instance.Init();
    }

    public void GetHttpAccount2Session(string account)
    {
        //if(account2SessionJsonnode == null)
        StartCoroutine(IEGetHttpAccount2Session(account));
    }

    IEnumerator IEGetHttpAccount2Session(string account)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(AppConfig.Account2SessionURL, "POST"))
        {
            var account2SessionJsonnode = GetAccount2SessionData(account);
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(account2SessionJsonnode.ToString());
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postBytes);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log($"==== {webRequest.error}");
                //重新链接
            }
            else
            {
                ConnectWebSocket(webRequest.downloadHandler.text);
            }
        }
    }

    private JSONObject GetAccount2SessionData(string account)
    {
        JSONObject jsonNode = new JSONObject();
        jsonNode.Add("proof", "CraftPunk2024##**##");

        jsonNode.Add("account", account == string.Empty ? SystemInfo.deviceUniqueIdentifier + 115151 : account);
        jsonNode.Add("platformId", "pc");
        jsonNode.Add("channelId", "20000");
        jsonNode.Add("gameChannelId", "20001");
        jsonNode.Add("packageName", "packageName");
        jsonNode.Add("serverId", 1);
        jsonNode.Add("parentUid", 0);
        jsonNode.Add("uid", 0);

        JSONObject jsonNode1 = new JSONObject();
        jsonNode.Add("device", jsonNode1);

        jsonNode1.Add("deviceName", "deviceName");
        jsonNode1.Add("osType", "pc");
        jsonNode1.Add("osVer", "0.0.0");
        jsonNode1.Add("netType", "wifi");
        jsonNode1.Add("deviceUuid", "25d55ad283aa400af464c76d713c07ad");
        jsonNode1.Add("iosIdfa", "");
        jsonNode1.Add("androidImei", "");
        jsonNode1.Add("screenHeight", 1920);
        jsonNode1.Add("screenWidth", 1080);
        jsonNode1.Add("userAgent", "");
        jsonNode1.Add("cpuAmount", 0);
        jsonNode1.Add("cpuModel", "");
        jsonNode1.Add("ramSize", 0);
        jsonNode1.Add("romSize", 0);

        return jsonNode;
    }

    async void ConnectWebSocket(string json)
    {
        Debug.Log(json);
        connectJsNode = JSONNode.Parse(json);
        var connectData = connectJsNode["data"];

        DebugUtils.Log("Connect " + connectJsNode["msg"]);
        //DebugUtils.Log("json:" + json);
        if (connectJsNode["code"].AsInt > 0)
        {
            if (UiMgr.IsOpenView<PopUpConfirmView>())
                return;

            PopUpConfirmMsg popUpConfirmMsg = ClassPool.instance.Pop<PopUpConfirmMsg>();
            popUpConfirmMsg.Content = connectJsNode["msg"];
            popUpConfirmMsg.Btn1Txt = "Cancel";
            popUpConfirmMsg.Btn2Txt = "OK";
            popUpConfirmMsg.showWaitImg = false;
            popUpConfirmMsg.Btn1Func = () =>
            {
                UiMgr.Close<PopUpConfirmView>();
            };

            popUpConfirmMsg.Btn2Func = () =>
            {
                UiMgr.Close<PopUpConfirmView>();
                //UiMgr.Close<WarningView>();
            };

            UiMgr.Open<PopUpConfirmView>(null, popUpConfirmMsg);

            return;
        }

        string hostWsPort = "";
#if UNITY_EDITOR
        hostWsPort = "ws://" + connectData["host"] + ":" + connectData["wsPort"];
        if (AppConfig.Account2SessionURL.Contains("https"))
        {
            hostWsPort = "wss://" + connectData["domain"] + "/wss" + connectData["wsPort"];
        }

#elif UNITY_WEBGL || UNITY_STANDALONE_WIN
        hostWsPort = "ws://" + connectData["host"] + ":" + connectData["wsPort"];
        if (AppConfig.Account2SessionURL.Contains("https"))
        {
            hostWsPort = "wss://" + "1252945775256932403.discordsays.com/.proxy/alpha-login" + "/wss" + connectData["wsPort"];
        }
#endif
        Debug.Log("hostWsPort:" + hostWsPort);
        Debug.Log(hostWsPort);
        websocket = new WebSocket(hostWsPort);

        websocket.OnOpen += () =>
        {
            Debug.Log("连接已建立！");

            var C2S_USER_LOGIN = new JSONObject();
            JSONObject data = new JSONObject();
            C2S_USER_LOGIN.Add("data", data);
            data.Add("uid", connectData["uid"].AsInt);
            data.Add("avatar", GameData.instance.DiscordIconURL == null ? "" : GameData.instance.DiscordIconURL);
            data.Add("name", GameData.instance.DiscordPlayerName == null ? "" : GameData.instance.DiscordPlayerName);
            data.Add("subId", connectData["subId"].AsInt);
            data.Add("token", "123");
            Cmd.instance.C2S_USER_LOGIN(C2S_USER_LOGIN);
        };

        websocket.OnError += (e) =>
        {
            EventDispatcher<CmdEventData>.instance.TriggerEvent(EventName.networkAborted);
            Debug.LogError("出错了！ " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("连接已关闭！");
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            //接收信息后解析json，然后分发事件
            DeserializationJson(message);

            //EventDispatcher.instance.TriggerEvent(EventId.StartMatching);
        };

        // 等待接收消息
        await websocket.Connect();
    }

    private void DeserializationJson(string json)
    {
        var jsonNode = JSONNode.Parse(json);
        string cmd = jsonNode["cmd"];
        if (!cmd.Equals("S2C_USER_PONG"))
            DebugUtils.Log($"接收: {cmd} {json}");
        if (Cmd.instance.S2CCmdDict.TryGetValue(cmd, out Action<JSONNode> jNode))
        {
            jNode(jsonNode);
        }
    }


    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if(websocket != null)
            websocket.DispatchMessageQueue();
#endif

        Cmd.instance.Update();
    }

    public async void SendWebSocketMessage(JSONObject msg)
    {
        var cmd = msg["cmd"];
        if (!cmd.Equals("C2S_USER_PING") && !cmd.Equals("C2S_LOAD_DATA") && !cmd.Equals("C2S_USER_LOGIN"))
        {
            newestNoSendC2S = msg;
        }

        if (websocket.State == WebSocketState.Open)
        {
            if (!cmd.Equals("C2S_USER_PING"))
                DebugUtils.Log($"发送: {msg}");
            // 发送纯文本
            await websocket.SendText(msg.ToString());
        }
        //else if (websocket.State == WebSocketState.Closed)
        //{
        //    DebugUtils.Log(websocket.State);
        //    EventDispatcher<CmdEventData>.instance.TriggerEvent(EventName.networkAborted);
        //}
        else if (websocket.State == WebSocketState.Aborted)
        {
            if (UiMgr.IsOpenView<PopUpConfirmView>())
                return;

            PopUpConfirmMsg popUpConfirmMsg = ClassPool.instance.Pop<PopUpConfirmMsg>();
            popUpConfirmMsg.Content = "You have been disconnected, please reconnect";
            popUpConfirmMsg.Btn1Txt = "Cancel";
            popUpConfirmMsg.Btn2Txt = "Reconnect";
            popUpConfirmMsg.showWaitImg = true;
            popUpConfirmMsg.Btn1Func = () =>
            {
                //ConnectWebSocket(connectJsNode.ToString());
            };

            popUpConfirmMsg.Btn2Func = () =>
            {
                ConnectWebSocket(connectJsNode.ToString());
                //UiMgr.Close<WarningView>();
            };

            UiMgr.Open<PopUpConfirmView>(null, popUpConfirmMsg);
        }
    }

    //发送断线后最新的C2S消息
    public void SendNewestNoSendC2S()
    {

        if (newestNoSendC2S != null)
            SendWebSocketMessage(newestNoSendC2S);

        newestNoSendC2S = null;
    }

    //{"userName":"yun200895","iconUrl":"https://cdn.discordapp.com/avatars/1240194225388392469/5445ffd7ffb201a98393cbdf684ea4b1.png?size=256"}
    public void GetDiscordData(string json)
    {
        Main.instance.Fsm.OnStart<SceneLoginFsm>();

        var jsonNode = JSONNode.Parse(json);

        GameData.instance.DiscordPlayerName = jsonNode["userName"];
        GameData.instance.DiscordIconURL = jsonNode["iconUrl"];
        GameData.instance.DiscordPlayerId = jsonNode["id"];

        //StartCoroutine(IEGetIcon(jsonNode));

        GetHttpAccount2Session(GameData.instance.DiscordPlayerId);

        //WebSocketMain.instance.GetHttpAccount2Session(inputUsername.text);
        //DeserializationJson(json);
    }

    private IEnumerator IEGetIcon(JSONNode jsonNode)
    {
        GameData.instance.DiscordPlayerName = jsonNode["userName"];
        GameData.instance.DiscordIconURL = jsonNode["iconUrl"];
        GameData.instance.DiscordPlayerId = jsonNode["id"];

        using (UnityWebRequest webRequest = new UnityWebRequest(GameData.instance.DiscordIconURL, "Post"))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(webRequest.error);
                yield break;
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                //GameData.instance.DiscordIcon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                GetHttpAccount2Session(GameData.instance.DiscordPlayerId);


            }
        }
    }

    public void C2S_checkBattleVaild()
    {
        StartCoroutine("IEC2S_checkBattleVaild");
    }

    IEnumerator IEC2S_checkBattleVaild()
    {
        JSONObject jsonObj = new JSONObject();
        JSONObject jsonObj1 = new JSONObject();
        jsonObj1.Add("battleId", GameData.instance.EnemyBattleJsonObj["data"]["battleId"]);
        jsonObj1.Add("battleType", GameData.instance.EnemyBattleJsonObj["data"]["pvp"]);
        jsonObj1.Add("atkBattleInfo", GameData.instance.PlayerBattleJsonObj);
        jsonObj1.Add("defBattleInfo", GameData.instance.EnemyBattleJsonObj);
        jsonObj1.Add("endStep", GameData.instance._UGameLogicFrame);

        JSONArray jsonArr = new JSONArray();
        foreach (var item in GameData.instance.OpFramesDict)
        {
            var op = item.Value;
            JSONObject jsonObj2 = new JSONObject();
            jsonObj2.Add("frame", op.Frame);
            jsonObj2.Add("x", op.X); //不用浮点，转字符串保小数点
            jsonObj2.Add("y", op.Y);
            jsonObj2.Add("cardSkillId", op.CardSkillId);
            jsonArr.Add(jsonObj2);
        }

        jsonObj1.Add("opFrame", jsonArr);
        jsonObj.Add("data", jsonObj1);

        string url = $"http://127.0.0.1:9001/checkBattleVaild";
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "Post"))
        {
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(jsonObj1.ToString());
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postBytes);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "text/plain");//"application/json"

            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(webRequest.error);
                //重新链接
            }
            else
            {
                Debug.Log(webRequest.result);
            }
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

}