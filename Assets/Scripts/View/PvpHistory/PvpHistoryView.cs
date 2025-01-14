

using Battle;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Linq;

public class PvpHistoryView : UiBase
{
    U3dObj BtnClose;
    U3dObj HistoryScrollView;
    U3dObj Mask;

    Dictionary<GameObject, ItemBase> _ItemList = new Dictionary<GameObject, ItemBase>();

    protected override void OnInit()
    {
        base.OnInit();

        BtnClose = Find("ImgBg/BtnClose");
        SetOnClick(BtnClose, OnBtnClose);

        HistoryScrollView = Find("ImgBg/HistoryScrollView");
        HistoryScrollView.LoopScrollView.Init(_ItemList);
        HistoryScrollView.LoopScrollView.SetRenderHandler(OnRenderItem);
        Mask = Find("Mask");
        SetOnClick(Mask, OnBtnClose);
    }

    protected override void OnShow()
    {
        base.OnShow();

        List<KeyValuePair<string, JSONNode>> dict = new List<KeyValuePair<string, JSONNode>>();
        foreach (var subData in GameData.instance.RecordsData)
        {
            dict.Add(new KeyValuePair<string, JSONNode>(subData.Value["settleTick"], subData.Value));
        }

        var sortedDictionary = dict.OrderByDescending(x => x.Key).ToList();
        GameData.instance.RecordsData.Clear();

        foreach (var subData in sortedDictionary)
        {
            GameData.instance.RecordsData.Add(subData.Value["battleId"], subData.Value);
        }

        Reflash();
    }

    private void Reflash()
    {
        HistoryScrollView.LoopScrollView.SetDataCount(GameData.instance.RecordsData.Count);

        MonoTimeMgr.instance.SetTimeAction(2, () =>
        {
            foreach (var subData in GameData.instance.RecordsData.Values)
            {
                if (subData["read"] != 0)
                {
                    continue;

                }
                JSONObject jsObj = new JSONObject();
                jsObj.Add("cmd", "C2S_PVP_RECORD_READ");
                JSONObject data = new JSONObject();
                data.Add("battleId", subData["battleId"]);
                jsObj.Add("data", data);

                WebSocketMain.instance.SendWebSocketMessage(jsObj);
            }

        });


    }

    private void OnRenderItem(GameObject obj, int index)
    {
        PvpHistoryItem item = ItemBase.GetItem<PvpHistoryItem>(obj, _ItemList, this);
        var data = GameData.instance.RecordsData[index];
        item.SetData(data, index);
    }

    public void OnBtnClose()
    {
        UiMgr.Close<PvpHistoryView>();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }
}
