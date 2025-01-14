//#if _CLIENTLOGIC_
//using BattleEditor;
//using SimpleJSON;
//using System.Linq;
//using UnityEngine;


//namespace Battle
//{
//    public class SceneBattleLoadingFsm : FsmState<Main>
//    {
//        private int cumEntityCount;
//        private int currEntityCount;
//        private Main o;

//        private GameObject mapRight;
//        private GameObject rightRoll;

//        private float rightMapMoveTime = 0.3f;
//        private float time;
//        private Vector3 rightMapStartPos;
//        private Vector3 rightMapEndPos;
//        private Vector3 start2endDir;

//        private bool moveRightMap;
//        private bool canMoveRightMap;
        

//        public override void OnEnter(Main owner)
//        {
//            base.OnEnter(owner);

//            UiMgr.Close<MainView>();
//            UiMgr.Close<PveView>();
//            UiMgr.Close<PvpView>();
//            UiMgr.Close<PutSoldierView>();

//            cumEntityCount = 0;
//            currEntityCount = 0;
//            time = 0;
//            canMoveRightMap = false;
//            mapRight = GameObject.Find("World/Map/Right");
//            rightRoll = GameObject.Find("World/Map/Scene_right_roll");
//            SceneRightRoll(true);
//            moveRightMap = false;
//            o = owner;
//            GameData.instance.BattleScene = BattleScene.Loading;

//            Simulator.instance.setTimeStep(GameData.instance._FixFrameLen);
//            //Simulator.instance.setAgentDefaults((Fix64)1, 4, (Fix64)10, (Fix64)10.0, (Fix64)0.1, (Fix64)0.2, FixVector2.Zero);

//            //var pbJson = GameData.instance.BuildPlayerBattleData();
//            //var jsonStr = SimpleJson.SimpleJson.SerializeObject(GameData.instance.PlayerBattleJson);

//            for (int i = GameData.instance.TroopsMonos.Count - 1; i >= 0; i--)
//            {
//                var troop = GameData.instance.TroopsMonos.ElementAt(i).Value;
//                troop.Release();
//            }

//            GameData.instance.TroopsMonos.Clear();

//            EventDispatcher<CmdEventData>.instance.AddEvent(EventName.enemyBattleInfo, (evtName, evt) =>
//            {
//                GameData.instance.EnemyBattleData = GameData.instance.DeserializeObjectPlayerBattleData(GameData.instance.EnemyBattleJsonObj);
//                GameData.instance.PlayerBattleData = GameData.instance.DeserializeObjectPlayerBattleData(GameData.instance.PlayerBattleJsonObj);

//                foreach (var data in GameData.instance.EnemyBattleData.troops)
//                {
//                    var pos = data.Value.pos;
//                    data.Value.pos = new FixVector3(-pos.x, pos.y, pos.z);
//                    cumEntityCount += data.Value.count;
//                }

//                var dir = GameData.instance.RightMapHight > 0 ? 1 : -1;

//                MaterialPropertyBlock properties = new MaterialPropertyBlock();
//                properties.SetFloat("_Dir", dir);
//                rightRoll.transform.GetComponent<SpriteRenderer>().SetPropertyBlock(properties);
//                BattleMgr.instance.ResetSoldierData();
//                BattleMgr.instance.InitPlayerTroop(GameData.instance.EnemyBattleData, PlayerGroup.Enemy, CreatePrefabCallBack);
//                BattleMgr.instance.InitSoldierFlag();
//            });

//            MonoTimeMgr.instance.SetTimeAction(2, () =>
//            {
//                canMoveRightMap = true;
//            });

//            JSONObject jsonObj = new JSONObject();
//            JSONObject jsonObj1 = new JSONObject();
//            jsonObj1.Add("oppId", GameData.instance.OppsId);
//            jsonObj.Add("data", jsonObj1);
//            Cmd.instance.C2S_CHALLENGE_PVP(jsonObj);
//        }

//        public override void OnUpdate(Main owner)
//        {
//            base.OnUpdate(owner);

//            if (!canMoveRightMap)
//                return;

//            if (!moveRightMap)
//                return;

//            time += Time.deltaTime;
//            var t = time / rightMapMoveTime;

//            if (time > rightMapMoveTime)
//            {
//                SceneRightRoll(false);
//                mapRight.transform.localPosition = new Vector3(rightMapEndPos.x, rightMapEndPos.y, 0);

//                UiMgr.Open<BattleView>();

//                o.Fsm.ChangeFsmState<SceneBattleFsm>();
//                return;
//            }

//            mapRight.transform.localPosition = Vector3.Lerp(rightMapStartPos, rightMapEndPos, t);
//        }

//        private void SceneRightRoll(bool show)
//        {
//            if (show)
//            {
//                rightRoll.gameObject.SetActive(true);
//                //mapRight.gameObject.SetActive(false);
//                var pos = new Vector3(mapRight.transform.localPosition.x, mapRight.transform.localPosition.y, -1f);
//                mapRight.transform.localPosition = new Vector3(pos.x, GameData.instance.RightMapHight, pos.z);

//                rightMapStartPos = mapRight.transform.localPosition;
//                rightMapEndPos = pos;
//                start2endDir = rightMapEndPos - rightMapStartPos;

//            }
//            else
//            {
//                rightRoll.gameObject.SetActive(false);
//                mapRight.gameObject.SetActive(true);
//            }
//        }

//        //
//        private void CreatePrefabCallBack()
//        {
//            currEntityCount++;

//            if (currEntityCount == cumEntityCount)
//            {
//                moveRightMap = true;
//            }
//        }

//        public override void OnLeave(Main owner)
//        {
//            EventDispatcher<CmdEventData>.instance.RemoveEventByName(EventName.enemyBattleInfo);

//            o = null;
//            rightRoll = null;
//            mapRight = null;
//            base.OnLeave(owner);
//        }
//    }
//}
//#endif