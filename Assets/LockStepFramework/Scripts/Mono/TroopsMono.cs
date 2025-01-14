
#if _CLIENTLOGIC_
using Battle;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public struct EntityOffset
{
    public EntityBase Entity;
    public FixVector3 Offset;

    public EntityOffset(EntityBase entity, FixVector3 offset)
    {
        Entity = entity;
        Offset = offset;
    }
}

public class TroopsMono : PhysicsRayMono
{
    public int Id;
    public int SoldierFlagId;
    public List<EntityOffset> entityOffsetList = new List<EntityOffset>();
    private FixVector3 troopPos;
    private float z = -0.1f;
    private Vector2 mouseOffset;
    public PlayerGroup PlayerGroup;
    public Vector3 originPos; //
    public int Grid;
    private RaycastHit hit;
    public float rayLength = 100f;
    int layerMask;

    public SpecialEffect Range;

    private void Start()
    {
        layerMask = LayerMask.GetMask("RayCast");
      
    }

    private void OnEnable()
    {
        //PhysicsRayMgr.instance.MouseDown += OnPhysicsRayMgrMouseDown;

        MouseDown += (ray) =>
        {

            if (Physics.Raycast(ray, out hit, rayLength, layerMask))
            {
                originPos = transform.position;
                mouseOffset = transform.position - hit.point;
                gameObject.GetComponent<BoxCollider>().enabled = false;
                //UiMgr.Open<PutSoldierView>();
                //EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowTroopRange, true);

                GameData.instance.SelectedTroop = this;
                EventDispatcher<int>.instance.TriggerEvent(EventName.UI_OpenSoldierLevelUpView);
                EventDispatcher<int>.instance.TriggerEvent(EventName.Scene_ShowSelectTroopRange, -1);
            }
        };

        Mouse += (ray) =>
        {
            if (Physics.Raycast(ray, out hit, rayLength, layerMask))
            {
                ShowEntitys(true);
                var pos = GameUtils.CheckMapBorderX(FixVector3.ToFixVector3(hit.point).ToFixVector2());

                UpdatePos(pos.ToFixVector3());
            }

            if (Range != null && Range.Trans != null)
            {
                Range.GameObj.SetActive(true);
            }
        };

        MouseOnUI += (ray) =>
        {
            ShowEntitys(false);
            if (Range != null && Range.Trans != null)
            {
                Range.GameObj.SetActive(false);
            }
        };

        MouseUp += () =>
        {
            //var nearestPos = GameUtils.FindNearestGridPoint(transform.position);
            var gridId = GameUtils.FindNearestGrid(transform.position);
            if (gridId != -1 && gridId != Grid && !GameUtils.IsGridLock(gridId))
            {
                JSONObject jsonObj = new JSONObject();
                JSONObject jsonObj1 = new JSONObject();
                jsonObj1.Add("id", Id);
                jsonObj1.Add("grid", gridId);
                jsonObj.Add("data", jsonObj1);
                Cmd.instance.C2S_ADJUST_TROOP(jsonObj);
            }
            else
            {
                UpdatePos(new FixVector3((Fix64)originPos.x, (Fix64)originPos.y, Fix64.Zero));
                gameObject.GetComponent<BoxCollider>().enabled = true;
                ShowEntitys(true);
            }
            AudioMgr.instance.PlayOneShot("ui_soldier_change");

            EventDispatcher<int>.instance.TriggerEvent(EventName.Scene_ShowSelectTroopRange, Id);
        };

        MouseUpOnUI += () =>
        {
            EventDispatcher<int>.instance.TriggerEvent(EventName.Scene_ShowSelectTroopRange, -1);

            if (GameData.instance.TroopsMonos.Count <= 1)
            {
                UpdatePos(new FixVector3((Fix64)originPos.x, (Fix64)originPos.y, Fix64.Zero));
                gameObject.GetComponent<BoxCollider>().enabled = true;
                ShowEntitys(true);
                return;
            }

            JSONObject jsonObj = new JSONObject();
            JSONObject jsonObj1 = new JSONObject();
            jsonObj1.Add("id", Id);
            jsonObj.Add("data", jsonObj1);
            Cmd.instance.C2S_PUT_TROOP_OFF(jsonObj);

            AudioMgr.instance.PlayOneShot("ui_soldier_remove");

            //EventDispatcher<TroopsMono>.instance.TriggerEvent(EventName.Scene_CloseTroopRange);
        };

        EventDispatcher<int>.instance.AddEvent(EventName.Scene_ShowSelectTroopRange, Scene_ShowSelectTroopRange);
    }

