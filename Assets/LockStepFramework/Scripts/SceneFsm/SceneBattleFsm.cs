


#if _CLIENTLOGIC_
using UnityEngine;
using Battle;

//
public class SceneBattleFsm : FsmState<Main>
{
    private LockStepLogic lockStepLogic;
    //private BattleLogic battleLogic;

    public override void OnEnter(Main owner)
    {
        base.OnEnter(owner);

        BattleMgr.instance.Pause = true;
        BattleMgr.instance.InitCost();

        UiMgr.Open<BattleView>(() =>
        {
            lockStepLogic = ClassPool.instance.Pop<LockStepLogic>();
            lockStepLogic.Init();
        });


#if _CLIENTLOGIC_
        //
        foreach (var entity in BattleMgr.instance.SoldierList)
        {
            entity.UpdateAnim(0);
        }
#endif

        //battleLogic = new BattleLogic();

        //battleLogic.Init();
        //lockStepLogic.SetCallUnit(battleLogic);

        //AudioMgr.instance.PlayOneShot(GameData.instance.TableJsonDict["AudioConf"]["attackAudio"], 1);
        EventDispatcher<string>.instance.AddEvent(EventName.Scene_ToSceneMainFsm, (evtName, evts) =>
        {
            UiMgr.Close<LoadingView>();
            UiMgr.Close<BattleView>();
            UiMgr.Close<PopUpConfirmView>();
            owner.Fsm.ChangeFsmState<SceneMainFsm>();

            if (evts.Length > 0)
            {
                string chcekMsg = evts[0];

                if (string.IsNullOrEmpty(chcekMsg))
                {
                    return;
                }

                PopUpConfirmMsg popUpConfirmMsg = ClassPool.instance.Pop<PopUpConfirmMsg>();
                popUpConfirmMsg.Content = chcekMsg;
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
                };

                UiMgr.Open<PopUpConfirmView>(null, popUpConfirmMsg);
            }
        });

        GameObject.Find("World/Map/Right").GetComponent<BoxCollider>().enabled = true;

        AudioMgr.instance.PlayOneShot("ui_battlestart");
        AudioMgr.instance.PlayLoop("bgm_battle");
    }

    public override void OnUpdate(Main owner)
    {
        base.OnUpdate(owner);

        if (BattleMgr.instance.Pause)
            return;

        lockStepLogic?.UpdateLogic();
        //AnimMgr.instance.UnityUpdate();
    }

    public override void OnLeave(Main owner)
    {
        EventDispatcher<string>.instance.RemoveEventByName(EventName.Scene_ToSceneMainFsm);
        lockStepLogic?.Release();
        //battleLogic.Release();

        Simulator.instance.Release();

        GameData.instance.Release();

        AudioMgr.instance.StopAll();
        TimeMgr.instance.Release();

        //battleLogic = null;
        lockStepLogic = null;
        base.OnLeave(owner);
    }
}
#endif