using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerChestView : UiBase
{
    U3dObj BtnClose;

    U3dObj ChestScrollView;

    U3dObj LayoutRewardInfo;

    JSONNode PowerChestConf;

    Dictionary<GameObject, ItemBase> _ItemList = new Dictionary<GameObject, ItemBase>();

    U3dObj LayoutPower;
    U3dObj txtPower;

    U3dObj BtnReturn;
    U3dObj BtnNextSpecial;
    U3dObj ImgBg;

    protected override void OnInit()
    {
        //Find
        BtnClose = Find("BgPowerChest/BtnClose");
        SetOnClick(BtnClose, OnBtnClose);
        PowerChestConf = GameData.instance.TableJsonDict["PowerChestConf"];

        LayoutPower = Find("BgPowerChest/ChestScrollView/Viewport/LayoutPower");
        txtPower = LayoutPower.Find("txtPower");

        ChestScrollView = Find("BgPowerChest/ChestScrollView");
        ChestScrollView.LoopScrollView.Init(_ItemList);
        ChestScrollView.LoopScrollView.SetRenderHandler(OnRenderItem);
        ChestScrollView.LoopScrollView.SetDataCount(PowerChestConf.Count);

        BtnReturn = Find("BgPowerChest/BtnReturn");
        SetOnClick(BtnReturn, OnBtnReturn);
        BtnNextSpecial = Find("BgPowerChest/BtnNextSpecial");
        SetOnClick(BtnNextSpecial, OnBtnNextSpecial);

        ImgBg = Find("ImgBg");
        SetOnClick(ImgBg, OnBtnClose);
    }

    protected override void OnShow()
    {
        EventDispatcher<CmdEventData>.instance.AddEvent("power_rwd", OnPower_rwd);
        EventDispatcher<CmdEventData>.instance.AddEvent("power", OnPowerChange);
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.power_reward, OnPowerReward);
        Refresh(true);
    }


    int nowCfgId;
    int highestCfgId;
    void Refresh(bool isResetPos)
    {

        float curPower = GameData.instance.PermanentData["power"];
        txtPower.Text.text = curPower.ToString();

        nowCfgId = 1;
        highestCfgId = 0;

        bool isFindNowCfgId = false;

        JSONNode powerRwdData = GameData.instance.PermanentData["power_rwd"];

        foreach (var item in PowerChestConf.Values)
        {
            if (curPower >= item["needPower"])
            {
                int cfgId = item["cfgId"];

                if (powerRwdData != null)
                {
                    if (powerRwdData[cfgId.ToString()] == null)
                    {
                        nowCfgId = cfgId;
                        isFindNowCfgId = true;
                    }

                    if (!isFindNowCfgId)
                    {
                        nowCfgId = cfgId;
                    }
                }

                highestCfgId = cfgId;
            }
        }


        highestCfgId += 1;

        foreach (var item in _ItemList.Values)
        {
            item.Refresh();
        }

        if (isResetPos)
        {
            ChestScrollView.LoopScrollView.Jump2DataIndex(getIndexByCfgId(nowCfgId));
        }
    }

    void OnPower_rwd(string name, CmdEventData[] datas)
    {
        Refresh(false);

    }

    void OnPowerChange(string name, CmdEventData[] datas)
    {
        txtPower.Text.text = GameData.instance.PermanentData["power"].ToString();
    }

    void OnPowerReward(string name, CmdEventData[] datas)
    {
        JSONNode data = datas[0].JsonNode;
        //txtPower.Text.text = GameData.instance.PermanentData["power"].ToString();

        foreach (var item in _ItemList.Values)
        {
            PowerChestItem powerChestItem = (PowerChestItem)item;
            if (powerChestItem.cfg["cfgId"].ToString() == data["cfgId"])
            {
                ResAnim.instance.ShowUIAnim(data["rewardItems"], powerChestItem.BtnClaim.transform.position);
            }
        }
    }

    void OnBtnReturn()
    {
        ChestScrollView.LoopScrollView.Scroll2DataIndex(getIndexByCfgId(nowCfgId), 0.5f);
    }

    void OnBtnNextSpecial()
    {
        //ResAnim.instance.ShowUIAnim(ResAnim.ResType.Diamonds, BtnNextSpecial.transform.position, 1000);
        foreach (var item in PowerChestConf.Values)
        {
            if (item["cfgId"] > nowCfgId && item["isSpecial"] == 1)
            {
                ChestScrollView.LoopScrollView.Scroll2DataIndex(getIndexByCfgId(item["cfgId"]), 0.5f);
                break;
            }
        }
    }

    int getIndexByCfgId(int cfgId)
    {
        return PowerChestConf.Count - cfgId + 1;
    }

    protected override void OnUpdate()
    {
        UpdatePowerPos();
    }

    void UpdatePowerPos()
    {
        foreach (var item in _ItemList.Values)
        {
            PowerChestItem powerChestItem = (PowerChestItem)item;
            if (powerChestItem.cfg["cfgId"] == highestCfgId)
            {
                LayoutPower.transform.SetActiveEx(true);
                LayoutPower.transform.position = powerChestItem.GetSliderPos();
                return;
            }
        }

        LayoutPower.transform.SetActiveEx(false);
    }

    protected override void OnHide()
    {
        EventDispatcher<CmdEventData>.instance.RemoveEvent("power_rwd", OnPower_rwd);
        EventDispatcher<CmdEventData>.instance.RemoveEvent("power", OnPowerChange);
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.power_reward, OnPowerReward);
    }


    protected override void OnDestroy()
    {

    }

    void OnBtnClose()
    {
        UiMgr.Close<PowerChestView>();
    }


    void OnRenderItem(GameObject obj, int index)
    {
        PowerChestItem item = ItemBase.GetItem<PowerChestItem>(obj, _ItemList, this);
        item.SetData(PowerChestConf[PowerChestConf.Count - index - 1]);
    }
}
