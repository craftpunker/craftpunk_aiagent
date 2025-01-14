using Battle;
using BattleEditor;
using SimpleJSON;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SoldierLevelArmyItemData
{

    public string Id;
    public int Idx;
    public SoldierLevelArmyDragItem DragItem;
    public SoldierLevelUpView View;

    public JSONNode Cfg;
    public JSONNode soldierCfg;

}

public class SoldierLevelArmyItem : ItemBase
{
    SoldierLevelUpView view;

    int soldierCfgId;
    JSONNode soldierCfg;
    public JSONNode TroopInfo;

    public U3dObj LayoutAdd;
    public U3dObj LayoutArmy;
    U3dObj Icon;

    U3dObj TxtLevel;
    U3dObj ImgDeployed;

    U3dObj LayoutAddIcon;

    U3dObj ImgPlus;
    U3dObj ImgFragmentBG;
    Slider SliderFragment;
    Text TxtGold;
    Text TxtItemCount;

    bool isDrag = false;
    bool isDraging = false;

    public Action<SoldierLevelArmyItemData> OnBeginDragPutSoldier;
    public Action<int> OnEndDragPutSoldier;

    public SoldierLevelArmyItemData Data;

    public bool isLayoutAdd;

    public GameObject ImgSelectFrame;

    //private UISpecialEffect uiRange;

    protected override void OnInit(params object[] args)
    {
        SetOnClick(gameObject, OnClickItem);
        Data = new SoldierLevelArmyItemData();

        LayoutArmy = Find("LayoutArmy");
        LayoutAdd = Find("LayoutAdd");
        Icon = LayoutArmy.Find("Icon");
        TxtLevel = LayoutArmy.Find("TxtLevel");
        ImgDeployed = LayoutArmy.Find("ImgDeployed");
        LayoutAddIcon = LayoutAdd.Find("Icon");
        ImgPlus = LayoutAdd.Find("ImgPlus");
        ImgFragmentBG = LayoutAdd.Find("ImgFragmentBG");
        SliderFragment = ImgFragmentBG.Find("Slider").Slider;
        TxtGold = ImgFragmentBG.Find("TxtCount").Text;
        TxtItemCount = SliderFragment.transform.Find("TxtItemCount").GetComponent<Text>();

        ImgDeployed.SetActive(false);

        SetLongPress(item.gameObject, OnLongPress, 0.2f);
        SetBeginDrag(item.gameObject, OnBeginDrag, "ui_soldier_toBattle");
        SetDrag(item.gameObject, OnDrag);
        SetEndDrag(item.gameObject, OnEndDrag);
        SetOnPointerUp(item.gameObject, OnPointerUp);

        SetOnClick(LayoutAdd.gameObject, OnLayoutAdd);

        ImgSelectFrame = LayoutArmy.Find("ImgSelectFrame").gameObject;
        ImgSelectFrame.SetActive(false);
        //uiRange = LayoutArmy.Find("GPUUIEffect_1").transform.GetComponent<UISpecialEffect>();

        //EventDispatcher<TroopsMono>.instance.AddEvent(EventName.Scene_CloseTroopRange, Scene_CloseTroopRange);
        EventDispatcher<int>.instance.AddEvent(EventName.Scene_ShowSelectTroopRange, Scene_ShowSelectTroopRange);
    }

    public void OnLayoutAdd()
    {
        var troop_info = GameData.instance.PermanentData["troop_info"];
        var dataCount = 0;
        foreach (var troop in troop_info)
        {
            if (troop.Value["cfgId"] == soldierCfg["cfgId"])
            {
                dataCount++;
            }
        }

        var maximum = soldierCfg["maximum"];

        if (dataCount >= maximum)
        {
            UiMgr.ShowTips("Exceeding the upper limit");
            return;
        }

        var cfgid = soldierCfg["cfgId"];
        var battleSoldierData = TableUtils.FindSoldierBattleConfByCfgidLevel(cfgid, 0);
        var nextLevelNeedItemNum = battleSoldierData["levelUpNeedItem"][1][1];
        var nextLevelNeedItemId = battleSoldierData["levelUpNeedItem"][1][0];
        var fragNumData = GameData.instance.PermanentData["bag_info"][nextLevelNeedItemId.ToString()];

        if (!LayoutAddIcon.gameObject.activeSelf)
        {
            LayoutAddIcon.SetActive(true);
            LayoutAddIcon.SetSprite("SoldierIconAtlas", soldierCfg["icon"], true);
            ImgPlus.SetActive(false);
            ImgFragmentBG.SetActive(true);

            TxtGold.text = battleSoldierData["levelUpNeedItem"][0][1];
            SliderFragment.value = fragNumData["num"].AsFloat / nextLevelNeedItemNum.AsFloat;
            if (fragNumData == null)
            {
                TxtItemCount.text = $"{nextLevelNeedItemNum} / 0";
                ImgFragmentBG.SetImgGray(true);
            }
            else
            {
                TxtItemCount.text = $"{nextLevelNeedItemNum} / {fragNumData["num"]}";

                ImgFragmentBG.SetImgGray(nextLevelNeedItemNum > fragNumData["num"]);
            }
        }
        else
        {
            if (nextLevelNeedItemNum > fragNumData["num"])
                return;

            JSONObject obj = new JSONObject();
            JSONObject obj1 = new JSONObject();
            obj.Add("data", obj1);
            obj1.Add("cfgId", soldierCfgId);
            Cmd.instance.C2S_CREATE_TROOP(obj);
        }
    }

