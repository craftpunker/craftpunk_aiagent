using Battle;
using UnityEngine;

public class TopShowResView : UiBase
{
    U3dObj LayoutGold;
    U3dObj LayoutDiamonds;
    U3dObj BtnBuyGold;

    U3dObj ImgGold;
    U3dObj ImgDiamonds;
    U3dObj BtnBuyDiamonds;


    protected override void OnInit()
    {
        LayoutGold = Find("LayoutGold");
        ImgGold = LayoutGold.Find("ImgGold");
        BtnBuyGold = LayoutGold.Find("BtnBuyGold");
        SetOnClick(BtnBuyGold, OnBtnBuyGold);

        LayoutDiamonds = Find("LayoutDiamonds");
        ImgDiamonds = LayoutDiamonds.Find("ImgDiamonds");
        BtnBuyDiamonds = LayoutDiamonds.Find("BtnBuyDiamonds");
        SetOnClick(BtnBuyDiamonds, OnBtnBuyDiamonds);
    }


    void OnBtnBuyGold() {
        UiMgr.Open<ShopView>(null, 5);
    }

    void OnBtnBuyDiamonds()
    {
        UiMgr.Open<ShopView>(null, 4);
    }

    private void ChangeBaginfo(string evtName, CmdEventData[] args)
    {
        var gold = GameData.instance.PermanentData["bag_info"]["100001"]["num"];
        LayoutGold.Text.text = gold == null ? "0" : gold;

        var diamonds = GameData.instance.PermanentData["bag_info"]["100002"]["num"];
        LayoutDiamonds.Text.text = diamonds == null ? "0" : diamonds;
    }

    protected override void OnShow()
    {
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.bag_info, ChangeBaginfo);

        var gold = GameData.instance.PermanentData["bag_info"]["100001"]["num"];
        LayoutGold.Text.text = gold == null ? "0" : gold;

        var diamonds = GameData.instance.PermanentData["bag_info"]["100002"]["num"];
        LayoutDiamonds.Text.text = diamonds == null ? "0" : diamonds;
    }

    public Vector3 GetGoldPos() {
        return ImgGold.transform.position;
    }

    public Vector3 GetDiamondsPos()
    {
        return ImgDiamonds.transform.position;
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.bag_info, ChangeBaginfo);
    }

    protected override void OnDestroy()
    {

    }
}
