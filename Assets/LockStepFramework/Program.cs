
namespace Battle
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using SimpleJSON;

    class Program
    {
        static LockStepLogic lockStepLogic = new LockStepLogic();
        static BattleLogic battleLogic = new BattleLogic();

        static void Main(string[] args)
        {
            //string ServerIP = ConfigurationManager.AppSettings["ServerIP"];
            //string BindIP = ConfigurationManager.AppSettings["BindIP"];
            //string ServerPort = ConfigurationManager.AppSettings["ServerPort"];
            //string ThreadCount = ConfigurationManager.AppSettings["ThreadCount"];

            //< add key = "ServerIP" value = "127.0.0.1" />
            //< add key = "BindIP" value = "*" />
            //< add key = "ServerPort" value = "9001" />
            //< add key = "ThreadCount" value = "1" />

            // EXE
            //string exePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            //// txt
            //string txtFilePath = Path.Combine(exePath, "Config.json");
            //DebugUtils.Log(txtFilePath);
            //// 
            //if (File.Exists(txtFilePath))
            //{
            //    // 
            //    string content = File.ReadAllText(txtFilePath);
            //    Console.WriteLine(content);
            //}
            //else
            //{
            //    Console.WriteLine(": " + txtFilePath);
            //}

            HttpServer server = new HttpServer(1);

            BattleMgr.instance.Init();
            GameData.instance.Init();
            TimeMgr.instance.Init();

            
            server.AddPathListener("/checkBattleVaild", (ctx) =>
            {
                HttpListenerRequest request = ctx.Request;
                HttpListenerResponse response = ctx.Response;

                response.StatusCode = 200;
                response.ContentType = "text/plain;charset=UTF-8";
                response.ContentEncoding = Encoding.UTF8;
                response.AppendHeader("Content-type", "text/plain");//"text/plain"

                string content = string.Empty;

                JSONObject jsonObj = new JSONObject();
                //BattleResult result = new BattleResult();
                try
                {
                    using (StreamReader reader = new StreamReader(ctx.Request.InputStream, Encoding.UTF8))
                    {
                        DebugUtils.Log($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}------------------Battle Start---------------------");
                        content = reader.ReadToEnd();
                        //battleLogic.Init(BattleType.Replay, content);
                        //battleLogic.StartBattle();

                        BattleMgr.instance.Release();
                        GameData.instance.Release();
                        Simulator.instance.Release();
                        TimeMgr.instance.Release();

                        BattleMgr.instance.PlayMode = PlayMode.Check;

                        var battleJsonNode = JSONNode.Parse(content);
                        //DebugUtils.Log(battleJsonNode);
                        GameData.instance.PlayerBattleJsonObj = battleJsonNode["atkBattleInfo"].AsObject;
                        GameData.instance.EnemyBattleJsonObj = battleJsonNode["defBattleInfo"].AsObject;
                        uint battleId = battleJsonNode["battleId"];
                        GameData.instance.EndStep = battleJsonNode["endStep"];
                        //DebugUtils.Log(GameData.instance.EndStep);
                        GameData.instance.DeserializeObjectOpFrames(battleJsonNode["opFrame"]);

                        GameData.instance.sRandom.SetRandSeed(battleId);

                        GameData.instance.EnemyBattleData = GameData.instance.DeserializeObjectPlayerBattleData(GameData.instance.EnemyBattleJsonObj);
                        GameData.instance.PlayerBattleData = GameData.instance.DeserializeObjectPlayerBattleData(GameData.instance.PlayerBattleJsonObj);

                        BattleMgr.instance.ChangeMapSize(GameData.instance.PlayerBattleData);

                        foreach (var data in GameData.instance.EnemyBattleData.troops)
                        {
                            var pos = data.Value.pos;
                            data.Value.pos = new FixVector3(-pos.x, pos.y, pos.z);
                        }

                        BattleMgr.instance.InitPlayerTroop(GameData.instance.PlayerBattleData, PlayerGroup.Player, true);
                        BattleMgr.instance.InitPlayerTroop(GameData.instance.EnemyBattleData, PlayerGroup.Enemy, true);
                        BattleMgr.instance.InitSoldierFlag();

                        lockStepLogic.Init();

                        while (true)
                        {
                            if (BattleMgr.instance.IsOutcome)
                                break;
                            lockStepLogic.UpdateLogic();
                        }

                        jsonObj.Add("battleId", battleId);
                        jsonObj.Add("result", BattleMgr.instance.OutCome);
                        jsonObj.Add("atkResult", BattleMgr.instance.AtkResult);
                        jsonObj.Add("defResult", BattleMgr.instance.DefResult);
                        jsonObj.Add("endStep", GameData.instance._UGameLogicFrame);

                        //DebugUtils.Log(jsonObj);

                        //battleResult.setBattleResult(NewGameData._DeadEntityDict, NewGameData._Victory, NewGameData._UGameLogicFrame);
                        //DebugUtils.Log($"PVP end step {NewGameData._UGameLogicFrame}");
                        //Console.WriteLine("battleResult.ret: " + (battleResult.ret == 1 ? "victory" : "fail") + " endstep:" + NewGameData._UGameLogicFrame);
                    }
                }
                catch (Exception e)
                {
                    //battleResult.setCode(3);
                    //battleResult.setMsg(e.ToString());
                    //LogHelper.WriteLog(e.ToString() + "\n" + content, "error");
                    //LogHelper.WriteException(e.ToString() + "\n" + content, "error");
                    Console.WriteLine(e);
                }
                var utf8WithoutBom = new System.Text.UTF8Encoding(false);
                using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, utf8WithoutBom))
                {
                    //string resultStr = SimpleJson.SerializeObject(battleResult);
                    //DebugUtils.Log("resultStr：" + resultStr.Length);
                    DebugUtils.Log($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}------------------Battle End---------------------");
                    writer.Write(jsonObj.ToString());
                }

                content = null;
                //battleResult.Release();
            });

            server.Start("*", 9001);

        }

    }
}