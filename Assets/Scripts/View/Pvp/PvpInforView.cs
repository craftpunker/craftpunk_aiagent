using Battle;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PvpInforView : UiBase
{
    U3dObj BtnClose;
    U3dObj BtnRule;
    U3dObj BtnReward;
    U3dObj LayoutRand;
    U3dObj LayoutRule;
    U3dObj Mask;

    U3dObj RewardScrollView;

    Dictionary<GameObject, ItemBase> _ItemList = new Dictionary<GameObject, ItemBase>();

    JSONObject pvpStageConf;
    JSONNode curConf = null;

    protected override void OnInit()
    {
        BtnClose = Find("ImgBg/BtnClose");
        SetOnClick(BtnClose, OnBtnClose);

        BtnRule = Find("ImgBg/ToggleGroup/BtnRule");
        SetOnClick(BtnRule.gameObject, OnBtnRule);

        BtnReward = Find("ImgBg/ToggleGroup/BtnReward");
        SetOnClick(BtnReward.gameObject, OnBtnReward);

        LayoutRand = Find("ImgBg/LayoutRand");
        LayoutRule = Find("ImgBg/LayoutRule");

        Mask = Find("Mask");
        SetOnClick(Mask, OnBtnClose);

        pvpStageConf = new JSONObject();
        var orderTabletable = GameData.instance.TableJsonDict["PvpStageConf"].Linq.OrderByDescending(k => int.Parse(k.Key));
        foreach (var item in orderTabletable)
        {
            pvpStageConf.Add(item.Key,item.Value);
        }
        RewardScrollView = Find("ImgBg/LayoutRand/RewardScrollView");
        RewardScrollView.LoopScrollView.Init(_ItemList);
        RewardScrollView.LoopScrollView.SetRenderHandler(OnRenderItem);
        RewardScrollView.LoopScrollView.SetDataCount(pvpStageConf.Count);
    }

    protected override void OnShow()
    {
        int scroe = GameData.instance.PermanentData["score"];
        for (int i = 0; i < pvpStageConf.Count; i++)
        {
            if (scroe >= pvpStageConf[i]["scroe"][0]&& scroe <= pvpStageConf[i]["scroe"][1])
            {
                curConf = pvpStageConf[i];
                break;
            }
        }
        OnBtnReward();
    }


    void OnBtnClose()
    {
        UiMgr.Close<PvpInforView>();
    }

    void OnBtnRule()
    {
        LayoutRule.SetActive(true);
        LayoutRand.SetActive(false);
    }

    void OnBtnReward()
    {
        LayoutRule.SetActive(false);
        LayoutRand.SetActive(true);
    }

    void OnRenderItem(GameObject obj, int index)
    {
        PvpInforItem item = ItemBase.GetItem<PvpInforItem>(obj, _ItemList, this);
        //item.SetData(pvpStageConf[pvpStageConf.Count - index]);
        item.SetData(pvpStageConf[index]);
    }
}
