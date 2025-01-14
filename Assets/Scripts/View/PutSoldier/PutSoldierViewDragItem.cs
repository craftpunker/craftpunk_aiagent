
using Battle;
using UnityEngine;

public class PutSoldierViewDragItem : ItemBase
{
    //bool isDrag = false;
    public PutSoldierViewItemData Data;
    //int idx;

    U3dObj ImgSoldier;
    U3dObj TextName;
    U3dObj BgLevel;
    U3dObj LayoutArmy;

    protected override void OnInit(params object[] args)
    {
        Data = new PutSoldierViewItemData();

        ImgSoldier = Find("ImgSoldier");
        TextName = Find("TextName");
        BgLevel = Find("BgLevel");
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
        ImgSoldier.SetSprite("SoldierIconAtlas", Data.Cfg["icon"], true);;
        TextName.Text.text = Data.Cfg["name"];
        //var level = GameData.instance.PermanentData["troop_info"][Data.Id]["level"].AsInt;
        BgLevel.Text.text = Data.SoldierCfg["level"];
        item.SetSprite("CommonAtlas", $"ui_Soldier_quality_{Data.Cfg["quality"]}", true);
    }
}