    //private void Scene_CloseTroopRange(string evtName, TroopsMono[] args)
    //{
    //    //ImgSelected.SetActive(false);
    //    if (ImgSelected != null)
    //    {
    //        Debug.Log("Scene_CloseTroopRange Close");
    //        ImgSelected.BKilled = true;
    //        ImgSelected = null;
    //    }
    //}

    public void OnClickItem()
    {
        if (GuideMgr.guideNodeList.Count > 0) {
            foreach (var item in GuideMgr.guideNodeList)
            {
                if (item.IsDragSoldier && item.stage == GuideNode.Stage.Guiding) {
                    return;
                }
            }
        }

        view.CurrSelectedArmyItem = this;
        view.StartLevelUp(TroopInfo);

        GameData.instance.SelectedTroop = null;

        if (GameData.instance.TroopsMonos.TryGetValue(TroopInfo["id"], out var troopMono))
        {
            GameData.instance.SelectedTroop = troopMono;
        }
        EventDispatcher<int>.instance.TriggerEvent(EventName.Scene_ShowSelectTroopRange, TroopInfo["id"]);
    }

    public void SetData(SoldierLevelUpView view) {
        LayoutAddIcon.SetActive(false);
        //uiRange.SetActiveEx(false);
        ImgPlus.SetActive(true);
        ImgFragmentBG.SetActive(false);
        this.view = view;
        soldierCfgId = view.SoldierCfgId;
        soldierCfg = view.soldierCfg;
        Refresh();
    }


    void OnLongPress()
    {

        if (isLayoutAdd)
            return;

        if (ImgDeployed.gameObject.activeSelf)
            return;

        isDrag = true;

        Data.DragItem.transform.position = Input.mousePosition;


        Data.DragItem.SetActive(Data.DragItem.transform.position.x > Data.View.transform.position.x);

        Data.DragItem.Data = Data;
        Data.DragItem.RunStep();
    }

    void OnBeginDrag(PointerEventData data)
    {
        if (isLayoutAdd)
            return;

        if (ImgDeployed.gameObject.activeSelf)
            return;

        EventDispatcher<int>.instance.TriggerEvent(EventName.Scene_ShowSelectTroopRange, -1);

        isDraging = true;
        OnBeginDragPutSoldier(Data);
    }

    void OnDrag(PointerEventData data)
    {

        if (isLayoutAdd)
            return;

        if (!isDrag)
        {
            return;
        }

        if (ImgDeployed.gameObject.activeSelf)
            return;

        Vector3 PanelPos = Data.View.transform.position;
        if (data.position.x < PanelPos.x)
        {
            Data.DragItem.gameObject.SetActiveEx(false);
        }
        else
        {
            Data.DragItem.gameObject.SetActiveEx(true);
            Data.DragItem.transform.position = data.position;
        }
    }

    void OnEndDrag(PointerEventData data)
    {

        if (isLayoutAdd)
            return;

        if (ImgDeployed.gameObject.activeSelf)
            return;

        isDraging = false;
        isDrag = false;
        Data.DragItem.SetActive(false);

        OnEndDragPutSoldier(Data.Id == null ? 0 : int.Parse(Data.Id));
    }

    void OnPointerUp(PointerEventData data)
    {
        if (isLayoutAdd)
            return;

        if (!isDraging)
        {
            isDraging = false;
            isDrag = false;
            Data.DragItem.SetActive(false);
        }
    }

    private void Scene_ShowSelectTroopRange(string evtName, int[] args)
    {
        if (TroopInfo == null)
            return;

        int id = args[0];
        int troopId = TroopInfo["id"];

        //if(uiRange != null)
        //    uiRange.SetActiveEx(false);

        if(ImgSelectFrame != null)
            ImgSelectFrame.SetActiveEx(false);

        if (id == troopId)
        {
            PlayRange();
        }
    }

    private void PlayRange()
    {

        ImgSelectFrame.SetActiveEx(true);
        //if(uiRange != null)
        //{
        //    uiRange.DoAnim(40, 50, 1, -1, null);
        //    uiRange.SetActiveEx(true);
        //}
    }

    protected override void OnRelease()
    {
        //EventDispatcher<int>.instance.RemoveEvent(EventName.Scene_ShowSelectTroopRange, Scene_ShowSelectTroopRange);

    }


    bool isSpriteLoad = false;
    protected override void OnRefresh()
    {
        if (isLayoutAdd)
            return;

        isSpriteLoad = false;
        Icon.SetSprite("SoldierIconAtlas", soldierCfg["icon"], true, () => {
            isSpriteLoad = true;
        });
        TxtLevel.Text.text = TroopInfo["level"];
        LayoutArmy.SetSprite("CommonAtlas", $"ui_Soldier_Mini_quality_{Data.soldierCfg["quality"]}", true);

        var battle_troop = GameData.instance.PermanentData["battle_troop"];
        string id = TroopInfo["id"].ToString();
        if (battle_troop[id] != null)
        {
            var battleTroopData = battle_troop[id];
            ImgDeployed.SetActive(true);
        }
        else
        {
            ImgDeployed.SetActive(false);
        }

        //uiRange.SetActiveEx(false);

        if (GameData.instance.SelectedTroop?.Id == TroopInfo["id"])
        {
            PlayRange();
        }
        //EventDispatcher<int>.instance.TriggerEvent(EventName.Scene_ShowSelectTroopRange, int.Parse(id));
    }

    public bool CheckIsCanDrag() {
        return !ImgDeployed.gameObject.activeSelf && !isLayoutAdd && isSpriteLoad;
    }
}
