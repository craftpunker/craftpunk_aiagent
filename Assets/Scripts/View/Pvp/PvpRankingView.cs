
using Battle;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvpRankingView : UiBase
{
    U3dObj btnClose;
    U3dObj ranksScrollView;

    Dictionary<GameObject, ItemBase> _ItemList = new Dictionary<GameObject, ItemBase>();

    JSONNode ranksData;

    U3dObj ImgRank;
    U3dObj TxtNumber;
    U3dObj TxtPoint;
    U3dObj TxtPower;
    U3dObj TxtName;
    U3dObj Mask;


    protected override void OnInit()
    {
        btnClose = Find("ImgBg/BtnClose");
        SetOnClick(btnClose, OnBtnClose);

        ImgRank = Find("ImgBg/PvpRankItem/ImgRank");
        TxtNumber = Find("ImgBg/PvpRankItem/TxtNumber");
        TxtPoint = Find("ImgBg/PvpRankItem/TxtPoint");
        TxtPower = Find("ImgBg/PvpRankItem/TxtPower");
        TxtName = Find("ImgBg/PvpRankItem/TxtName");

        Mask = Find("Mask");
        SetOnClick(Mask, OnBtnClose);

        ranksScrollView = Find("ImgBg/RankingScrollView");
        ranksScrollView.LoopScrollView.Init(_ItemList);
        ranksScrollView.LoopScrollView.SetRenderHandler(OnRenderItem);

    }

    protected override void OnShow()
    {
        base.OnShow();
        //Reflash();
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.ranks, (evtName, evt) =>
        {
            ranksData = evt[0].JsonNode;
            Reflash();
            ranksScrollView.LoopScrollView.SetRenderHandler(OnRenderItem);
        });

        JSONObject jsonObj = new JSONObject();
        jsonObj.Add("data", new JSONObject());
        Cmd.instance.C2S_PVP_RANK(jsonObj);

    }

    private void Reflash()
    {
        TxtNumber.Text.text = "--";
        int score = GameData.instance.PermanentData["score"];
        TxtPoint.Text.text = score.ToString();
        TxtPower.Text.text = GameData.instance.PermanentData["power"]; ;
        TxtName.Text.text = GameData.instance.UserData["name"];
        string icon = TableUtils.GetRankIcon(score);
        ImgRank.SetSprite("PvpAtlas", icon);

        int rank = 1;
        foreach (var item in ranksData)
        {
            if (item.Value["id"] == GameData.instance.UserData["uid"])
            {
                TxtNumber.Text.text = rank.ToString();
            }

            rank++;
        }
        ResetScrollViewDataCount();
    }

    public void SetSelfRank(int rank)
    {
        TxtNumber.Text.text = rank.ToString();
    }

    private void ResetScrollViewDataCount()
    {
        ranksScrollView.LoopScrollView.SetDataCount(ranksData.Count);
    }


    private void OnRenderItem(GameObject obj, int index)
    {
        PvpRankItem item = ItemBase.GetItem<PvpRankItem>(obj, _ItemList, this);
        var data = ranksData[index];
        item.View = this;
        item.SetData(data, index);
    }

    void OnBtnClose()
    {
        UiMgr.Close<PvpRankingView>();
    }

    protected override void OnHide()
    {
        EventDispatcher<CmdEventData>.instance.RemoveEventByName(EventName.ranks);
        base.OnHide();
    }
}
