
using Battle;
using SimpleJSON;
using UnityEngine;

public class PvpRankItem : ItemBase
{
    U3dObj ImgRank;
    U3dObj TxtNumber;
    U3dObj TxtPoint;
    U3dObj TxtPower;
    U3dObj TxtName;

    JSONNode Data;

    public int Id;
    public PvpRankingView View;

    Color selfColor = new Color(0.9686275f, 0.682353f, 0.1137255f);

    protected override void OnInit(params object[] args)
    {
        base.OnInit(args);

        ImgRank = Find("ImgRank");
        TxtNumber = Find("TxtNumber");
        TxtPoint = Find("TxtPoint");
        TxtPower = Find("TxtPower");
        TxtName = Find("TxtName");
    }

    public void SetData(JSONNode data, int index)
    {
        Data = data;
        int score = data["score"];
        string icon = TableUtils.GetRankIcon(score);
        ResetColor();
        ImgRank.SetSprite("PvpAtlas", icon);

        TxtNumber.Text.text = (index + 1).ToString();
        TxtPoint.Text.text = score.ToString();
        TxtPower.Text.text = data["power"];
        TxtName.Text.text = data["name"];
        if (data["id"] == GameData.instance.UserData["uid"])
        {
            SelfColor();
        }
    }

    private void ResetColor()
    {
        TxtNumber.Text.color = Color.white;
        TxtPoint.Text.color = Color.white;
        TxtPower.Text.color = Color.white;
        TxtName.Text.color = Color.white;
    }

    private void SelfColor()
    {
        TxtNumber.Text.color = selfColor;
        TxtPoint.Text.color = selfColor;
        TxtPower.Text.color = selfColor;
        TxtName.Text.color = selfColor;
    }
}
