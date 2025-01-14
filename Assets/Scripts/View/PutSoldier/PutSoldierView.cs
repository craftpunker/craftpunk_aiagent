using Battle;
using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

//��¼��糡���϶�ʿ����ģ��
public class DragSoldier
{
    private int index;
    private Vector2 posOffset;
    private JSONNode jsonNode;
    public SpecialEffect SpecialEffect;

    public void Init(int index, JSONNode jsonNode)
    {
        this.index = index;
        this.jsonNode = jsonNode;

        posOffset = GameUtils.GetTroosEntityPos(index, jsonNode["count"], PlayerGroup.Player).ToVector2();
        SpecialEffect = SpecialEffectFactory.instance.CreateSpecialEffect(jsonNode["animCfgId"], FixVector3.Zero, (go) =>
        {
            go.GameObj.SetActiveEx(false);
        });
    }

    public void UpdatePos(Vector2 pos)
    {
        if (SpecialEffect.Trans != null)
        {
            SpecialEffect.GameObj.SetActiveEx(true);
            SpecialEffect.Trans.position = pos + posOffset;
        }
    }

    public void Release()
    {
        jsonNode = null;
        //BattleMgr.instance.SpecialEffectList.Remove(SpecialEffect);
        SpecialEffect.BKilled = true;
       // SpecialEffect.Release();
        SpecialEffect = null;
        ClassPool.instance.Push(this);
    }
}

public class PutSoldierView : UiBase
{
    public U3dObj btnClose;

    U3dObj soldierScrollView;
    Dictionary<GameObject, ItemBase> _ItemList = new Dictionary<GameObject, ItemBase>();
    PutSoldierViewDragItem dragItem;

    JSONNode soldierCfg = GameData.instance.TableJsonDict["SoldierConf"];

    public float rayLength = 100f;
    int layerMask;
    private PhysicsRayMono CurrPhysicsRayMono; //���߻���ʵ��
    public bool IsDragUI;
    public Transform UIDragSoliderTrans; //�ϱ�ʵ��
    private Vector3 dragSoliderhitPos;

    private List<DragSoldier> dragSoldierList = new List<DragSoldier>();
    // ActionSetSoldierPos(pos, cfgId, count)

    private RaycastHit hit;

    //U3dObj layoutGoldTxt;
    //U3dObj layoutDiamondsTxt;

    private int dataCount;

    private SortedDictionary<int, JSONNode> soldierCfgs = new SortedDictionary<int, JSONNode>();
    private List<JSONNode> soldierCfgList = new List<JSONNode>();

    protected override void OnInit()
    {
        soldierScrollView = Find("BgSoldiers/SoldierScrollView");
        soldierScrollView.LoopScrollView.Init(_ItemList);
        soldierScrollView.LoopScrollView.SetRenderHandler(OnRenderItem);
        btnClose = Find("BgSoldiers/BtnClose"); //new U3dObj(Panel.Find("BgSoldiers/BtnClose"));
        SetOnClick(btnClose.gameObject, OnClickClose);

        dragItem = ItemBase.GetItem<PutSoldierViewDragItem>(FindGameObject("PutSoldierViewItem"));
        dragItem.transform.SetActiveEx(false);

        //layoutGoldTxt = Find("LayoutGold/TxtGold");
        //layoutDiamondsTxt = Find("LayoutDiamonds/TxtDiamonds");
    }

    protected override void OnShow()
    {
        BoardMgr.instance.ShowBoard(true);
        soldierCfgs.Clear();
        foreach (var kv in soldierCfg)
        {
            var item = kv.Value;

            if (item["enabled"] == 1)
            {
                soldierCfgs.Add(item["cfgId"], item);
            }

            //Debug.Log(kv.Value);
        }

        Sort();


        var itemCount = (int)Math.Ceiling((float)soldierCfgList.Count / 3);

        dataCount = soldierCfgList.Count;

        soldierScrollView.LoopScrollView.SetDataCount(itemCount);
        EventDispatcher<string>.instance.AddEvent(EventName.UI_PutSoldierView_RenderItem, (evtName, evt) =>
        {
            soldierScrollView.LoopScrollView.SetRenderHandler(OnRenderItem);
        });
        EventDispatcher<int>.instance.AddEvent(EventName.UI_OpenSoldierLevelUpView, OpenSoldierLevelUpView);
        //EventDispatcher<CmdEventData>.instance.AddEvent(EventName.bag_info, ChangeBaginfo);
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.troop_info, ChangeTroopInfo);

        layerMask = LayerMask.GetMask("RayCast");
        IsDragUI = false;

