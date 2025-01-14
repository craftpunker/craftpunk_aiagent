
using Battle;
using SimpleJSON;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class PutSoldierViewItemData {

    public string Id;
    public int Idx;
    public PutSoldierViewDragItem DragItem;
    public PutSoldierView View;

    public JSONNode Cfg;
    public JSONNode SoldierCfg;
}

public class PutSoldierViewItem : ItemBase
{
    U3dObj ImgSoldier;
    //U3dObj TextCount;
    U3dObj TextName;
    U3dObj BgLevel;
    U3dObj Mask;
    U3dObj ImgNew;

    public PutSoldierViewItemData Data;

    bool isDrag = false;
    bool isDraging = false;

    public Action<PutSoldierViewItemData> OnBeginDragPutSoldier;
    public Action<int> OnEndDragPutSoldier;


    protected override void OnInit(params object[] args)
    {
        Data = new PutSoldierViewItemData();

        ImgSoldier = Find("ImgSoldier");
        //TextCount = Find("TextCount");
        TextName = Find("TextName");
        BgLevel = Find("BgLevel");
        Mask = Find("Mask");
        Mask.SetActive(false);
        ImgNew = Find("ImgNew");
        //SetOnPointerDown(item.gameObject, OnPointerDown);
        //SetOnPointerUp(item.gameObject, OnPointerUp);


        SetOnClick(item.gameObject, OnClick);
        SetLongPress(item.gameObject, OnLongPress, 0.2f);
        SetBeginDrag(item.gameObject, OnBeginDrag, "ui_soldier_toBattle");
        SetDrag(item.gameObject, OnDrag);
        SetEndDrag(item.gameObject, OnEndDrag);
        SetOnPointerUp(item.gameObject, OnPointerUp);
        //item.gameObject.AddComponent<UIDragHandler>();
    }

    public void ShowMask(bool value)
    {
        Mask.SetActive(value);
    }

    public void SetData()
    {
        Refresh();
    }

    bool[] _SpriteLoadFinishList;
    protected override void OnRefresh()
    {

        _SpriteLoadFinishList = new bool[] { false, false };
        ImgSoldier.SetSprite("SoldierIconAtlas", Data.Cfg["icon"], true, () => {
            _SpriteLoadFinishList[0] = true;
        });

        TextName.Text.text = Data.Cfg["name"];
        item.SetSprite("CommonAtlas", $"ui_Soldier_quality_{Data.Cfg["quality"]}", true, () => {
            _SpriteLoadFinishList[1] = true;
        });

        BgLevel.SetActive(false);
        ImgNew.SetActive(false);
        ShowMask(true);

        //该兵种有未上阵
        if (Data.Id != null)
        {
            ShowMask(false);
            var level = GameData.instance.PermanentData["troop_info"][Data.Id]["level"].AsInt;
            int count = 0;
            var soldierBattleTable = GameData.instance.TableJsonDict["SoldierBattleConf"];
            
            foreach (var data in soldierBattleTable)
            {
                if (data.Value["cfgId"] == Data.Cfg["cfgId"].AsInt && data.Value["level"] == level)
                {
                    count = data.Value["count"];
                }
            }
            //TextCount.Text.text = count.ToString();
            BgLevel.Text.text = level.ToString();
            BgLevel.SetActive(true);
            return;
        }
        else //兵种已全部上阵 || 没有该兵卡
        {
            int cfgid = Data.Cfg["cfgId"];
            var troop_infos = GameData.instance.PermanentData["troop_info"];

            int value = 0; //1 兵种已全部上阵；0 没有该兵卡
            foreach (var info in troop_infos)
            {
                var troop_info_cfgid = info.Value["cfgId"];

                if (troop_info_cfgid == cfgid)
                {
                    value = 1;
                    break;
                }
            }

            if (value == 1)
            {
                ShowMask(false);
            }
            else //没有该兵卡；有碎片 || 无碎片
            {
                var bag_info = GameData.instance.PermanentData["bag_info"];

                var itemCfgid = cfgid + 200000; //碎片ID
                var item = bag_info[itemCfgid.ToString()];

                if (item != null && item["num"] != 0) //有碎片 
                {
                    ImgNew.SetActive(true);
                    ShowMask(false);
                }
                else //无碎片
                {
                    ShowMask(true);
                }
            }
        }
    }

    public bool CheckIsCanGuide() {
        foreach (bool isLoad in _SpriteLoadFinishList)
        {
            if (!isLoad) {
                return false;
            }
        }
        return true;
    }

    void OnPointerUp(PointerEventData data) {
        if (!isDraging) {
            isDraging = false;
            isDrag = false;
            Data.DragItem.SetActive(false);
        }
    }

    public void OnClick() {
        //UiMgr.Open<SoldierLevelUpView>(Data.Cfg);
        int CfgId = Data.Cfg["cfgId"];
        UiMgr.Open<SoldierLevelUpView>(null, CfgId, Data.Id);

    }

    void OnLongPress()
    {
        //Debug.Log($"OnLongPress {idx}");
        isDrag = true;

        Data.DragItem.transform.position = Input.mousePosition;


        Data.DragItem.SetActive(Data.DragItem.transform.position.x > Data.View.transform.position.x);

        Data.DragItem.Data = Data;
        Data.DragItem.RunStep();
    }

    void OnBeginDrag(PointerEventData data)
    {
        EventDispatcher<int>.instance.TriggerEvent(EventName.Scene_ShowSelectTroopRange, -1);
        isDraging = true;

        OnBeginDragPutSoldier(Data);
    }

    void OnDrag(PointerEventData data)
    {
        if (!isDrag) {
            return;
        }

        Vector3 PanelPos = Data.View.transform.position;
        if (data.position.x < PanelPos.x)
        {
            Data.DragItem.gameObject.SetActiveEx(false);
        }
        else {
            Data.DragItem.gameObject.SetActiveEx(true);
            Data.DragItem.transform.position = data.position;
        }
    }

    void OnEndDrag(PointerEventData data)
    {
        isDraging = false;
        isDrag = false;
        Data.DragItem.SetActive(false);

        OnEndDragPutSoldier(Data.Id == null ? 0 : int.Parse(Data.Id));
    }

    protected override void OnRelease()
    {
        Data = null;
    }
}
