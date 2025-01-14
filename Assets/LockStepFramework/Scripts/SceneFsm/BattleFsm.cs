

using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleFsm : FsmState<BattleLogic>
    {
        public override void OnEnter(BattleLogic owner)
        {
            base.OnEnter(owner);
            GameData.instance.BattleScene = BattleScene.Battle;
            BattleMgr.instance.InitCost();
            BattleMgr.instance.Pause = false;

            var list = BattleMgr.instance.SoldierFlagDict;

            foreach (var item in list)
            {
                item.Value.StartLogic();
            }

            EventDispatcher<JSONNode>.instance.AddEvent(EventName.Bat_BattleEnd, (evtName, evt) =>
            {
                var jsonNode = evt[0];
                TimeMgr.instance.SetTimeAction((Fix64)0.5, () =>
                {
                    BattleMgr.instance.Pause = true;
                    owner.Fsm.ChangeFsmState<BattleEndFsm>();
                    EventDispatcher<JSONNode>.instance.TriggerEvent(EventName.UI_ShowBattleResult, jsonNode);
                });
            });

            TimeMgr.instance.SetTimeAction(Fix64.One, BattleMgr.instance.AddCost);
        }

        public override void OnUpdate(BattleLogic owner)
        {
            base.OnUpdate(owner);

            Move();
        }

        private void Move()
        {
            //Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //RVO.Vector2 target = new RVO.Vector2(targetPos.x, targetPos.y);
            List<EntityBase> itemAgents = BattleMgr.instance.SoldierList;

            foreach (var entityBase in itemAgents)
            {
                var agent = entityBase.Agent;
                int id = agent.id_;
                if (Simulator.instance.isNeedDelete(id))
                {
                    continue;
                }

                //FixVector2 targetPos = i % 2 == 0 ? GameData.instance.starPos1 + new FixVector2(10, 0) : GameData.instance.starPos2 + new FixVector2(-10, 0);
                //FixVector2 targetPos = GameData.instance.EndPos2;

                //if (BattleMgr.instance.CheckStage(entityBase, StageConst.StopAction))
                //{
                //    continue;
                //}


                //|| !entityBase.Agent.isORCA
                if (entityBase.TargetPos == FixVector2.None)
                {
                    continue;
                }

                FixVector2 target = entityBase.TargetPos;//entityBase.LockEntity.Fixv2LogicPosition;

                var goalVector = target - Simulator.instance.getAgentPosition(id);
                if (FixVector2.SqrMagnitude(goalVector) > (Fix64)0.01)
                {
                    goalVector = (goalVector).GetNormalized() * (Fix64)0.5;
                    Simulator.instance.setAgentPrefVelocity(id, goalVector);

                    Fix64 angle = GameData.instance.sRandom.Next() * (Fix64)2.0 * Fix64.PI;
                    Fix64 dist = GameData.instance.sRandom.Next() * (Fix64)0.0001;

                    Simulator.instance.setAgentPrefVelocity(id, Simulator.instance.getAgentPrefVelocity(id) +
                                                                dist *
                                                                new FixVector2(Fix64.Cos(angle), Fix64.Sin(angle)));
                }
                else
                {
                    //new FixVector2(0, 0)
                    Simulator.instance.setAgentPrefVelocity(id, new FixVector2(0, 0));
                }
            }

            Simulator.instance.doStep();
        }

        public override void OnLeave(BattleLogic owner)
        {
            EventDispatcher<JSONNode>.instance.RemoveEventByName(EventName.Bat_BattleEnd);

            base.OnLeave(owner);
        }
    }
}
