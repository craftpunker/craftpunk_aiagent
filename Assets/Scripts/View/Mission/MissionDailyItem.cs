using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDailyItem : ItemBase
{
    public enum MissionItemType
    {
        Daily,
        Weekly

    }

    string dataKey;
    JSONNode data;
    public JSONNode taskCfg;

    MissionItemType type;

    U3dObj TxtContent;
    public U3dObj BtnReceive;

    U3dObj ImgFinish;

    U3dObj ImgFlag;

    U3dObj Slider;

    protected override void OnInit(params object[] args)
    {

        TxtContent = Find("BgContent/TxtContent");
        ImgFlag = Find("BgContent/ImgFlag");

        Slider = Find("BgContent/Slider");

        BtnReceive = Find("BtnReceive");

        ImgFinish = Find("ImgFinish");
        SetOnClick(BtnReceive, OnBtnReceive, "ui_task_complete");
    }


    public void SetData(string dataKey, JSONNode data, MissionItemType type)
    {

        this.dataKey = dataKey;
        this.data = data;
        this.type = type;
        taskCfg = GameData.instance.TableJsonDict["TaskConf"][dataKey];

        //Debug.Log(index);
        Refresh();
    }

    void OnBtnReceive()
    {

        if (!isCanFetch)
        {
            //UiMgr.ShowTips("not completed");
            return;
        }

        JSONObject jsonObj = new JSONObject();
        JSONObject data = new JSONObject();
        jsonObj.Add("data", data);
        data.Add("cfgId", dataKey);

        if (type == MissionItemType.Daily)
        {
            Cmd.instance.C2S_FINISH_DAILY_TASK(jsonObj);
        }
        else if (type == MissionItemType.Weekly)
        {

            Cmd.instance.C2S_FINISH_WEEKLY_TASK(jsonObj);
        }
    }

    protected override void OnRelease()
    {

    }

    bool isCanFetch = false;

    protected override void OnRefresh()
    {
        
        TxtContent.Text.text = taskCfg["desc"];

        Slider.Text.text = $"{data["cur"]} / {data["need"]}";
        Slider.Slider.value = (float)data["cur"] / (float)data["need"];


        bool isFetched = data["draw"] != 0;
        isCanFetch = data["cur"] >= data["need"];

        ImgFinish.SetActive(isFetched);
        BtnReceive.SetActive(!isFetched);

        if (isFetched || !isCanFetch)
        {
            ImgFlag.SetSprite("MissionAtlas", "Ui_task_active_icon_a");
        }
        else {
            ImgFlag.SetSprite("MissionAtlas", "Ui_task_active_icon");
        }

        BtnReceive.SetImgGray(!isCanFetch);
        BtnReceive.Button.interactable = isCanFetch;

        ImgFlag.Text.text = taskCfg["activation"];
    }
}
