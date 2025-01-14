using Battle;
using DG.Tweening;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionActBoxItem : ItemBase
{
    public U3dObj ImgBox;
    U3dObj TxtBox;

    public string key;
    public JSONNode cfg;

    JSONNode rewardData = null;

    U3dObj LayoutReward;
    U3dObj ImgRewardIcon;
    U3dObj TxtRewardCount;

    U3dObj RewardAnim;

    protected override void OnInit(params object[] args)
    {
        ImgBox = Find("ImgBox");
        TxtBox = Find("TxtBox");

        LayoutReward = Find("LayoutReward");
        ImgRewardIcon = LayoutReward.Find("ImgRewardIcon");
        TxtRewardCount = LayoutReward.Find("TxtRewardCount");
        SetOnClick(gameObject, OnClickBox, "ui_task_boxClaim");
        RewardAnim = Find("RewardAnim");
    }

    bool isCanFetch = false;
    int score;

    Sequence animSeq;

    public void SetData(string key, JSONNode cfg) { 
        this.key = key;
        this.cfg = cfg;

        TxtBox.Text.text = cfg["needActivePoint"];

        score = 0;

        rewardData = null;
        if (cfg["rewardType"] == 1) {

            if (GameData.instance.DailyData["da_reward"] != null) {
                rewardData = GameData.instance.DailyData["da_reward"][key];
            }

            score = GameData.instance.DailyData["d_atc"];
        }
        else if(cfg["rewardType"] == 2)
        {
            if (GameData.instance.WeeklyData["wa_reward"] != null) {
                rewardData = GameData.instance.WeeklyData["wa_reward"][key];
            }

            score = GameData.instance.WeeklyData["w_atc"];
        }

        if (rewardData == null)
        {
            ImgBox.SetSprite("MissionAtlas", "Ui_task_bao_a", true);
        }
        else {
            ImgBox.SetSprite("MissionAtlas", "Ui_task_kaibao_a", true);
        }

        isCanFetch = score >= cfg["needActivePoint"] && rewardData == null;
        ImgBox.SetRedPoint(isCanFetch);

        JSONNode reward = cfg["reward"][0];

        JSONNode rewardItemCfg = GameData.instance.TableJsonDict["ItemConf"][reward[0].ToString()];

        LayoutReward.SetActive(false);
        ImgRewardIcon.SetSprite(rewardItemCfg["iconAtlas"], rewardItemCfg["icon"]);
        TxtRewardCount.Text.text = reward[1];

        if (animSeq != null) {
            animSeq.Kill();
            animSeq = null;
        }

        if (score >= cfg["needActivePoint"] && rewardData == null)
        {
            RewardAnim.SetActive(true);
            //animSeq = AnimUtil.StartRotation(RewardAnim.RectTransform);

        }
        else {
            RewardAnim.SetActive(false);
        }
    }

    void OnClickBox() {
        if (score < cfg["needActivePoint"]) {
            LayoutReward.SetActive(!LayoutReward.gameObject.activeSelf);
            return;
        }

        if (rewardData != null)
        {
            //UiMgr.ShowTips("already fetch");
            return;
        }
        else if (!isCanFetch) {
            //UiMgr.ShowTips("not enought active point");
            return;
        }

        //Debug.Log(key);

        JSONObject jsonObj = new JSONObject();
        JSONObject data = new JSONObject();
        jsonObj.Add("data", data);
        data.Add("cfgId", key);

        if (cfg["rewardType"] == 1)
        {
            Cmd.instance.C2S_GET_DAILY_ACT_REWARD(jsonObj);

        }
        else if (cfg["rewardType"] == 2) {
            Cmd.instance.C2S_GET_WEEKLY_ACT_REWARD(jsonObj);
        }
    }

    public void SetRewardShow(bool isShow) {
        LayoutReward.SetActive(isShow);
    }

    protected override void OnRefresh()
    {

    }

    protected override void OnRelease()
    {
        animSeq.Kill();
        animSeq = null;
    }
}
