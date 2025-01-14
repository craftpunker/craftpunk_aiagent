
using Battle;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PvpView : UiBase
{
    Dictionary<RedPointBase, U3dObj> dictRedPoint;

    U3dObj BtnAuto;
    U3dObj Gain;

    U3dObj BtnInfor;
    U3dObj BtnRanking;
    U3dObj BtnHistory;

    U3dObj ImgGainMark;
    U3dObj TextGainCount;

    U3dObj BtnBattle;
    U3dObj Enemys;

    U3dObj BtnReflesh;
    U3dObj TxtCd;
    U3dObj TxtS;

    U3dObj TxtRefleshCd;

    U3dObj TxtPvpCount;
    U3dObj ImgLimit;

    JSONNode pvp_opps;

    Dictionary<GameObject, ItemBase> PvpBattleItemList = new Dictionary<GameObject, ItemBase>();

    private int pvpRef;
    private int pvpMax;

    protected override void OnInit()
    {
        BtnAuto = Find("BtnAuto");

        Gain = Find("Gain");
        ImgGainMark = Gain.Find("ImgGainMark");
        TextGainCount = Gain.Find("TextGainCount");
        BtnBattle = Find("BtnBattle");
        BtnInfor = Find("BtnInfor");
        BtnHistory = Find("BtnHistory");
        BtnRanking = Find("BtnRanking");

        Enemys = Find("Enemys");
        BtnReflesh = Find("BtnReflesh");

        TxtCd = Find("SeasonCountdown/TxtCd");
        TxtS = Find("SeasonCountdown/TxtS");

        TxtRefleshCd = BtnReflesh.Find("TxtCd");
        TxtPvpCount = Find("LayoutPvpCount/TxtPvpCount");
        ImgLimit = Find("ImgLimit");

        TxtS.Text.text = $"S{GameData.instance.PermanentData["pvp_name"]}";

        //float lessRefreshTime = ModuleData.DayEnd - Time.time;
        //TimeUtils.TimeHMS t = TimeUtils.DHMS(lessRefreshTime);
        //Debug.Log($"{t.Hour}:{t.Minute}:{t.Second}");

        SetOnClick(BtnInfor.gameObject, OnBtnInfor);
        SetOnClick(BtnRanking.gameObject, OnBtnRanking);

        SetOnClick(Gain.gameObject, OnClickGain);

        SetOnClick(BtnAuto.gameObject, OnBtnAuto);
        SetOnClick(BtnReflesh.gameObject, OnBtnReflesh);
        SetOnClick(BtnBattle.gameObject, OnBtnBattle);
        SetOnClick(BtnHistory.gameObject, OnBtnHistory);

        for (int i = 0; i < Enemys.transform.childCount; i++)
        {
            var child = Enemys.transform.GetChild(i);
            ItemBase.GetItem<PvpBtnBattleItem>(child, PvpBattleItemList, this);
        }

        pvpRef = GameData.instance.TableJsonDict["GlobalConf"]["pvpRef"]["intValue"];
        pvpMax = GameData.instance.TableJsonDict["GlobalConf"]["pvpMax"]["intValue"];

        dictRedPoint = new Dictionary<RedPointBase, U3dObj>();
        dictRedPoint.Add(RedPointMgr.GetRedPoint<PvpRecordRedPoint>(), BtnHistory);
    }

    private void OnBtnHistory()
    {
        UiMgr.Open<PvpHistoryView>();
    }

    private void OnBtnReflesh()
    {
        if (!isCanRefresh) {
            return;
        }
       
        JSONObject jsonObj = new JSONObject();
        jsonObj.Add("data", new JSONObject());
        Cmd.instance.C2S_PVP_REFRESH_OPPONENT(jsonObj);
    }

    protected override void OnShow()
    {
        //EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowRightMapHixBox, true);
        EventDispatcher<RedPointBase>.instance.AddEvent(EventName.Red_Point, OnRedPointChange);

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.pvp_opps, (evtName, evt) =>
        {
            var cmdEventData = evt[0];
            if (cmdEventData.EventAction == CmdEventAction.Update)
            {
                Reflesh();
            }
        });

        TxtPvpCount.Text.text = $"{pvpMax - GameData.instance.DailyData["pvp_cnt"].AsInt}";

        Reflesh();

        MapMgr.instance.ChangeMap(1, "Scene_right_4");

        RefreshRedPoint();

        var globalTable = GameData.instance.TableJsonDict["GlobalConf"];
        var pvpNeedLevel = globalTable["pvpNeedLevel"]["intValue"];
        int pass_pve = GameData.instance.PermanentData["pass_pve"];

        var pveTable = GameData.instance.TableJsonDict["PveConf"];
        var pveData = pveTable[pvpNeedLevel.ToString()];

        var rightView = UiMgr.GetView<RightView>();

        if (pvpNeedLevel > pass_pve)
        {
            ImgLimit.Find("Txt").Text.text = $"You can only unlock PvP after passing {pveData["name"].Value}";

            UiMgr.SetUIType(UIType.Loading, rightView);
            ImgLimit.SetActive(true);
        }
        else
        {
            ImgLimit.SetActive(false);
        }

//#if UNITY_EDITOR
//        ImgLimit.SetActive(false);
//#endif
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


    //======================
    bool isCanRefresh;

    protected override void OnUpdate()
    {
        base.OnUpdate();
        float lessRefreshTime = ModuleData.MonthEnd - Time.time;
        var dhms = TimeUtils.DHMS(lessRefreshTime);
        if (dhms.Day == 0 && dhms.Hour < 24)
        {
            TxtCd.Text.text = $"ends in {dhms.Hour}H {dhms.Minute}m";
        }
        else
        {
            TxtCd.Text.text = $"ends in {dhms.Day}D {dhms.Hour}H";
        }

        var nowTick = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var tick = GameData.instance.PermanentData["pvp_tick"];

        var second = nowTick - tick;
        second++;

        if (second >= pvpRef)
        {
            isCanRefresh = true;
            if (TxtRefleshCd.gameObject.activeSelf)
            {
                TxtRefleshCd.SetActive(false);
                BtnReflesh.SetImgGray(false);
                BtnReflesh.Button.interactable = true;
            }
        }
        else
        {
            isCanRefresh = false;
            if (!TxtRefleshCd.gameObject.activeSelf)
            {
                TxtRefleshCd.SetActive(true);
                BtnReflesh.SetImgGray(true);
                BtnReflesh.Button.interactable = false;
            }

            TxtRefleshCd.Text.text = $"00:{(pvpRef - second):D2}";
        }
    }

    private void Reflesh()
    {
        for (int i = 0; i < Enemys.transform.childCount; i++)
        {
            var btn = Enemys.transform.GetChild(i);
            btn.gameObject.SetActive(false);
            btn.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        pvp_opps = GameData.instance.PermanentData["pvp_opps"];//cmdEventData.JsonNode;

        for (int i = 0; i < pvp_opps.Count; i++)
        {
            var opps = pvp_opps[i];
            Enemys.transform.GetChild(i).gameObject.SetActive(true);
            PvpBtnBattleItem item = PvpBattleItemList.ElementAt(i).Value as PvpBtnBattleItem;
            item.SetData(opps);
        }
    }


    void OnBtnInfor()
    {
        UiMgr.Open<PvpInforView>();
    }

    void OnBtnRanking()
    {
        UiMgr.Open<PvpRankingView>();
    }

    private void OnBtnBattle()
    {
        //EventDispatcher<int>.instance.TriggerEvent(EventName.c2s_StartMatching, -1);
    }

    void OnClickGain()
    {
        UiMgr.ShowTips("comming soon");
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
        //EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowRightMapHixBox, false);
        EventDispatcher<RedPointBase>.instance.RemoveEvent(EventName.Red_Point, OnRedPointChange);
        EventDispatcher<CmdEventData>.instance.RemoveEventByName(EventName.pvp_opps);

        for (int i = 0; i < Enemys.transform.childCount; i ++)
        {
            var btn = Enemys.transform.GetChild(i).GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
        }
    }

    protected override void OnDestroy()
    {
    }

}
