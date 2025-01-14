
using Battle;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public enum PlayMode
    {
        Play = 0,
        Check = 1,
        RePlay = 2,
    }

    public enum PvMode
    {
        None = 0,
        Pve = 1,
        Pvp = 2,
        GuideBattle = 3,
    }

    public class CardSkillOp
    {
        public int CardCfgid;
        public FixVector3 StartPos;
        public PlayerGroup Group;
        public int Frame;

        public void Init(int cardCfgid, FixVector3 startPos, PlayerGroup group, int frame)
        {
            CardCfgid = cardCfgid;
            StartPos = startPos;
            Group = group;
            Frame = frame;
        }

        public void Release()
        {
            ClassPool.instance.Push(this);
        }
    }

    public class BattleMgr : Singleton<BattleMgr>
    {
        public List<EntityBase> SoldierList = new List<EntityBase>();
        public List<SkillBase> SkillList = new List<SkillBase>();
        public List<SkillEffectBase> SkillEffectList = new List<SkillEffectBase>();
        public SortedDictionary<int, SoldierFlagBase> SoldierFlagDict = new SortedDictionary<int, SoldierFlagBase>();

        public CardSkillOp CardSkillOp; //，

        public PlayMode PlayMode = PlayMode.Play;
        public PvMode PvMode = PvMode.None;

        public float PlayerCumHp;
        public float EnemyCumHp;
        public float CurrPlayerCumHp;
        public float CurrEnemyCumHp;

        public Fix64 CostMax;
        public Fix64 CostRecover;
        public Fix64 CurrCost;

        public string OutCome; // win fail

        public JSONObject AtkResult = new JSONObject();
        public JSONObject DefResult = new JSONObject();

        public bool IsFirstBattle;

#if _CLIENTLOGIC_
        public List<SpecialEffect> SpecialEffectList = new List<SpecialEffect>();
#endif

        public bool Pause = false;
        public bool IsOutcome = false;//

        public void Init()
        {
            Pause = false;
            IsOutcome = false;
            CardSkillOp = null;
            OutCome = null; 

            foreach (var item in SoldierList)
            {
                item.Release();
            }

            foreach (var item in SkillList)
            {
                item.Release();
            }

            foreach (var item in SkillEffectList)
            {
                item.Release();
            }

            foreach (var item in SoldierFlagDict)
            {
                item.Value.Release();
            }

#if _CLIENTLOGIC_
            foreach (var item in SpecialEffectList)
            {
                item.Release();
            }

            SpecialEffectList.Clear();
#endif

            SoldierList.Clear();
            SkillList.Clear();
            SkillEffectList.Clear();
            SoldierFlagDict.Clear();

            AtkResult.Clear();
            DefResult.Clear();
        }

        public void InitCost()
        {
            CurrCost = GameData.instance.PlayerBattleData.costInit;
            CostRecover = GameData.instance.PlayerBattleData.costRecover;
            CostMax = GameData.instance.PlayerBattleData.costMax;
        }

        public void AddCost()
        {
            CurrCost += CostRecover;

            if (CurrCost >= CostMax)
            {
                CurrCost = CostMax;
            }

            EventDispatcher<int>.instance.TriggerEvent(EventName.UI_UpdateCost);
            TimeMgr.instance.SetTimeAction(Fix64.One, AddCost);
        }

        //，
        public void SaveCardSkillOp(int cardCfgid, FixVector3 startPos, PlayerGroup group)
        {
            CardSkillOp = ClassPool.instance.Pop<CardSkillOp>();
            CardSkillOp.Init(cardCfgid, startPos, group, GameData.instance._UGameLogicFrame + 1);
        }

        //
        public void DoCardSkill(CardSkillOp op)
        {
            if (GameData.instance.CardDict.TryGetValue(op.CardCfgid, out CardData cardData))
            {
                if (cardData.Cost > CurrCost)
                {
                    //cost
                    return;
                }

                CurrCost -= cardData.Cost;
                EventDispatcher<int>.instance.TriggerEvent(EventName.UI_UpdateCost);
                SkillFactory.instance.CreateSkill(op.CardCfgid, null, null, op.StartPos, op.Group);

                if (PlayMode == PlayMode.Play)
                {
                    OpFrame opFrame = ClassPool.instance.Pop<OpFrame>();
                    opFrame.Frame = GameData.instance._UGameLogicFrame;
                    opFrame.X = op.StartPos.x.ToString();
                    opFrame.Y = op.StartPos.y.ToString();
                    opFrame.CardSkillId = op.CardCfgid;

                    GameData.instance.OpFramesDict.Add(opFrame.Frame, opFrame);
                }
            }
        }

        /// <summary>
        /////
        /// </summary>
        public void InitPlayerTroop(BattleData playerData, PlayerGroup group, bool IsAgent = false, Action callBack = null)
        {
            var troopsData = playerData.troops;
            for (int i = troopsData.Count - 1; i >= 0; i--)
            {
                var kv = troopsData.ElementAt(i);
                //TroopsData troopData = troopsData[i];
                CreateTroops(kv.Value, group, IsAgent, callBack);
            }
        }

        public void CreateTroops(TroopsData troopData, PlayerGroup group, bool IsAgent, Action callBack)
        {
            var troopPos = troopData.pos;
#if _CLIENTLOGIC_
            TroopsMono mono = null;
#endif
            if (group == PlayerGroup.Player && !IsAgent)
            {
                //mono
#if _CLIENTLOGIC_

                ResMgr.instance.LoadGameObjectAsync("TroopMono", (go) =>
                {
                    go.transform.position = new Vector3((float)troopPos.x, (float)troopPos.y, -0.1f);

                    mono = go.GetComponent<TroopsMono>();
                    mono.Id = troopData.id;
                    mono.Grid = troopData.grid;
                    mono.SoldierFlagId = troopData.SoldierFlagId;
                    var animData = GameData.instance.TableJsonDict["AnimConf"][troopData.animCfgId.ToString()];
                    go.transform.localScale = GameUtils.JsonPosToUnityV3(animData["prefabSize"]);

                    GameData.instance.TroopsMonos.Add(mono.Id, mono);
                    CreateTroopEntitys(troopData, group, troopPos, false, IsAgent, callBack, mono);
                });

#endif
                return;
            }

            CreateTroopEntitys(troopData, group, troopPos, false, IsAgent, callBack);
        }

        //
        public List<EntityBase> CreateTroopEntitys(TroopsData troopData, PlayerGroup group, FixVector3 pos, bool isSummon, bool isAgent,
            Action callBack,
            object obj = null) //TroopsMono
        {
#if _CLIENTLOGIC_
            TroopsMono mono = null;
            if (obj != null)
            {
                mono = obj as TroopsMono;
            }
#endif
            List<EntityBase> entitys = new List<EntityBase>();
            for (int j = 0; j < troopData.count; j++)
            {
#if _CLIENTLOGIC_
                var entity = CreateTroopEntity(troopData, mono, j, group, pos, isSummon, isAgent, callBack, obj);
#else
                var entity = CreateTroopEntity(troopData, j, group, pos, isSummon, isAgent, callBack, obj);
#endif
                entitys.Add(entity);
            }

            return entitys;
        }

#if _CLIENTLOGIC_
        //
        public EntityBase CreateTroopEntity(TroopsData troopData, TroopsMono mono, int index, PlayerGroup group, FixVector3 pos, bool isSummon,
            bool isAgent, Action callBack, object obj = null)
        {
            var offset = GameUtils.GetTroosEntityPos(index, troopData.count, group).ToFixVector3();

            EntityBase entity = CreateTroopEntityHandle(troopData, index, group, pos, isSummon, isAgent, callBack, offset);
            mono?.SetEntity(entity, offset);

            entity.DoAnim(AnimType.Idle);

            return entity;
        }
#endif

        //
        public EntityBase CreateTroopEntity(TroopsData troopData, int index, PlayerGroup group, FixVector3 pos, bool isSummon,
            bool isAgent, Action callBack, object obj = null)
        {
            var offset = GameUtils.GetTroosEntityPos(index, troopData.count, group).ToFixVector3();

            return CreateTroopEntityHandle(troopData, index, group, pos, isSummon, isAgent, callBack, offset);
        }

        private EntityBase CreateTroopEntityHandle(TroopsData troopData, int index, PlayerGroup group, FixVector3 pos, bool isSummon,
            bool isAgent, Action callBack, FixVector3 offset)
        {
            EntityBase entity = SoldierFactory.instance.CreateSolider(troopData, offset + pos, group, isSummon, isAgent, callBack);

            if (group == PlayerGroup.Enemy)
            {
                entity.FaceDir = 1;
            }

            return entity;
        }

        //
        public void InitSoldierFlag()
        {
            foreach (var kv in GameData.instance.PlayerBattleData.troops)
            {
                var flag = SoldierFlagFactory.instance.CreateFlag(kv.Value, PlayerGroup.Player);
                //Debug.Log($"ID：{kv.Key}");
                SoldierFlagDict.Add(flag.SoldierFlagId, flag);
            }

            foreach (var kv in GameData.instance.EnemyBattleData.troops)
            {
                var flag = SoldierFlagFactory.instance.CreateFlag(kv.Value, PlayerGroup.Enemy);
                //Debug.Log($"ID：{kv.Key}");
                SoldierFlagDict.Add(flag.SoldierFlagId, flag);
                //Debug.Log(flag.SoldierFlagId);
            }

            foreach (EntityBase soldier in SoldierList)
            {
                //Debug.Log(soldier.TroopsData.SoldierFlagId);
                var flag = SoldierFlagDict[soldier.TroopsData.SoldierFlagId];
                flag.SetSoldier(soldier);
            }
        }

        public void ResetSoldierData()
        {
            foreach (var soldier in SoldierList)
            {
                var troopData = soldier.TroopsData;
                if (soldier.PlayerGroup == PlayerGroup.Enemy)
                {
                    if (GameData.instance.EnemyBattleData.troops.ContainsKey(troopData.id))
                    {
                        SoldierFactory.instance.ResetSoliderData(soldier, GameData.instance.EnemyBattleData.troops[troopData.id], PlayerGroup.Enemy, false, null);
                    }
                }
                else
                {
                    if (GameData.instance.PlayerBattleData.troops.ContainsKey(troopData.id))
                    {
                        SoldierFactory.instance.ResetSoliderData(soldier, GameData.instance.PlayerBattleData.troops[troopData.id], PlayerGroup.Player, false, null);
                    }
                }
            }
        }

        //
        public void ChangeMapSize(BattleData playerData)
        {
            GameData.instance.AreaSize = playerData.mapData.areaSize;
#if _CLIENTLOGIC_
            string id = playerData.mapData.cfgId;
            var jsonNode = GameData.instance.TableJsonDict["PvpStageConf"][id];
            Camera.main.orthographicSize = jsonNode["cameraSize"].AsFloat / 1000;
            Vector2 v2 = GameUtils.JsonPosToV2(jsonNode["mapSize"].ToString()).ToVector2();
            GameObject.Find("World/Map").transform.localScale = new Vector3(v2.x, v2.y, 1);
#endif
        }

        public void Atk(Fix64 atk, EntityBase atker, EntityBase target)
        {
            if (target.BKilled)
                return;

            //90
            //if (target.SoldierType == SoldierType.SubWallSoldier)
            //{
            //    if (target is SubWallSoldier subWall)
            //    {
            //        target = subWall.MainWallSoldier;

            //        if (target == null || target.BKilled)
            //            return;
            //    }
            //}

            target.BeAtkAction?.Invoke(atker, atk);

            ChangeHp(atk, target);
        }

        //
        public void ChangeHp(Fix64 atk, EntityBase target)
        {
            Fix64 value = target.Hp;
            target.Hp -= atk;
#if _CLIENTLOGIC_
            target.UpdateBloodRander();
#endif
            if (target.Hp <= Fix64.Zero)
            {
                RecordKillData(target);
                target.Fsm.ChangeFsmState<DeadFsm>();
                CumHpChange(target, -value);
            }
            else
            {
                //
#if _CLIENTLOGIC_
                target.ShaderValue.z = 1;
#endif
                CumHpChange(target, -atk);
            }
        }

        private void RecordKillData(EntityBase target)
        {
            var jsonObj = target.PlayerGroup == PlayerGroup.Player ? AtkResult : DefResult;

            string cfgid = target.CfgId.ToString();
            if (jsonObj.HasKey(cfgid))
            {
                int conut = jsonObj[cfgid] + 1;
                jsonObj[cfgid] = conut;
            }
            else
            {
                jsonObj.Add(cfgid, 1);
            }
        }

        public void Heal(Fix64 heal, EntityBase target)
        {
            Fix64 value = target.MaxHp - target.Hp;
            target.Hp += heal;
#if _CLIENTLOGIC_
            target.UpdateBloodRander();
#endif

            if (target.Hp >= target.MaxHp)
            {
                target.Hp = target.MaxHp;
                CumHpChange(target, value);
            }
            else
            {
                //
                //target.ShaderValue.z = 1;
                CumHpChange(target, heal);
            }
        }

        public void ChangeAtk(EntityBase entity, Fix64 value)
        {
            if (entity == null)
                return;

            entity.CurrAttrValue.Atk += value;
        }

        public void ChangeMoveSpeed(EntityBase entity, Fix64 value)
        {
            if (entity == null)
                return;

            entity.CurrAttrValue.MoveSpeed += value;
        }

        public void ChangeAtkSpeed(EntityBase entity, Fix64 value)
        {
            if (entity == null)
                return;

            entity.CurrAttrValue.AtkSpeed += value;
        }

        //，StageConst
        public void AddStage(EntityBase entity, int stage)
        {
            entity.CurrentState |= stage;
        }

        public bool CheckStage(EntityBase entity, int stage)
        {
            return (entity.CurrentState & stage) != 0;
        }

        public void RemoveStage(EntityBase entity, int stage)
        {
            entity.CurrentState &= ~stage;
        }

        private void CumHpChange(EntityBase entity, Fix64 value)
        {
            if (IsOutcome)
                return;

            if (entity.IsSummon)
                return;

            if (entity.SoldierType == SoldierType.WallSoldier || entity.SoldierType == SoldierType.SubWallSoldier)
                return;

            if (entity.PlayerGroup == PlayerGroup.Player)
            {
                CurrPlayerCumHp += (float)value;
                EventDispatcher<int>.instance.TriggerEvent(EventName.UI_PlayerHpChange);
                //,
                if (CurrPlayerCumHp <= 1 && !CheckTroopsLife(entity.PlayerGroup))
                {
                    DecideOutcome("fail");
                    //EventDispatcher<int>.instance.TriggerEvent(EventName.Bat_BattleEnd, 0);
                }
            }
            else if (entity.PlayerGroup == PlayerGroup.Enemy)
            {
                CurrEnemyCumHp += (float)value;
                EventDispatcher<int>.instance.TriggerEvent(EventName.UI_EnemyHpChange);
                if (CurrEnemyCumHp <= 1 && !CheckTroopsLife(entity.PlayerGroup))
                {
                    DecideOutcome("win");
                    // EventDispatcher<int>.instance.TriggerEvent(EventName.Bat_BattleEnd, 1);
                }
            }
        }

        //
        public void DecideOutcome(string outCome)
        {
            if (IsOutcome)
                return;

            OutCome = outCome;
            //StringBuilder sb = new StringBuilder();
            //foreach (var str in GameData.instance.StepMd5)
            //{
            //    sb.AppendLine(str);
            //}

            //GameUtils.OutPutMd5File(sb);
            IsOutcome = true;
#if _CLIENTLOGIC_
            //WebSocketMain.instance.C2S_checkBattleVaild();

            //
            if (PvMode == PvMode.GuideBattle && !IsFirstBattle)
            {
                Pause = true;
                IsFirstBattle = true;
                GameData.instance.PlayerBattleJsonObj = GameData.instance.PlayerBattleJsonObjTemp;
                GameData.instance.PlayerBattleJsonObjTemp = null;
                return;
            }

            if (PlayMode == PlayMode.Play)
            {
                JSONObject jsonObj = new JSONObject();
                jsonObj.Add("data", new JSONObject());
                jsonObj["data"].Add("battleId", GameData.instance.EnemyBattleJsonObj["data"]["battleId"]);
                jsonObj["data"].Add("result", OutCome);
                jsonObj["data"].Add("atkResult", AtkResult);
                jsonObj["data"].Add("defResult", DefResult);
                jsonObj["data"].Add("endStep", GameData.instance._UGameLogicFrame);

                JSONArray jsonArr = new JSONArray();
                foreach (var item in GameData.instance.OpFramesDict)
                {
                    var op = item.Value;
                    JSONObject jsonObj2 = new JSONObject();
                    jsonObj2.Add("frame", op.Frame);
                    jsonObj2.Add("x", op.X); //，
                    jsonObj2.Add("y", op.Y);
                    jsonObj2.Add("cardSkillId", op.CardSkillId);
                    jsonArr.Add(jsonObj2);
                }

                jsonObj["data"].Add("opFrame", jsonArr);
                Cmd.instance.C2S_BATTLE_END(jsonObj);

                EventDispatcher<bool>.instance.TriggerEvent(EventName.UI_ShowBattleWaitImg, true);
            }
#endif
        }

        //true  false 
        private bool CheckTroopsLife(PlayerGroup group)
        {
            foreach (var item in SoldierList)
            {
                if (item.PlayerGroup == group && !item.BKilled)
                {
                    if (!item.IsSummon && item.SoldierType != SoldierType.WallSoldier && item.SoldierType != SoldierType.SubWallSoldier)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void LockFlag(SoldierFlagBase flag, SoldierFlagBase lockFlag)
        {
            if(flag == null || lockFlag == null)
                return;

            flag.LockTarget = lockFlag;
            flag.TargetPos = lockFlag.Pos;
            lockFlag.AddFlagLockMe(flag);
        }

        public void UnLockFlag(SoldierFlagBase flag, SoldierFlagBase lockMeFlag)
        {
            if (flag == null || lockMeFlag == null)
                return;

            lockMeFlag.Fsm?.ChangeFsmState<FlagSearchEnemyFlagFsm>();
            flag.RemoveFlagLockMe(lockMeFlag);
        }

    
        //
        public void FaceEnemy(EntityBase self, EntityBase target)
        {
            if (target == null)
                return;

            if (self.Fixv3LogicPosition.x <= target.Fixv3LogicPosition.x)
            {
                self.FaceDir = -1;
            }
            else
            {
                self.FaceDir = 1;
            }
        }

        public void FaceMovePos(EntityBase self)
        {
            if (self.Fixv3LogicPosition.x == self.Fixv3LastPosition.x)
                return;

            if (self.Fixv3LogicPosition.x > self.Fixv3LastPosition.x)
            {
                self.FaceDir = -1;
            }
            else
            {
                self.FaceDir = 1;
            }
        }

        public void FacePos(EntityBase self, FixVector3 pos)
        {
            if (self.Fixv3LogicPosition.x > pos.x)
            {
                self.FaceDir = 1;
            }
            else
            {
                self.FaceDir = -1;
            }
        }

        public void FaceFlag(EntityBase self, SoldierFlagBase flag)
        {
            if (self.Fixv3LogicPosition.x <= flag.Pos.x)
            {
                self.FaceDir = -1;
            }
            else
            {
                self.FaceDir = 1;
            }
        }

        //--------------------------------------------


        //----------------------()------------------------------

        public void ToSearchEnemyFsm(EntityBase entity)
        {
            if (CheckStage(entity, StageConst.StopAction))
                return;

            if (entity.Fsm?.GetFsmState<SearchEnemyFsm>() != null)
            {
                entity.Fsm.ChangeFsmState<SearchEnemyFsm>();
            }
        }

        //public void ToRemoteSearchFlagLockFlagEntitysFsm(EntityBase entity)
        //{
        //    if (entity.Fsm?.GetFsmState<RemoteSearchFlagLockFlagEntitysFsm>() != null)
        //    {
        //        entity.Fsm.ChangeFsmState<RemoteSearchFlagLockFlagEntitysFsm>();
        //    }
        //}

        //

        public void ToMoveToFlagOffestPosFsm(EntityBase entity)
        {
            if (entity.Fsm?.GetFsmState<MoveToFlagOffestPosFsm>() != null)
            {
                entity.Fsm.ChangeFsmState<MoveToFlagOffestPosFsm>();
            }
        }

        public void ToIdleFsm(EntityBase entity)
        {
            if (entity.Fsm?.GetFsmState<IdleFsm>() != null)
            {
                entity.Fsm.ChangeFsmState<IdleFsm>();
            }
        }

        public void ToSkefCtorFsm(EntityBase entity, SkillEffectBase skef)
        {
            if (entity.Fsm?.GetFsmState<SkefCtorFsm>() != null)
            {
                if (entity.SkillEffectFsm != null)
                {
                    entity.SkillEffectFsm.BKilled = true;
                }

                entity.Fsm.ChangeFsmState<SkefCtorFsm>();
                entity.SkillEffectFsm = skef;
            }
        }

        //----------------------------------------------------

        //JsonObj PVE
        public JSONObject GetDisPlayTroopsJsonObjByLayout(string id)
        {
            JSONObject jsonObj = new JSONObject();
            var pveData = GameData.instance.TableJsonDict["PveConf"][id];
            var layout = pveData["layout"];
            var soldierBattleData = GameData.instance.TableJsonDict["SoldierBattleConf"];
            jsonObj.Add("data", new JSONObject());
            jsonObj["data"].Add("battleInfo", new JSONObject());
            jsonObj["data"]["battleInfo"].Add("score", 0);
            jsonObj["data"]["battleInfo"].Add("troops", new JSONObject());
            int i = 1;
            var gridTable = GameData.instance.TableJsonDict["GridConf"];
            foreach (var item in layout)
            {
                string soldierBattleId = (item.Value[0].AsInt * 1000 + item.Value[1].AsInt).ToString();
                var soldierData = soldierBattleData[soldierBattleId];
                JSONObject obj1 = soldierData.Clone().AsObject;
                //ID，
                obj1["id"] = i;
                string grid = item.Value[2];
                var pos = gridTable[grid]["center"];
#if _CLIENTLOGIC_
                obj1["pos"] = new Vector2(-pos[0], pos[1]); //new Vector2(item.Value[2], item.Value[3]);
#endif
                //Debug.Log(obj1["pos"]);
                jsonObj["data"]["battleInfo"]["troops"].Add(obj1["id"], obj1);
                i++;
            }
            return jsonObj;
        }

        //Troop
        public void ResetTroopData(BattleData data)
        {
            var troops = data.troops;
            foreach (var kv in troops)
            {
                var troop = kv.Value;
            }
        }


        public void UpdateLogic()
        {

        }

        public void Release()
        {
            Init();
        }
    }
}


