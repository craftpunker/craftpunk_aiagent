
using UnityEngine;

public class SoldierLevelArmyDragItem : ItemBase
{
    //bool isDrag = false;
    public SoldierLevelArmyItemData Data;
    //int idx;

    U3dObj ImgIcon;
    U3dObj TxtLevel;
    U3dObj LayoutArmy;
    

    protected override void OnInit(params object[] args)
    {
        Data = new SoldierLevelArmyItemData();

        ImgIcon = Find("LayoutArmy/Icon");
        TxtLevel = Find("LayoutArmy/TxtLevel");
        LayoutArmy = Find("LayoutArmy");

        Find("LayoutArmy/ImgSelectFrame").SetActive(false);

        //ImgSoldier = Find("ImgSoldier");
        //TextCount = Find("TextCount");
        //TextName = Find("TextName");
        //BgLevel = Find("BgLevel");
        //SetOnPointerDown(item.gameObject, OnPointerDown);
        //SetOnPointerUp(item.gameObject, OnPointerUp);


        //SetLongPress(item.gameObject, OnLongPress, 0.5f);
        //SetDrag(item.gameObject, OnDrag);
        //SetEndDrag(item.gameObject, OnEndDrag);
        ////item.gameObject.AddComponent<UIDragHandler>();

    }
    protected override void OnRelease()
    {
        Data = null;
    }

    protected override void OnRefresh()
    {
        ImgIcon.SetSprite("SoldierIconAtlas", Data.soldierCfg["icon"], true);
        TxtLevel.Text.text = Data.Cfg["level"];
        LayoutArmy.SetSprite("CommonAtlas", $"ui_Soldier_Mini_quality_{Data.soldierCfg["quality"]}", true);
    }
}
