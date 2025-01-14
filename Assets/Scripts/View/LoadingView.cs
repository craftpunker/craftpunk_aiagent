
using Battle;
using System;
using UnityEngine;

public class LoadingView : UiBase
{
    U3dObj slider;
    U3dObj textPlayer;

    float lessLoadTime;
    float loadTime;

    protected override void OnInit()
    {
        slider = Find("Slider");
        textPlayer = Find("TextPlayer");
    }

    protected override void OnShow()
    {
        lessLoadTime = 0;
        loadTime = 0;
        EventDispatcher<float>.instance.AddEvent(EventName.UI_LoadingChange, OnSliderChange);
    }

    void OnSliderChange(string evtName, params float[] args)
    {
        GameData.LoadingValue = args[0];
        slider.Slider.value = GameData.LoadingValue;
        slider.Text.text = "loading..." + Math.Round(GameData.LoadingValue * 100, 1) + "%";
    }

    protected override void OnUpdate()
    {
        //目前只有登录到主界面才用到loading,后续看需求再做通用
        if (GameData.LoadingValue >= 1)
        {
            EventDispatcher<string>.instance.TriggerEvent(EventName.Scene_ToSceneMainFsm);
            return;
        }

        lessLoadTime -= Time.deltaTime;
        GameData.LoadingValue = (loadTime - lessLoadTime) / loadTime;

        if (GameData.LoadingValue > 0.9)
            GameData.LoadingValue = 0.9f;
    }

    protected override void OnHide()
    {
        EventDispatcher<float>.instance.RemoveEventByName(EventName.UI_LoadingChange);
    }

    protected override void OnDestroy()
    {

    }
}
