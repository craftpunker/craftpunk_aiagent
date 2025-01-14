using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGiftItem : ItemBase
{
    public string cfgId;

    U3dObj TxtTitle;
    U3dObj ImgDiscount;

    U3dObj TxtCount;
    U3dObj TxtLessTime;

    U3dObj ImgRight;

    public U3dObj ImgGoods;
    U3dObj TxtPrice;

    U3dObj BtnInfo;

    protected override void OnInit(params object[] args)
    {
        TxtTitle = Find("TxtTitle");
        ImgDiscount = Find("ImgDiscount");
        TxtCount = Find("TxtCount");
        TxtLessTime = Find("TxtLessTime");
        ImgRight = Find("ImgRight");
        ImgGoods = Find("ImgGoods");
        TxtPrice = Find("TxtPrice");
        BtnInfo = Find("BtnInfo");
        
        SetOnClick(BtnInfo, OnClickInfo);

        SetOnClick(gameObject, OnClickItem);
    }

    JSONNode cfg;
    JSONNode data;

    public void SetData(string cfgId)
    {
        this.cfgId = cfgId;
        cfg = GameData.instance.TableJsonDict["RechargeConf"][cfgId];
        data = GameData.instance.PermanentData["rhg_record"][cfgId];

        Refresh();
    }

    protected override void OnRefresh()
    {
        if (data["less"] > 0)
        {
            TxtLessTime.SetActive(true);
        }
        else {
            TxtLessTime.SetActive(false);
        }

        TxtPrice.Text.text = "$" + (float)cfg["price"] / 100;
        TxtTitle.Text.text = cfg["name"];
        ImgDiscount.Text.text = cfg["benefit"] + "%";

        int buyCount = cfg["buyCount"];
        TxtCount.Text.text = data["cnt"] + "/" + buyCount;
    }

    void OnClickInfo()
    {
        //Debug.Log(cfg["rewardItem"]);
        JSONNode rewardCfg = cfg["rewardItem"];
        List<PopUpReceiveView.ItemData> dataList = new List<PopUpReceiveView.ItemData>();

        for (int i = 0; i < rewardCfg.Count; i++)
        {
            JSONNode subCfg = rewardCfg[i];
            dataList.Add(new PopUpReceiveView.ItemData() { ItemCfgId = subCfg[0], count = subCfg[1] });
        }

        UiMgr.Open<PopUpReceiveView>(null, dataList);
    }

    void OnClickItem() {
        ShopData.GetRecord(cfgId);
    }

    public void Update() {
        if (data["less"] > 0) {
            float less = ShopData.DictLessTimeRecord[cfgId] - Time.time;
            TimeUtils.TimeHMS time = TimeUtils.DHMS(less);
            TxtLessTime.Text.text = $"{time.Day}Day {time.Hour}:{time.Minute}:{time.Second}";
        }
    }

    protected override void OnRelease()
    {

    }
}
