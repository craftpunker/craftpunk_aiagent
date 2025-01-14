#if _CLIENTLOGIC_
using Battle;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//    
public class SceneMainFsm : FsmState<Main>
{
    private Main o;

    private RaycastHit hit;
    public float rayLength = 100f;
    int layerMask;

    //-----------------------loading----------------------

    private int cumPvpEnemyEntityCount;
    private int currPvpEnemyEntityCount;

    private GameObject mapRight;
    private GameObject rightRoll;

    private float rightMapMoveTime = 0.3f;
    private float time;
    private Vector3 rightMapStartPos;
    private Vector3 rightMapEndPos;
    private Vector3 start2endDir;

    private bool moveRightMap;
    private bool canMoveRightMap;
    //----------------------------------------------------

    public override void OnEnter(Main owner)
    {
        base.OnEnter(owner);
        layerMask = LayerMask.GetMask("RayCast");

        if (!PlayerPrefs.HasKey("isOpenBlood"))
        {
            PlayerPrefs.SetInt("isOpenBlood", 1);
        }
        GameData.instance.IsOpenBlood = PlayerPrefs.GetInt("isOpenBlood") == 1 ? true : false;


        if (GameData.instance.PermanentData["guide_info"] == null)
        {
            GameData.instance.PlayerBattleJsonObjTemp = GameData.instance.PlayerBattleJsonObj;
            GameData.instance.PlayerBattleJsonObj = JSONObject.Parse(GameData.instance.FirstPlayerBattleJson).AsObject;
            GuideMgr.SendGuideInfo(new string[] { "10001" });
        }

        canMoveRightMap = false;
        canMoveRightMap = false;

        BattleMgr.instance.Release();
        o = owner;
        UiMgr.Open<PopUpConfirmView>();
        Simulator.instance.setAgentDefaults((Fix64)1, GameData.instance.MaxNeighbors, (Fix64)10, (Fix64)10.0, (Fix64)0.1, Fix64.One * 0.5, FixVector2.Zero);

        //if (GameData.instance.BattleData == null)
        GameData.instance.PlayerBattleData = GameData.instance.DeserializeObjectPlayerBattleData(GameData.instance.PlayerBattleJsonObj);

        BattleMgr.instance.ChangeMapSize(GameData.instance.PlayerBattleData);
        int currEntityCount = 0;
        int cumEntityCount = 0;

        AudioMgr.instance.PlayLoop("bgm_main");

        var troops = GameData.instance.PlayerBattleData.troops;
        foreach (var troop in troops)
        {
            cumEntityCount += troop.Value.count;
        }

        BattleMgr.instance.InitPlayerTroop(GameData.instance.PlayerBattleData, PlayerGroup.Player, false, () =>
        {
            currEntityCount++;
            if (cumEntityCount == currEntityCount)
            {
                UiMgr.Close<PopUpConfirmView>();
            }
        });
        GameData.instance.RightMapHight = 20;

        cumPvpEnemyEntityCount = 0;
        currPvpEnemyEntityCount = 0;
        time = 0;
        canMoveRightMap = false;
        mapRight = GameObject.Find("World/Map/Right");
        rightRoll = GameObject.Find("World/Map/Scene_right_roll");
        moveRightMap = false;

        EventDispatcher<bool>.instance.AddEvent(EventName.Scene_ShowEnemyTroops, ShowEnemyTroops);

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.cur_deploy, (evtName, evt) =>
        {
            var CmdEventData = evt[0];
        });

