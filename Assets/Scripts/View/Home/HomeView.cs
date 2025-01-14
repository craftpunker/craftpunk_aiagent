using Battle;
using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HomeView : UiBase
{
    U3dObj BtnAuto;
    U3dObj TimeChest;
    U3dObj PowerChest;
    U3dObj TxtTime;

    U3dObj ImgGainMark;
    U3dObj TextGainCount;

    U3dObj Reward1;
    U3dObj Reward2;
    U3dObj Reward3;

    U3dObj powerChestAnim;
    U3dObj rewardChestAnim;

    JSONNode TimeChestConf;

    Dictionary<RedPointBase, U3dObj> dictRedPoint;

    protected override void OnInit()
    {
        dictRedPoint = new Dictionary<RedPointBase, U3dObj>();
        BtnAuto = Find("BtnAuto");

        TimeChest = Find("TimeChest");
        PowerChest = Find("PowerChest/PowerClick");
        TxtTime = Find("TimeChest/TxtTime");


        Reward1 = Find("Reward1");
        Reward2 = Find("Reward2");
        Reward3 = Find("Reward3");


        SetOnClick(TimeChest.gameObject, OnClickTimeChest);
        SetOnClick(PowerChest.gameObject, OnClickPowerChest);

        SetOnClick(BtnAuto.gameObject, OnBtnAuto);

        SetOnClick(Reward1.gameObject, OnClickReward1);
        SetOnClick(Reward2.gameObject, OnClickReward2);
        SetOnClick(Reward3.gameObject, OnClickReward3);

        TimeChestConf = GameData.instance.TableJsonDict["TimeChestConf"];

        powerChestAnim = Find("PowerChestAnim");
        rewardChestAnim = Find("RewardChestAnim");

        //DebugUtils.Log(GameData.instance.UserData)

        dictRedPoint.Add(RedPointMgr.GetRedPoint(typeof(PowerChestRedPoint)), PowerChest);
    }

    //red
    void OnRedPointChange(string evtName, RedPointBase[] evt)
    {
        RedPointBase redpoint = evt[0];
        if (dictRedPoint.TryGetValue(redpoint, out U3dObj obj))
        {
            obj.SetRedPoint(redpoint.IsRed);
        }
    }

    void RefreshRedPoint()
    {
        //BtnOptionList
        foreach (var item in dictRedPoint)
        {
            item.Value.SetRedPoint(item.Key.IsRed);
        }
    }

    //=======


    protected override void OnShow()
    {
        EventDispatcher<RedPointBase>.instance.AddEvent(EventName.Red_Point, OnRedPointChange);
        RefreshRedPoint();
        //EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowRightMapHixBox, true);
        //TimeMgr.instance.SetTimeAction()
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.time_reward, OnTimeReward);
        MapMgr.instance.ChangeMap(1, "Scene_right_2");

        powerChestAnim.transform.GetComponent<Animator>().SetInteger("value", 0);
        rewardChestAnim.transform.GetComponent<Animator>().SetInteger("value", 1);
    }

    private void OnTimeReward(string evtName, CmdEventData[] args)
    {
        JSONNode data = args[0].JsonNode;
        ResAnim.instance.ShowUIAnim(data["rewardItems"], TimeChest.transform.position);
    }

    protected override void OnUpdate()
    {
        long nowMillSecond = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        int second = (int)nowMillSecond - GameData.instance.PermanentData["time_chest"];
        int pass_pve = GameData.instance.PermanentData["pass_pve"];
        int maxTimeMin = TimeChestConf[pass_pve.ToString()]["maxTime"];
        if ((maxTimeMin * 60) <= second) //
        {
            TxtTime.Text.text = "Is Max";
        }
        else
        {
            int showSecond = (int)(maxTimeMin * 60 - second);
            int hour = showSecond / 3600;
            int minute = (showSecond - hour * 3600) / 60;
            int secondTime = showSecond - (hour * 3600) - (minute * 60);
            TxtTime.Text.text = string.Format("{0}:{1}:{2}", hour, minute < 10 ? "0" + minute : minute, secondTime < 10?"0" + secondTime : secondTime);
        }

    }

    void OnClickTimeChest()
    {
        UiMgr.Open(UiMgr.GetUiAssetPath<TimeChestView>());
    }

    void OnClickPowerChest()
    {
        UiMgr.Open(UiMgr.GetUiAssetPath<PowerChestView>());
    }

    void OnBtnAuto()
    {
        DebugUtils.Log("OnBtnAuto");
    }


    void OnClickReward1()
    {
        UiMgr.ShowTips("comming soon");

    }

    void OnClickReward2()
    {
        UiMgr.ShowTips("comming soon");

    }


    void OnClickReward3()
    {
        UiMgr.ShowTips("comming soon");

    }


    protected override void OnHide()
    {
        EventDispatcher<RedPointBase>.instance.RemoveEvent(EventName.Red_Point, OnRedPointChange);
        //EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowRightMapHixBox, false);
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.time_reward, OnTimeReward);
    }

    protected override void OnDestroy()
    {
    }
}