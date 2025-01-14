
using System.Linq;
using static UnityEngine.UI.GridLayoutGroup;

namespace Battle
{
    public class BattleLogic
    {
        public FsmCompent<BattleLogic> Fsm;// = new FsmCompent<BattleLogic>();

        public void Init()
        {
            Fsm = ClassPool.instance.Pop<FsmCompent<BattleLogic>>();
            //Fsm.CreateFsm(this, new BattleFsm(), new BattleEndFsm(), new BattleReadyFsm());
            Fsm.CreateFsm(this,
                 ClassPool.instance.Pop<BattleFsm>(),
                 ClassPool.instance.Pop<BattleEndFsm>(),
                 ClassPool.instance.Pop<BattleReadyFsm>()
                );
            Fsm.OnStart<BattleReadyFsm>();
        }

        public void FrameLockLogic()
        {
            if (BattleMgr.instance.Pause)
                return;

            //校验服按帧释放技能
            if (BattleMgr.instance.PlayMode == PlayMode.Check)
            {
                if (GameData.instance.OpFramesDict.TryGetValue(GameData.instance._UGameLogicFrame, out OpFrame op))
                {
                    CardSkillOp cardSkillOp = ClassPool.instance.Pop<CardSkillOp>();
                    Fix64 x = (Fix64)float.Parse(op.X);
                    Fix64 y = (Fix64)float.Parse(op.Y);
                    FixVector3 pos = new FixVector3(x, y, Fix64.Zero);
                    cardSkillOp.Init(op.CardSkillId, pos, PlayerGroup.Player, op.Frame);
                    BattleMgr.instance.DoCardSkill(cardSkillOp);
                    cardSkillOp.Release();
                }
            }
            else if (BattleMgr.instance.PlayMode == PlayMode.Play)
            {

                if (BattleMgr.instance.CardSkillOp != null && BattleMgr.instance.CardSkillOp.Frame == GameData.instance._UGameLogicFrame)
                {
                    BattleMgr.instance.DoCardSkill(BattleMgr.instance.CardSkillOp);

                    BattleMgr.instance.CardSkillOp.Release();
                    BattleMgr.instance.CardSkillOp = null;
                }
            }

            RecordLastTransForm();
            Update();

            if (BattleMgr.instance.PvMode == PvMode.GuideBattle && BattleMgr.instance.IsFirstBattle)
            {
                BattleMgr.instance.IsFirstBattle = false;
                Fsm.ChangeFsmState<BattleEndFsm>();
                EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, false);
                EventDispatcher<string>.instance.TriggerEvent(EventName.Scene_ToSceneMainFsm);
            }
        }

        private void Update()
        {
            Fsm.OnUpdate(this);

            for (int i = BattleMgr.instance.SoldierFlagDict.Count - 1; i >= 0; i--)
            {
                var item = BattleMgr.instance.SoldierFlagDict.ElementAt(i);

                if (item.Value.BKilled)
                {
                    BattleMgr.instance.SoldierFlagDict.Remove(item.Key);
                    item.Value.Release();
                    continue;
                }

                item.Value.Update();
            }

            for (int i = BattleMgr.instance.SoldierList.Count - 1; i >= 0; i--)
            {
                var item = BattleMgr.instance.SoldierList[i];

                if (item.BKilled)
                {
                    if (item.SoldierFlag != null)
                    {
                        item.SoldierFlag.SoldierBKill(item);
                    }

                    item.Agent.needDelete_ = true;
                    BattleMgr.instance.SoldierList.Remove(item);
                    item.Release();

                    continue;
                }

                item.UpdateLogic();
            }

            for (int i = BattleMgr.instance.SkillList.Count - 1; i >= 0; i--)
            {
                var item = BattleMgr.instance.SkillList[i];

                if (item.BKilled)
                {
                    BattleMgr.instance.SkillList.Remove(item);
                    item.Release();

                    continue;
                }

                item.Update();
            }

            for (int i = BattleMgr.instance.SkillEffectList.Count - 1; i >= 0; i--)
            {
                var item = BattleMgr.instance.SkillEffectList[i];

                if (item.BKilled)
                {
                    BattleMgr.instance.SkillEffectList.Remove(item);
                    item.Release();

                    continue;
                }

                if (item.SkillEffectData.frequency >= Fix64.Zero && item.NextTriggerTime <= GameData.instance._CumTime)
                {
                    item.Update();
                }
            }
#if _CLIENTLOGIC_
            for (int i = BattleMgr.instance.SpecialEffectList.Count - 1; i >= 0; i--)
            {
                var item = BattleMgr.instance.SpecialEffectList[i];

                if (item.BKilled)
                {
                    BattleMgr.instance.SpecialEffectList.Remove(item);
                    item.Release();
                }
            }
#endif

            TimeMgr.instance.LockStepUpdate();

            //var sumValue = GameUtils.CalcMd5(GameUtils.SumBattleValue());
            var sumValue = GameUtils.StepDetail();
            GameData.instance.StepMd5.Add(sumValue);
        }

        private void RecordLastTransForm()
        {
            foreach (var entity in BattleMgr.instance.SoldierList)
            {
                entity.RecordLastPos();
            }

            foreach (var skill in BattleMgr.instance.SkillList)
            {
                skill.RecordLastPos();
            }

#if _CLIENTLOGIC_
            foreach (var item in BattleMgr.instance.SpecialEffectList)
            {
                item.RecordLastPos();
            }
#endif
        }

        public void UpdateRenderPosition(Fix64 interpolation)
        {
            foreach (var entity in BattleMgr.instance.SoldierList)
            {
                if (!entity.BKilled)
                    entity.UpdateRenderPosition((float)interpolation);
            }

            foreach (var skill in BattleMgr.instance.SkillList)
            {
                if (!skill.BKilled)
                    skill.UpdateRenderPosition((float)interpolation);
            }
#if _CLIENTLOGIC_
            foreach (var item in BattleMgr.instance.SpecialEffectList)
            {
                if (!item.BKilled)
                    item.UpdateRenderPosition((float)interpolation);
            }
#endif
        }

#if _CLIENTLOGIC_
        public void UnityUpdate(float deltaTime)
        {
            foreach (var entity in BattleMgr.instance.SoldierList)
            {
                if (!entity.BKilled && entity.Trans != null)
                    entity.UpdateAnim(deltaTime);
            }

            foreach (var skill in BattleMgr.instance.SkillList)
            {
                if (!skill.BKilled && skill.Trans != null && skill.MainCfgid != 0)
                    skill.UpdateAnim(deltaTime);
            }


            foreach (var effect in BattleMgr.instance.SpecialEffectList)
            {
                if (!effect.BKilled && effect.Trans != null)
                    effect.UpdateAnim(deltaTime);
            }
        }

#endif

        public void Release()
        {
            Fsm?.ReleaseAllFsmState();
            Fsm = null;
            ClassPool.instance.Push(this);
        }
    }
}
