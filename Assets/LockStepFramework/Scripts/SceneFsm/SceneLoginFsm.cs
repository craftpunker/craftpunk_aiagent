#if _CLIENTLOGIC_
using Battle;
using SimpleJSON;
using System;
using UnityEngine;

//，
public class SceneLoginFsm : FsmState<Main>
{
    LoadingView loadingView;
    LoginView loginView;

    //float loadTime = 2;
    //float lessLoadTime = 0;

    public override void OnEnter(Main owner)
    {
        base.OnEnter(owner);
        //lessLoadTime = loadTime;
        //Main.Loading = (loadTime - lessLoadTime) / loadTime;

        //if (AppConfig.Platform == "discord")
        //{
        //    loadingView = (LoadingView)UiMgr.Open<LoadingView>();
        //}
        //else
        //{
        //    loginView = (LoginView)UiMgr.Open<LoginView>();
        //}

        //BattleMgr.instance.Release();

        GameData.instance.Release();

        Simulator.instance.Release();
        TimeMgr.instance.Release();


        UiMgr.CloseAll();

        if (AppConfig.Platform == "webgl")
        {
            loginView = (LoginView)UiMgr.Open<LoginView>();
        }

        EventDispatcher<string>.instance.AddEvent(EventName.Scene_ToSceneMainFsm, (evtName, evts) =>
        {
            Main.instance.DestoryLoadingView();
            UiMgr.Close<LoginView>();
            UiMgr.Close<LoadingView>();
            owner.Fsm.ChangeFsmState<SceneMainFsm>();
        });
    }

    public override void OnUpdate(Main owner)
    {
        base.OnUpdate(owner);
    }



    public override void OnLeave(Main owner)
    {
        EventDispatcher<string>.instance.RemoveEventByName(EventName.Scene_ToSceneMainFsm);
    }
}
#endif