        //，，
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.battle_troop, (evtName, evt) =>
        {
            var battle_troop = GameData.instance.PermanentData["battle_troop"];
            var troopsDict = GameData.instance.PlayerBattleData.troops;
            var CmdEventData = evt[0];

            foreach (var item in CmdEventData.JsonNode.Value)
            {
                if (CmdEventData.EventAction == CmdEventAction.Delete) //
                {
                    int id = int.Parse(item.Key);
                    GameData.instance.TroopsMonos[id].RemoveEntitys();
                    GameData.instance.TroopsMonos.Remove(id);
                    GameData.instance.SelectedTroop = null;
                }
                else if (CmdEventData.EventAction == CmdEventAction.Update)
                {
                    IEnumerable<int> exceptKeys = null;

                    exceptKeys = troopsDict.Keys.Except(GameData.instance.TroopsMonos.Keys);

                    for (int i = exceptKeys.Count() - 1; i >= 0; i--)
                    {
                        var key = exceptKeys.ElementAt(i);
                        var newData = troopsDict[key];
                        BattleMgr.instance.CreateTroops(newData, PlayerGroup.Player, false, null);
                    }

                    //
                    var jsonTroops = GameData.instance.PlayerBattleJsonObj["data"]["battleInfo"]["troops"].AsObject;
                    foreach (var jsonTroop in jsonTroops)
                    {
                        var battle_troopData = battle_troop[jsonTroop.Key];
                        if (battle_troopData != null)
                        {
                            var jsonobj = jsonTroop.Value.AsObject;
                            jsonobj["pos"] = battle_troopData["pos"];
                        }
                    }
                }
            }
            UpdateEntityPos();

            EventDispatcher<string>.instance.TriggerEvent(EventName.UI_PutSoldierView_RenderItem);
        });

        //，
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.battleInfo, (evtName, evt) =>
        {
            GameData.instance.PlayerBattleData = GameData.instance.DeserializeObjectPlayerBattleData(GameData.instance.PlayerBattleJsonObj);
            var troops = GameData.instance.PlayerBattleData.troops;

            for (int i = troops.Count - 1; i >= 0; i--)
            {             
                var troop = troops.ElementAt(i).Value;

                if (GameData.instance.TroopsMonos.TryGetValue(troop.id, out TroopsMono mono))
                {
                    if (troop.count != mono.entityOffsetList.Count)
                    {
                        mono.RemoveEntitys();
                        BattleMgr.instance.CreateTroops(troop, PlayerGroup.Player, false, null);
                    }
                }
            }
        });

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.troop_info, UpdateTroopInfo);

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.Scene_EnterFirstBattle, (evtName, evt) =>
        {
            //GameData.instance.PlayerBattleJsonObjTemp = GameData.instance.PlayerBattleJsonObj;
            //GameData.instance.PlayerBattleJsonObj = JSONObject.Parse(GameData.instance.FirstPlayerBattleJson).AsObject;
            JSONNode enemyBattleJsonObj = JSONObject.Parse(GameData.instance.FirstEnemyBattleJson);
            GameData.instance.EnemyBattleJsonObj = enemyBattleJsonObj.AsObject;
            //GameData.instance.PlayerBattleData = GameData.instance.DeserializeObjectPlayerBattleData(playerBattleJsonObj.AsObject);

            CmdEventData evData = new CmdEventData();
            KeyValuePair<string, JSONNode> kv = new KeyValuePair<string, JSONNode>("1", enemyBattleJsonObj);
            evData.JsonNode = kv;
            EventDispatcher<CmdEventData>.instance.TriggerEvent(EventName.enemyBattleInfo, evData);
        });

        //，（），
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.enemyBattleInfo, (evtName, evt) =>
        {
            BattleMgr.instance.IsFirstBattle = false;
            var jsonData = evt[0].JsonNode.Value["data"];
            var pvType = jsonData["battleType"];
            GameData.instance.EnemyBattleData = GameData.instance.DeserializeObjectPlayerBattleData(GameData.instance.EnemyBattleJsonObj);
            GameData.instance.PlayerBattleData = GameData.instance.DeserializeObjectPlayerBattleData(GameData.instance.PlayerBattleJsonObj);
            uint battleId = jsonData["battleId"];

            GameData.instance.sRandom.SetRandSeed(battleId);

            foreach (var item in BattleMgr.instance.SpecialEffectList)
            {
                item.Release();
            }

            foreach (var item in BattleMgr.instance.SoldierList)
            {
                item.Release();
            }

            BattleMgr.instance.SoldierList.Clear();
            BattleMgr.instance.SpecialEffectList.Clear();

            for (int i = GameData.instance.TroopsMonos.Count - 1; i >= 0; i--)
            {
                var troop = GameData.instance.TroopsMonos.ElementAt(i).Value;
                troop.Release();
            }

            GameData.instance.TroopsMonos.Clear();

            BattleMgr.instance.InitPlayerTroop(GameData.instance.PlayerBattleData, PlayerGroup.Player, true);


            foreach (var data in GameData.instance.EnemyBattleData.troops)
            {
                var pos = data.Value.pos;
                data.Value.pos = new FixVector3(-pos.x, pos.y, pos.z);
                cumPvpEnemyEntityCount += data.Value.count;
            }

            if (pvType == "pve")
            {
                BattleMgr.instance.PvMode = PvMode.Pve;
                ReadyEnterPve();
            }
            else if (pvType == "pvp")
            {
                BattleMgr.instance.PvMode = PvMode.Pvp;
                int dir = -1;

                if (dir < 0)
                {
                    GameData.instance.RightMapHight *= -1;
                    //GameData.instance.RightMapHight -= 2.8f; //
                }

                //GameData.instance.OppsId = evt[1];

                UiMgr.Close<MainView>();
                UiMgr.Close<PveView>();
                UiMgr.Close<PvpView>();
                UiMgr.Close<PutSoldierView>();
                UiMgr.Close<PvpHistoryView>();

                SceneRightRoll(true);
                //moveRightMap = false;
                o = owner;
                GameData.instance.BattleScene = BattleScene.Loading;

                Simulator.instance.setTimeStep(GameData.instance._FixFrameLen);

                dir = GameData.instance.RightMapHight > 0 ? 1 : -1;
                MonoTimeMgr.instance.SetTimeAction(2, () =>
                {
                    canMoveRightMap = true;
                });

                MaterialPropertyBlock properties = new MaterialPropertyBlock();
                properties.SetFloat("_Dir", dir);
                rightRoll.transform.GetComponent<SpriteRenderer>().SetPropertyBlock(properties);
                //BattleMgr.instance.ResetSoldierData();
                BattleMgr.instance.InitPlayerTroop(GameData.instance.EnemyBattleData, PlayerGroup.Enemy, true, CreatePrefabCallBack);
                BattleMgr.instance.InitSoldierFlag();
            }
            else if (pvType == "guideBattle")
            {
                BattleMgr.instance.PvMode = PvMode.GuideBattle;
                MapMgr.instance.ChangeMap(1, "Scene_right_1.1");
                ReadyEnterPve();
            }
        });

        EventDispatcher<int>.instance.AddEvent(EventName.Scene_ChangeTroops, (evtName, evt) =>
        {
            int index = evt[0];
            JSONObject jsonObj = new JSONObject();
            JSONObject jsonObj1 = new JSONObject();
            jsonObj1.Add("index", index);
            jsonObj.Add("data", jsonObj1);
            Cmd.instance.C2S_CHOOSE_DEPLOY(jsonObj);
        });

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.record, (evtName, evt) =>
        {
            var CmdEventData = evt[0];
            //
        });

        UiMgr.Open<MainView>();

        PhysicsRayMgr.instance.MouseDown += MouseDown;
    }

    private void ReadyEnterPve()
    {
        //BattleMgr.instance.ResetSoldierData();
        BattleMgr.instance.InitPlayerTroop(GameData.instance.EnemyBattleData, PlayerGroup.Enemy, true);
        BattleMgr.instance.InitSoldierFlag();
        EnterPveBattle();
    }

    private void UpdateTroopInfo(string evtName, CmdEventData[] args)
    {
        if (args[0].EventAction == CmdEventAction.Update)
        {
            if (UiMgr.IsOpenView<SoldierLevelUpView>())
            {
                AudioMgr.instance.PlayOneShot("ui_soldier_upgrade");
            }
        }
    }

    private void MouseDown(Ray ray)
    {
        if (UiMgr.IsOpenView<PutSoldierView>())
            return;

        if (Physics.Raycast(ray, out hit, rayLength, layerMask))
        {
            if (hit.collider.TryGetComponent(out TroopsMono troopsMono))
            {
                //GameData.instance.SelectedTroop = troopsMono;
                //EventDispatcher<TroopsMono>.instance.TriggerEvent(EventName.Scene_CloseTroopRange, troopsMono);
                //troopsMono.Range = SpecialEffectFactory.instance.CreateSpecialEffect(30022, new FixVector3(hit.point.x, hit.point.y, hit.point.z), false, (go) =>
                //{
                //    go.GameObj.SetActive(true);

                //});
                GameData.instance.SelectedTroop = troopsMono;
                EventDispatcher<int>.instance.TriggerEvent(EventName.UI_OpenSoldierLevelUpView);
                EventDispatcher<int>.instance.TriggerEvent(EventName.Scene_ShowSelectTroopRange, troopsMono.Id);
            }
        }
    }

    private void UpdateEntityPos()
    {
        var playerTroops = GameData.instance.PermanentData["battle_troop"];
        foreach (var troop in playerTroops)
        {
            //var jsPos = troop.Value["pos"];
            var pos = GameUtils.JsonPosToV3(troop.Value["pos"]);
            var id = troop.Value["id"];

            if (GameData.instance.TroopsMonos.TryGetValue(id, out var troopMono))
            {
                troopMono.UpdatePos(pos);
                troopMono.ShowEntitys(true);
                troopMono.Grid = troop.Value["grid"];
            }
        }
    }

    private void EnterPveBattle()
    {
        UiMgr.Close<MainView>();
        UiMgr.Close<PveView>();
        UiMgr.Close<PvpView>();
        UiMgr.Close<PutSoldierView>();
        UiMgr.Close<HomeView>();
        o.Fsm.ChangeFsmState<SceneBattleFsm>();
    }

    private void ShowEnemyTroops(string evtName, bool[] args)
    {
        bool show = (bool)args[0];
        if (show)
        {
            UiMgr.Open<PopUpConfirmView>();
            BattleData enemyData = GameData.instance.DeserializeObjectPlayerBattleData(GameData.instance.EnemyBattleJsonObj);
            int currEntityCount = 0;
            int cumEntityCount = 0;

            var troops = enemyData.troops;

            foreach (var troop in troops)
            {
                cumEntityCount += troop.Value.count;
            }

            BattleMgr.instance.InitPlayerTroop(enemyData, PlayerGroup.Enemy, false, () =>
            {
                currEntityCount++;
                if (cumEntityCount == currEntityCount)
                {
                    UiMgr.Close<PopUpConfirmView>();
                }
            });
        }
        else
        {
            for (int i = BattleMgr.instance.SoldierList.Count - 1; i >= 0; i--)
            {
                var item = BattleMgr.instance.SoldierList[i];

                if (item.PlayerGroup == PlayerGroup.Player)
                    continue;

                item.BKilled = true;
                //Simulator.instance.delAgent(item.Agent.id_);
                //item.Agent.needDelete_ = true;
                BattleMgr.instance.SoldierList.Remove(item);
                item.Release();
            }
        }
    }

    public override void OnUpdate(Main owner)
    {
        base.OnUpdate(owner);

        for (int i = BattleMgr.instance.SoldierList.Count - 1; i >= 0; i--)
        {
            var item = BattleMgr.instance.SoldierList[i];
            item.UpdateAnim(0);
        }

        for (int i = BattleMgr.instance.SpecialEffectList.Count - 1; i >= 0; i--)
        {
            var item = BattleMgr.instance.SpecialEffectList[i];

            if (item.BKilled)
            {
                item.Release();
                BattleMgr.instance.SpecialEffectList.Remove(item);
                //continue;
            }

            item.UpdateAnim(Time.deltaTime);
        }

        if (!canMoveRightMap)
            return;

        if (!moveRightMap)
            return;

        time += Time.deltaTime;
        var t = time / rightMapMoveTime;
        if (time > rightMapMoveTime)
        {
            SceneRightRoll(false);
            mapRight.transform.localPosition = new Vector3(rightMapEndPos.x, rightMapEndPos.y, 0);

            //UiMgr.Open<BattleView>();

            o.Fsm.ChangeFsmState<SceneBattleFsm>();
            return;
        }

        mapRight.transform.localPosition = Vector3.Lerp(rightMapStartPos, rightMapEndPos, t);
    }

    private void SceneRightRoll(bool show)
    {
        if (show)
        {
            rightRoll.gameObject.SetActive(true);
            //mapRight.gameObject.SetActive(false);
            var pos = new Vector3(mapRight.transform.localPosition.x, mapRight.transform.localPosition.y, -1f);
            mapRight.transform.localPosition = new Vector3(pos.x, GameData.instance.RightMapHight, pos.z);

            rightMapStartPos = mapRight.transform.localPosition;
            rightMapEndPos = pos;
            start2endDir = rightMapEndPos - rightMapStartPos;

        }
        else
        {
            rightRoll.gameObject.SetActive(false);
            mapRight.gameObject.SetActive(true);
        }
    }

    private void CreatePrefabCallBack()
    {
        currPvpEnemyEntityCount++;
        if (currPvpEnemyEntityCount == cumPvpEnemyEntityCount)
        {
            moveRightMap = true;
        }
    }

    public override void OnLeave(Main owner)
    {
        o = null;
        rightRoll = null;
        mapRight = null;
        EventDispatcher<CmdEventData>.instance.RemoveEventByName(EventName.battleInfo);
        EventDispatcher<CmdEventData>.instance.RemoveEventByName(EventName.battle_troop);
        EventDispatcher<bool>.instance.RemoveEventByName(EventName.Scene_ShowEnemyTroops);
        EventDispatcher<CmdEventData>.instance.RemoveEventByName(EventName.enemyBattleInfo);
        EventDispatcher<CmdEventData>.instance.RemoveEventByName(EventName.cur_deploy);
        EventDispatcher<int>.instance.RemoveEventByName(EventName.Scene_ChangeTroops);
        EventDispatcher<CmdEventData>.instance.RemoveEventByName(EventName.record);
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.troop_info, UpdateTroopInfo);
        EventDispatcher<CmdEventData>.instance.RemoveEventByName(EventName.Scene_EnterFirstBattle);
        PhysicsRayMgr.instance.MouseDown -= MouseDown;
        //EventDispatcher<bool>.instance.RemoveEventByName(EventName.Scene_ShowTroopRange);
        AudioMgr.instance.StopAll();
        base.OnLeave(owner);
    }
}
#endif