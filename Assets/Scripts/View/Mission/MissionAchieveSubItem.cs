using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionAchieveSubItem : ItemBase
{
    KeyValuePair<string, JSONNode> data;
    public JSONNode cfg;

    U3dObj TxtCount;
    U3dObj TxtContent;
    U3dObj ImgIcon;

    U3dObj ImgReceive;
    U3dObj BtnReceive;

    U3dObj Slider;


    // Start is called before the first frame update
    protected override void OnInit(params object[] args)
    {
        _IsStep = true;
        TxtCount = Find("TxtCount");
        TxtContent = Find("TxtContent");
        ImgIcon = Find("ImgIcon");
        ImgReceive = Find("ImgReceive");
        BtnReceive = Find("BtnReceive");

        Slider = Find("Slider");

        SetOnClick(BtnReceive, OnBtnReceive);
    }

    public void SetData(KeyValuePair<string, JSONNode> data)
    {
        this.data = data;
        cfg = GameData.instance.TableJsonDict["TaskConf"][data.Key];

        Refresh();
    }

    public Vector3 GetAnimPos() { 
        return ImgIcon.transform.position;
    }

    void OnBtnReceive() {

        if (!isCanFetch) {
            //UiMgr.ShowTips("not completed");
            return;
        }

        JSONObject jsObj = new JSONObject();
        jsObj.Add("cmd", "C2S_FINISH_ACHIEVE_TASK");
        JSONObject data = new JSONObject();
        data.Add("cfgId", this.data.Key);

        jsObj.Add("data", data);

        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    bool isCanFetch = false;
    protected override void OnRefresh()
    {
        TxtContent.Text.text = cfg["desc"];

        int cur = data.Value["cur"];
        int need = data.Value["need"];

        if(data.Value["target"] == 13)
        {
            cur -= 10000;
            need -= 10000;
        }

        //TxtCount.Text.text = Math.Min(cur, need)  + "/" + need;

        Slider.Text.text = Math.Min(cur, need) + "/" + need;
        Slider.Slider.value = (float)cur / (float)need;

        isCanFetch = cur >= need;
        if (data.Value["draw"] == 0) {
            ImgReceive.SetActive(false);
            BtnReceive.SetActive(true);
            BtnReceive.SetImgGray(cur < need);

            BtnReceive.Button.interactable = cur >= need;
        }
        else
        {
            ImgReceive.SetActive(true);
            BtnReceive.SetActive(false);
        }

        JSONNode rewardCfg = GameData.instance.TableJsonDict["ItemConf"][cfg["rewardItem"][0][0].ToString()];

        ImgIcon.Text.text = cfg["rewardItem"][0][1];

        ImgIcon.SetSprite(rewardCfg["iconAtlas"], rewardCfg["icon"], true, () => {
            ImgIcon.SetSpriteRectBaseW(59);
        });
    }

    protected override void OnRelease()
    {

    }
}