        PhysicsRayMgr.instance.MouseDownOnUI += OnMouseDownOnUI;
        PhysicsRayMgr.instance.MouseDown += OnMouseDown;
        PhysicsRayMgr.instance.MouseOnUI += OnMouseOnUI;
        PhysicsRayMgr.instance.Mouse += OnMouse;
        PhysicsRayMgr.instance.MouseUpOnUI += OnMouseUpOnUI;
        PhysicsRayMgr.instance.MouseUp += OnMouseUp;

        if (Args.Length > 0 && GameData.instance.SelectedTroop != null)
        {
            TroopsMono troop = GameData.instance.SelectedTroop;
            var entityOffset = troop.entityOffsetList[0];

            int CfgId = entityOffset.Entity.CfgId;
            int id = troop.Id;

            UiMgr.Open<SoldierLevelUpView>(null, CfgId, id);
        }

        EventDispatcher<CmdEventData>.instance.TriggerEvent(EventName.bag_info);
    }

    private void Sort()
    {

        var bag_info = GameData.instance.PermanentData["bag_info"];
        var troop_infos = GameData.instance.PermanentData["troop_info"];

        List<int> troop_infoList = new List<int>();
        foreach (var troop in troop_infos)
        {
            troop_infoList.Add(troop.Value["cfgId"]);
        }

        //������ʿ����Ƭ
        List<int> bag_infoList = new List<int>();
        foreach (var item in bag_info)
        {
            if (int.Parse(item.Key) > 200000)
            {
                bag_infoList.Add(int.Parse(item.Key) - 200000);
            }
        }

        //δ�о���δ����Ƭ
        List<int> noList = new List<int>();
        //�о���
        List<int> hasTroopList = new List<int>();
        //�޾���,����Ƭ
        List<int> hasItemList = new List<int>();
        foreach (var soldier in soldierCfgs)
        {
            int soldierCfgId = soldier.Key;
            if (troop_infoList.Contains(soldierCfgId))
            {
                hasTroopList.Add(soldierCfgId);
            }
            else
            {
                if (bag_infoList.Contains(soldierCfgId))
                {
                    hasItemList.Add(soldierCfgId);
                }
                else
                {
                    noList.Add(soldierCfgId);
                }
            }
        }

        hasTroopList.Sort();
        hasItemList.Sort();
        noList.Sort();

        soldierCfgList.Clear();
        foreach (var cfgId in hasItemList)
        {
            var soldierCfg = soldierCfgs[cfgId];
            soldierCfgList.Add(soldierCfg);
        }

        foreach (var cfgId in hasTroopList)
        {
            var soldierCfg = soldierCfgs[cfgId];
            soldierCfgList.Add(soldierCfg);
        }

        foreach (var cfgId in noList)
        {
            var soldierCfg = soldierCfgs[cfgId];
            soldierCfgList.Add(soldierCfg);
        }
    }

    private void ChangeTroopInfo(string evtName, CmdEventData[] args)
    {
        EventDispatcher<string>.instance.TriggerEvent(EventName.UI_PutSoldierView_RenderItem);
    }

    //private void ChangeBaginfo(string evtName, CmdEventData[] args)
    //{
    //    var gold = GameData.instance.PermanentData["bag_info"]["100001"]["num"];
    //    layoutGoldTxt.Text.text = gold == null ? "0" : gold;

    //    var diamonds = GameData.instance.PermanentData["bag_info"]["100002"]["num"];
    //    layoutDiamondsTxt.Text.text = diamonds == null ? "0" : diamonds;
    //}

    private void OpenSoldierLevelUpView(string evtName, int[] args)
    {
        if (GameData.instance.SelectedTroop != null)
        {
            //UiMgr.Close<SoldierLevelUpView>();
            TroopsMono troop = GameData.instance.SelectedTroop;
            var entityOffset = troop.entityOffsetList[0];

            int CfgId = entityOffset.Entity.CfgId;
            int id = troop.Id;

            if (UiMgr.IsOpenView<SoldierLevelUpView>())
            {
                var levelUpView = UiMgr.GetView<SoldierLevelUpView>();
                var currOpensoldierCfgId = levelUpView.SoldierCfgId;

                if (currOpensoldierCfgId == CfgId)
                {
                    return;
                }
            }

            UiMgr.Close<SoldierLevelUpView>();
            UiMgr.Open<SoldierLevelUpView>(null, CfgId, id, true);
        }
        //EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, false);
    }

    protected override void OnUpdate()
    {
        //OnDragPutSolider();

        foreach (var item in dragSoldierList)
        {
            item.SpecialEffect.UpdateAnim(Time.deltaTime);
        }
    }

    protected override void OnHide()
    {
        EventDispatcher<string>.instance.RemoveEventByName(EventName.UI_PutSoldierView_RenderItem);
        EventDispatcher<int>.instance.RemoveEvent(EventName.UI_OpenSoldierLevelUpView, OpenSoldierLevelUpView);
        //EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.bag_info, ChangeBaginfo);
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.troop_info, ChangeTroopInfo);
        BoardMgr.instance.ShowBoard(false);

        PhysicsRayMgr.instance.MouseDownOnUI -= OnMouseDownOnUI;
        PhysicsRayMgr.instance.MouseDown -= OnMouseDown;
        PhysicsRayMgr.instance.MouseOnUI -= OnMouseOnUI;
        PhysicsRayMgr.instance.Mouse -= OnMouse;
        PhysicsRayMgr.instance.MouseUpOnUI -= OnMouseUpOnUI;
        PhysicsRayMgr.instance.MouseUp -= OnMouseUp;

        soldierCfgs.Clear();
        soldierCfgList.Clear();
    }

    protected override void OnDestroy()
    {
        soldierScrollView.LoopScrollView.Release();
        dragItem.Release();
    }

    public void OnClickClose() {
        UiMgr.Close<PutSoldierView>();
        //UiMgr.Open<MainView>();
        EventDispatcher<int>.instance.TriggerEvent(EventName.Scene_ShowSelectTroopRange, -1);
        //UIMgr.instance.ClosePanel("PutSoldier/PutSoldierView");
    }

    void OnRenderItem(GameObject obj, int index)
    {
        int childCount = obj.transform.childCount;
        //Debug.Log(GameData.instance.PermanentData["troop_info"]);
        for (int i = 0; i < childCount; i++)
        {
            int idx = index * childCount + i;
            PutSoldierViewItem item = PutSoldierViewItem.GetItem<PutSoldierViewItem>(obj.transform.GetChild(i).gameObject, _ItemList, this);
            item.OnBeginDragPutSoldier = OnBeginDragPutSoldier;
            item.OnEndDragPutSoldier = OnEndDragPutSolider;
            if (idx < dataCount)
            {
                var data = item.Data;
                data.Idx = idx;
                data.DragItem = dragItem;
                data.View = this;
                data.Cfg = soldierCfgList[idx];
                data.Id = GetHighLevelTroopsId(data.Cfg["cfgId"]);
                data.SoldierCfg = soldierCfg;

                item.SetActive(true);
                item.SetData();
            }
            else
            {
                item.SetActive(false);
            }
        }
    }

    //��ȡһ�־���û�������ҵȼ���ߵĲ���
    private string GetHighLevelTroopsId(int cfgId)
    {
        var troop_infos = GameData.instance.PermanentData["troop_info"];
        var battle_troop = GameData.instance.PermanentData["battle_troop"];
        string id = null;
        int level = 0;

        foreach (var info in troop_infos)
        {
            //�Ѿ�����ľͲ���ʾ
            if (battle_troop.HasKey(info.Key))
                continue;

            if (cfgId == info.Value["cfgId"].AsInt)
            {
                var data = info.Value;
                if (data["level"] > level) 
                {
                    level = data["level"];
                    id = info.Key;
                }
            }
        }

        return id;
    }

    public void OnBeginDragPutSoldier(PutSoldierViewItemData data)
    {
        IsDragUI = true;

        if (data.Id != null && GameData.instance.PermanentData["troop_info"].HasKey(data.Id))
        {
            var troopInfoData = GameData.instance.PermanentData["troop_info"][data.Id];

            JSONNode soldierBattleData = TableUtils.FindSoldierBattleConfByCfgidLevel(troopInfoData["cfgId"], troopInfoData["level"]);

            int count = soldierBattleData["count"];

            for (int i = 0; i < count; i++)
            {
                var item = ClassPool.instance.Pop<DragSoldier>();
                item.Init(i, soldierBattleData);
                dragSoldierList.Add(item);
            }
        }

        if (UIDragSoliderTrans == null)
        {
            //UIDragSoliderTrans = ResMgr.instance.LoadGameObject("TroopMono").transform;
            ResMgr.instance.LoadGameObjectAsync("TroopMono", (go) =>
            {
                UIDragSoliderTrans = go.transform;
                UIDragSoliderTrans.gameObject.SetActive(false);
            });
        }
    }

    public void OnEndDragPutSolider(int troopId)
    {
        if (UIDragSoliderTrans.gameObject.activeSelf)
        {
            //var nearestPos = GameUtils.FindNearestGridPoint(dragSoliderhitPos);
            var gridId = GameUtils.FindNearestGrid(dragSoliderhitPos);
            if (gridId != -1)
            {
                JSONObject obj = new JSONObject();
                JSONObject obj1 = new JSONObject();
                obj.Add("data", obj1);
                obj1.Add("grid", gridId);
                obj1.Add("id", troopId);
                Cmd.instance.C2S_PUT_TROOP_ON(obj);
            }
        }

        foreach (var item in dragSoldierList)
        {
            item.Release();
        }
        dragSoldierList.Clear();

        IsDragUI = false;
        ResMgr.instance.ReleaseGameObject(UIDragSoliderTrans.gameObject);
        UIDragSoliderTrans = null;
    }

    public void OnMouseDownOnUI(Ray ray)
    {

    }

    public void OnMouseDown(Ray ray)
    {
        if (Physics.Raycast(ray, out hit, rayLength, layerMask))
        {
            CurrPhysicsRayMono = hit.collider.GetComponent<PhysicsRayMono>();
            //CurrPhysicsRayMono.OnMouseButtonDown?.Invoke(hit.point);
            CurrPhysicsRayMono.MouseDown?.Invoke(ray);
        }
    }

    public void OnMouseOnUI(Ray ray)
    {
        if (CurrPhysicsRayMono != null)
        {
            if (CurrPhysicsRayMono is TroopsMono troopsMono)
            {
                //troopsMono.ShowEntitys(false);
                CurrPhysicsRayMono.MouseOnUI?.Invoke(ray);
            }
        }

        if (UIDragSoliderTrans != null)
        {
            UIDragSoliderTrans.SetActiveEx(false);
        }
    }

    public void OnMouse(Ray ray)
    {
        if (IsDragUI)
        {
            if (Physics.Raycast(ray, out hit, rayLength, layerMask))
            {
                if (UIDragSoliderTrans != null)
                {
                    UIDragSoliderTrans.SetActiveEx(true);

                    dragSoliderhitPos = hit.point;

                    dragSoliderhitPos = GameUtils.CheckMapBorderX(FixVector3.ToFixVector3(dragSoliderhitPos).ToFixVector2()).ToVector3();

                    UIDragSoliderTrans.position = dragSoliderhitPos;

                    foreach (var item in dragSoldierList)
                    {
                        item.UpdatePos(dragSoliderhitPos);
                    }
                }
            }
            return;
        }

        if (CurrPhysicsRayMono != null)
        {
            if (Physics.Raycast(ray, out hit, rayLength, layerMask))
            {
                //CurrPhysicsRayMono.OnMouseButton?.Invoke(hit.point, true);
                CurrPhysicsRayMono.Mouse?.Invoke(ray);
            }
        }
    }

    public void OnMouseUpOnUI()
    {
        if (CurrPhysicsRayMono != null)
        {
            //CurrPhysicsRayMono.OnMouseButtonUp?.Invoke(false);
            CurrPhysicsRayMono.MouseUpOnUI?.Invoke();
            CurrPhysicsRayMono = null;
        }
    }

    public void OnMouseUp()
    {
        if (CurrPhysicsRayMono != null)
        {
            //CurrPhysicsRayMono.OnMouseButtonUp?.Invoke(true);
            CurrPhysicsRayMono.MouseUp?.Invoke();
            CurrPhysicsRayMono = null;
        }
    }

    private void OnStartGame()
    {

    }

    //public override void Close()
    //{
    //    soldierScrollView.LoopScrollView.Release();
    //    base.Close();
    //}

    //=============guide
    public override RectTransform GetGuideTrans(GuideNode guideNode)
    {
        if (guideNode.cfg["strArg2"] == "ClickSoldier") {
            foreach (var item in _ItemList.Values)
            {
                PutSoldierViewItem putSoldierViewItem = (PutSoldierViewItem)item;
                if (putSoldierViewItem.Data.Cfg["cfgId"].ToString() == guideNode.cfg["strArg3"] && putSoldierViewItem.CheckIsCanGuide()) {
                    return putSoldierViewItem.transform.GetComponent<RectTransform>();
                }
            }
            return null;
        }

        return base.GetGuideTrans(guideNode);
    }

    public override void TriggerGuide(GuideNode guideNode)
    {
        if (guideNode.cfg["strArg2"] == "ClickSoldier")
        {
            foreach (var item in _ItemList.Values)
            {
                PutSoldierViewItem putSoldierViewItem = (PutSoldierViewItem)item;
                if (putSoldierViewItem.Data.Cfg["cfgId"].ToString() == guideNode.cfg["strArg3"])
                {
                    putSoldierViewItem.OnClick();
                    guideNode.NextStep();
                }
            }
            return;
        }

        base.TriggerGuide(guideNode);
    }

}
