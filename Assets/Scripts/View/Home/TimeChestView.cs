using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TimeChestView : UiBase
{
    U3dObj LayoutGoldChest;
    U3dObj BtnClose;
    U3dObj BtnInfo;
    U3dObj BtnClaim;
    U3dObj TxtGoldAmount;
    U3dObj TxtGoldInfo;
    U3dObj TxtDiamondsAmount;
    U3dObj TxtDiamondsInfo;

    U3dObj LayoutRewardInfo;

    U3dObj TxtNextLevel;
    U3dObj TxtNextGold;
    U3dObj TxtNextDiamonds;

    U3dObj ImgBg;

    JSONNode TimeChestConf;
    JSONNode curTimeChestConf;
    int pass_pve;

    protected override void OnInit()
    {
        //Find

        ImgBg = Find("ImgBg");
        SetOnClick(ImgBg, OnBtnClose);

        LayoutGoldChest = Find("LayoutGoldChest");

        BtnClose = LayoutGoldChest.Find("BtnClose");
        SetOnClick(BtnClose, OnBtnClose);

        BtnInfo = LayoutGoldChest.Find("BtnInfo");
        SetOnClick(BtnInfo, OnBtnInfo);

        BtnClaim = LayoutGoldChest.Find("BtnClaim");
        SetOnClick(BtnClaim, OnBtnCliam);

        TxtGoldAmount = LayoutGoldChest.Find("LayoutGold/TxtGoldAmount");
        TxtGoldInfo = LayoutGoldChest.Find("LayoutGold/TxtGoldInfo");
        TxtDiamondsAmount = LayoutGoldChest.Find("LayoutDiamonds/TxtDiamondsAmount");
        TxtDiamondsInfo = LayoutGoldChest.Find("LayoutDiamonds/TxtDiamondsInfo");

        LayoutRewardInfo = Find("LayoutRewardInfo");
        TxtNextLevel = LayoutRewardInfo.Find("TxtNextLevel");
        TxtNextGold = LayoutRewardInfo.Find("LayoutGoldInfo/TxtNextGold");
        TxtNextDiamonds = LayoutRewardInfo.Find("LayoutDiamondsInfo/TxtNextDiamonds");


        TimeChestConf = GameData.instance.TableJsonDict["TimeChestConf"];
    }

    protected override void OnShow()
    {
        pass_pve = GameData.instance.PermanentData["pass_pve"];
        curTimeChestConf = TimeChestConf[pass_pve.ToString()];
        for (int i = 0; i < curTimeChestConf["rewardItem"].Count; i++)
        {
            if (curTimeChestConf["rewardItem"][i][0] == 100001)
            {
                TxtGoldInfo.Text.text = (curTimeChestConf["rewardItem"][i][1] * 60) + " / h";
            }
            else if (curTimeChestConf["rewardItem"][i][0] == 100002)
            {
                TxtDiamondsInfo.Text.text = (float.Parse(curTimeChestConf["rewardItem"][i][1]) * 60) + " / h";
            }
            else
            {
                DebugUtils.Log("ÅäÖÃ´íÎó" + curTimeChestConf["rewardItem"][i][0]);
            }
        }
        LayoutRewardInfo.SetActive(false);
        EventDispatcher<string>.instance.AddEvent(EventName.ui_change_page, CloseView);
        
    }

    protected override void OnUpdate()
    {
;        long nowSecond = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        bool isFull = (nowSecond - GameData.instance.PermanentData["time_chest"]) >= (curTimeChestConf["maxTime"] * 60) ? true : false;
        int gainMinute;
        if (isFull)
        {
            gainMinute = curTimeChestConf["maxTime"];
        }
        else
        {
            gainMinute = math.max((int)math.abs((nowSecond - GameData.instance.PermanentData["time_chest"])) / 60,0);
        }
        Debug.Log(gainMinute);
        for (int i = 0; i < curTimeChestConf["rewardItem"].Count; i++)
        {
            if (curTimeChestConf["rewardItem"][i][0] == 100001)
            {
                TxtGoldAmount.Text.text = (curTimeChestConf["rewardItem"][i][1] * gainMinute).ToString();
            }
            else if (curTimeChestConf["rewardItem"][i][0] == 100002)
            {
                TxtDiamondsAmount.Text.text = ((int)(float.Parse(curTimeChestConf["rewardItem"][i][1]) * gainMinute)).ToString();
            }
            else
            {
                DebugUtils.Log("ÅäÖÃ´íÎó" + curTimeChestConf["rewardItem"][i][0]);
            }
        }
    }

    protected override void OnHide()
    {
        EventDispatcher<string>.instance.RemoveEventByName(EventName.ui_change_page);
    }


    protected override void OnDestroy()
    {
        
    }

    void OnBtnClose()
    {
        UiMgr.Close<TimeChestView>();
    }

    private void CloseView(string evtName, string[] args)
    {
        UiMgr.Close<TimeChestView>();
    }

    void OnBtnInfo()
    {
        LayoutRewardInfo.SetActive(!LayoutRewardInfo.gameObject.activeSelf);
        int maxStage = TimeChestConf[TimeChestConf.Count - 1]["stage"];
        if (curTimeChestConf["stage"] == maxStage)
        {

        }
        else
        {
            int curId = curTimeChestConf["cfgId"];
            int maxId = TimeChestConf[TimeChestConf.Count - 1]["cfgId"];
            JSONNode nextStageConf = null;
            int nextStage = curTimeChestConf["stage"] + 1;
            int beginNextStage = 0;
            int endNextStage = 0;
            for (int i = curId; i < maxId; i++)
            {
                if (beginNextStage == 0 && TimeChestConf[i.ToString()]["stage"] == nextStage)
                {
                    beginNextStage = i;
                    nextStageConf = TimeChestConf[i.ToString()];
                    if (nextStage == maxStage )
                    {
                        endNextStage = maxId;
                        break;
                    }
                }
                else if (endNextStage == 0 && TimeChestConf[i.ToString()]["stage"] == nextStage+1)
                {
                    endNextStage = i - 1;
                    break;
                }
            }
            TxtNextLevel.Text.text = string.Format("Level {0} - {1}", TimeChestConf[beginNextStage.ToString()]["levelNumber"].AsInt, TimeChestConf[endNextStage.ToString()]["levelNumber"].AsInt);
            for (int i = 0; i < nextStageConf["rewardItem"].Count; i++)
            {
                if (nextStageConf["rewardItem"][i][0] == 100001)
                {
                    TxtNextGold.Text.text = (nextStageConf["rewardItem"][i][1] * 60) + " / h";
                }
                else if (curTimeChestConf["rewardItem"][i][0] == 100002)
                {
                    TxtNextDiamonds.Text.text = (int)(float.Parse(curTimeChestConf["rewardItem"][i][1]) * 60) + " / h";
                }
                else
                {
                    DebugUtils.Log("ÅäÖÃ´íÎó" + curTimeChestConf["rewardItem"][i][0]);
                }
            }
            Debug.Log(TimeChestConf[beginNextStage.ToString()]);
        }

    }

    void OnBtnCliam()
    {
        JSONObject jsObj = new JSONObject();
        UiMgr.Close<TimeChestView>();
        Cmd.instance.C2S_GET_TIME_REWARD(jsObj);

    }
}
