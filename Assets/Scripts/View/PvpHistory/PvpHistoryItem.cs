
using Battle;
using SimpleJSON;
using System;
using UnityEngine;

public class PvpHistoryItem : ItemBase
{
    U3dObj ImgResult;
    U3dObj TxtScore;
    U3dObj TxtPoint;
    U3dObj TxtPower;
    U3dObj TxtName;
    U3dObj BtnRevenge;

    U3dObj ImgAd;
    U3dObj TxtAd;
    U3dObj TxtTime;

    private JSONNode recordData;

    protected override void OnInit(params object[] args)
    {
        base.OnInit(args);
        ImgResult = Find("ImgResult");
        TxtScore = Find("TxtScore");
        TxtPoint = Find("TxtPoint");
        TxtPower = Find("TxtPower");
        TxtName = Find("TxtName");
        BtnRevenge = Find("BtnRevenge");
        ImgAd = Find("ImgAd");
        TxtAd = Find("TxtAd");
        TxtTime = Find("TxtTime");
        SetOnClick(BtnRevenge.gameObject, OnBtnRevenge);
    }

    private void OnBtnRevenge()
    {
        JSONObject jsonObj = new JSONObject();
        JSONObject jsonObj1 = new JSONObject();
        jsonObj1.Add("battleId", recordData["battleId"]);
        jsonObj.Add("data", jsonObj1);
        Cmd.instance.C2S_PVP_REVENGE(jsonObj);
    }

    public void SetData(JSONNode data, int index)
    {
        recordData = data;
        Debug.Log(data);

        if (data["result"] == "win")
        {
            ImgResult.SetSprite("PvpAtlas", "Ui_PVP_win");
        }
        else
        {
            ImgResult.SetSprite("PvpAtlas", "Ui_PVP_fail");
        }

        if (data["tp"] == "atk")
        {
            ImgAd.SetSprite("PvpAtlas", "Ui_PVP_attack");
            TxtAd.Text.text = "Attack";
            TxtAd.Text.color = Color.red;
        }
        else
        {
            ImgAd.SetSprite("PvpAtlas", "Ui_PVP_defense");
            TxtAd.Text.text = "Defense";
            TxtAd.Text.color = Color.green;
        }

        //Debug.Log(data["createTick"]);

        //var currTime = GameData.instance.ServerTime + Time.time - GameData.instance.UerrLoginTime;   
        //var dhms = TimeUtils.DHMS(currTime - data["settleTick"]);
        TxtTime.Text.text = data["datetime"];//$"{dhms.Hour}h {dhms.Minute}m {dhms.Second}s";

        int addScore = data["newScore"] - data["oldScore"];
        if (addScore > 0)
        {
            TxtScore.Text.text = $"+{addScore.ToString()}";
            TxtScore.Text.color = Color.green;
        }
        else
        {
            TxtScore.Text.text = $"-{addScore.ToString()}";
            TxtScore.Text.color = Color.red;
        }

        TxtPoint.Text.text = data["enemyScore"];
        TxtPower.Text.text = data["enemyPower"];
        TxtName.Text.text = data["enemyName"];

        if (data["tp"] == "def" && data["result"] == "fail")
        {
            if (data["revenge"] == 0)
            {
                DateTime date1 = DateTime.Parse(data["datetime"]);
                DateTime date2 = DateTime.Now;
                bool isSameMonth = IsSameMonth(date1, date2);
                if (isSameMonth)
                {
                    BtnRevenge.SetActive(true);
                    return;
                }
            }
        }

        BtnRevenge.SetActive(false);
    }

    private bool IsSameMonth(DateTime date1, DateTime date2)
    {
        return date1.Year == date2.Year && date1.Month == date2.Month;
    }
}
