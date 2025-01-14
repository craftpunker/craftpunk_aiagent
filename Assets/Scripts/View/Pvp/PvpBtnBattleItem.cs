
using Battle;
using SimpleJSON;

public class PvpBtnBattleItem : ItemBase
{
    U3dObj imgHonor;
    U3dObj txtPower;
    U3dObj txtScore;
    U3dObj txtId;

    JSONNode Data;

    protected override void OnInit(params object[] args)
    {
        base.OnInit(args);

        imgHonor = Find("ImgHonor");
        txtPower = Find("ImgPower/TxtPower");
        txtScore = Find("ImgScore/TxtScore");
        txtId = Find("TxtId");

        txtId.SetActive(false);

        SetOnClick(gameObject, OnClick);
    }

    private void OnClick()
    {
        GameData.instance.OppsId = Data["id"];
        //EventDispatcher<int>.instance.TriggerEvent(EventName.c2s_StartMatching, -1, Data["id"]);
        JSONObject jsonObj = new JSONObject();
        JSONObject jsonObj1 = new JSONObject();
        jsonObj1.Add("oppId", GameData.instance.OppsId);
        jsonObj.Add("data", jsonObj1);
        Cmd.instance.C2S_CHALLENGE_PVP(jsonObj);
    }

    public void SetData(JSONNode data)
    {
        Data = data;
        int score = data["score"];
        string icon = TableUtils.GetRankIcon(score);
        txtPower.Text.text = data["power"];
        txtScore.Text.text = score.ToString();

        imgHonor.SetSprite("PvpAtlas", icon);

#if UNITY_EDITOR
        txtId.SetActive(true);
        txtId.Text.text = data["id"];
#endif
    }
}