    private void Update()
    {
        if (Range != null && Range.Trans != null)
        {
            Range.Trans.position = transform.position;
        }
    }

    private void Scene_ShowSelectTroopRange(string evtName, int[] args)
    {
        int id = args[0];

        ReleaseRange();

        CreateRange(id);
    }

    private void CreateRange(int id)
    {
        if (id == Id)
        {
            //GameData.instance.SelectedTroop = this;
            Range = SpecialEffectFactory.instance.CreateSpecialEffect(30022,
            new FixVector3(transform.position.x, transform.position.y, transform.position.z), (go) =>
            {
                go.GameObj.SetActive(true);
            });
        }
    }

    public void UpdatePos(FixVector3 pos)
    {
        var monoV3 = pos.ToVector3();
        transform.position = new Vector3(monoV3.x, monoV3.y, transform.position.z);
        foreach (var eo in entityOffsetList)
        {
            eo.Entity.Fixv3LogicPosition = pos + eo.Offset;
           // Simulator.instance.setAgentPosition(eo.Entity.Agent.id_, eo.Entity.Fixv3LogicPosition);

#if _CLIENTLOGIC_
            eo.Entity.UpdateRenderPosition(0);
#endif
        }

        gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    public void SetEntity(EntityBase entity, FixVector3 offset)
    {
        EntityOffset eo = new EntityOffset(entity, offset);
        entityOffsetList.Add(eo);

        if (PlayerGroup == PlayerGroup.None)
        {
            PlayerGroup = entity.PlayerGroup;
        }
    }

    public void RemoveEntitys()
    {
        for (int i = 0; i < entityOffsetList.Count; i++)
        {
            var item = entityOffsetList[i].Entity;

            item.BKilled = true;
            if (item.Agent != null)
            {
                Simulator.instance.delAgent(item.Agent.id_);
            }
            //item.Agent.needDelete_ = true;
            BattleMgr.instance.SoldierList.Remove(item);
            item.Release();
        }

        //for (int i = GameData.instance.PlayerBattleData.troops.Count - 1; i >= 0; i--)
        //{
        //    var kv = GameData.instance.PlayerBattleData.troops.ElementAt(i);
        //    if (kv.Key == Id)
        //    {
        //        GameData.instance.PlayerBattleData.troops.Remove(kv.Key);
        //    }
        //}

        //Simulator.instance.updateDeleteAgent();
        Release();
    }

    public void ShowEntitys(bool value)
    {
        for (int i = entityOffsetList.Count - 1; i >= 0; i--)
        {
            var item = entityOffsetList[i].Entity;
            item.Trans?.SetActiveEx(value);
        }
    }

    private void OnDisable()
    {
        EventDispatcher<int>.instance.RemoveEvent(EventName.Scene_ShowSelectTroopRange, Scene_ShowSelectTroopRange);
        //EventDispatcher<TroopsMono>.instance.RemoveEvent(EventName.Scene_CloseTroopRange, CloseTroopRange);
        //PhysicsRayMgr.instance.MouseDown -= OnPhysicsRayMgrMouseDown;


        ReleaseRange();

        MouseDownOnUI = null;
        MouseDown = null;
        MouseOnUI = null;
        Mouse = null;
        MouseUpOnUI = null;
        MouseUp = null;

    }

    private void ReleaseRange()
    {
        if (Range != null)
        {
            Range.BKilled = true;
            //BattleMgr.instance.SpecialEffectList.Remove(Range);
            //Range.Release();
            Range = null;
        }
    }

    public void Release()
    {
        entityOffsetList.Clear();
        PlayerGroup = PlayerGroup.None;
        GameData.instance.TroopsMonos.Remove(Id);

        ReleaseRange();

        if (gameObject != null)
            ResMgr.instance.ReleaseGameObject(gameObject);
    }
}
#endif