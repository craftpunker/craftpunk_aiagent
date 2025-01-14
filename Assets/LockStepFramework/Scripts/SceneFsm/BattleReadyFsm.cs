


#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class BattleReadyFsm : FsmState<BattleLogic>
    {
        private BattleLogic m_owner;
        public override void OnEnter(BattleLogic owner)
        {
            base.OnEnter(owner);
            GameData.instance.BattleScene = BattleScene.Ready;
            m_owner = owner;
            BattleMgr.instance.PlayerCumHp = 0;
            BattleMgr.instance.EnemyCumHp = 0;

            foreach (var entity in BattleMgr.instance.SoldierList)
            {
                if (entity.SoldierType == SoldierType.WallSoldier || entity.SoldierType == SoldierType.SubWallSoldier)
                    continue;

                if (entity.PlayerGroup == PlayerGroup.Player)
                {
                    BattleMgr.instance.PlayerCumHp += (float)entity.Hp;
                }
                else if (entity.PlayerGroup == PlayerGroup.Enemy)
                {
                    BattleMgr.instance.EnemyCumHp += (float)entity.Hp;
                }
            }

            BattleMgr.instance.CurrEnemyCumHp = BattleMgr.instance.EnemyCumHp;
            BattleMgr.instance.CurrPlayerCumHp = BattleMgr.instance.PlayerCumHp;

            //CameraMove.instance.MoveToLeft();
            //EventDispatcher.instance.AddEvent(EventId.StartGame, StartGame);

#if _CLIENTLOGIC_

            //
            MonoTimeMgr.instance.SetTimeAction(0.25f, () =>
            {
                EventDispatcher<int>.instance.TriggerEvent(EventName.UI_BattleCountDown, 3);
                MonoTimeMgr.instance.SetTimeAction(0.25f, () =>
                {
                    EventDispatcher<int>.instance.TriggerEvent(EventName.UI_BattleCountDown, 2);
                    MonoTimeMgr.instance.SetTimeAction(0.25f, () =>
                    {
                        EventDispatcher<int>.instance.TriggerEvent(EventName.UI_BattleCountDown, 1);
                        MonoTimeMgr.instance.SetTimeAction(0.25f, () =>
                        {
                            EventDispatcher<int>.instance.TriggerEvent(EventName.UI_BattleCountDown, 0);
                            m_owner.Fsm.ChangeFsmState<BattleFsm>();

                            MonoTimeMgr.instance.SetTimeAction(0.25f, () =>
                            {
                                EventDispatcher<int>.instance.TriggerEvent(EventName.UI_BattleCountDown, -1);
                            });
                        });
                    });
                });
            });

            return;
#endif

            m_owner.Fsm.ChangeFsmState<BattleFsm>();
        }

        public override void OnUpdate(BattleLogic owner)
        {
            base.OnUpdate(owner);
        }

        public override void OnLeave(BattleLogic owner)
        {
            //EventDispatcher.instance.RemoveEvent(EventId.StartGame, StartGame);
            m_owner = null;
            base.OnLeave(owner);
        }
    }
}
