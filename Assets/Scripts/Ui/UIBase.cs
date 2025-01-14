using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UiStage { 
    show,
    hide,
    close,
}

public struct RemoveEventInfo {
    public int eventId;
    public EventHandler callback;
}

public class UiBase
{
    public virtual bool isShowMainBg
    {
        get { return false; }
    }

    public virtual bool isPauseGame
    {
        get { return false; }
    }

    public float closeDelay = 3;
    public float closeTime;

    public GameObject gameObject;
    public Transform transform;
    public UiStage uiStage;
    public bool IsLoading;

    List<GameObject> _EventReleaseList;

    List<RemoveEventInfo> _EventRemoveList;

    protected object[] Args;
    public void Init(GameObject go)
    {
        _EventReleaseList = new List<GameObject>();
        _EventRemoveList = new List<RemoveEventInfo>();
        gameObject = go;
        transform = go.transform;
        OnInit();
    }

    public void Show(object[] args)
    {
        gameObject.SetActive(true);
        Args = args;
        uiStage = UiStage.show;
        transform.SetAsLastSibling();
        OnShow();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        OnHide();
        uiStage = UiStage.hide;
    }

    public void Close()
    {
        if (uiStage == UiStage.close) {
            return;
        }

        if (IsLoading) {
            uiStage = UiStage.close;
            UiMgr.Destroy(this);
            //Mgr.UiMgr.Destroy(this);
            return;
        }

        //foreach (var obj in _EventRemoveList)
        //{
        //    EventManager.RemoveEvent(obj.eventId, obj.callback);
        //}
        //_EventRemoveList.Clear();

        Hide();
        closeTime = Time.time + closeDelay;
        uiStage = UiStage.close;
    }

    public void Update()
    {
        if (IsLoading || uiStage == UiStage.close) {
            return;
        }
        OnUpdate();
    }

    public void Destroy()
    {
        DebugUtils.Log($"Destroy {this}");

        if (IsLoading) {
            return;
        }

        foreach (var item in _EventReleaseList)
        {
            UIEventHandler.Clear(item);
        }
        _EventReleaseList = null;

        OnDestroy();
        ResMgr.Destroy(gameObject);

        //ResManager.ReleaseInstance(gameObject);
        //GameObject.Destroy(gameObject);
    }

    //overide=============================================
    protected virtual void OnInit()
    {

    }

    protected virtual void OnShow()
    {

    }

    protected virtual void OnUpdate()
    {

    }

    protected virtual void OnHide()
    {

    }

    protected virtual void OnDestroy()
    {

    }

    //=============================================

    protected void SetOnClick(GameObject go, Action onClick, string audioName = "ui_button_click")
    {
        _EventReleaseList.Add(go);
        UIEventHandler.Get(go).SetOnClick(onClick, audioName);
    }

    public void SetLongPress(GameObject go, Action action, float longPressTime = 0.5f)
    {
        _EventReleaseList.Add(go);
        UIEventHandler.Get(go).SetOnLongPress(action, longPressTime);
    }

    protected virtual void SetOnPointerDown(GameObject go, Action<PointerEventData> onPointerDown)
    {
        _EventReleaseList.Add(go);
        UIEventHandler.Get(go).SetOnPointerDown(onPointerDown);
    }

    protected virtual void SetOnPointerUp(GameObject go, Action<PointerEventData> onPointerUp)
    {
        _EventReleaseList.Add(go);
        UIEventHandler.Get(go).SetOnPointerUp(onPointerUp);
    }

    protected void SetOnClick(U3dObj go, Action onClick, string audioName = "ui_button_click")
    {
        _EventReleaseList.Add(go.gameObject);
        UIEventHandler.Get(go.gameObject).SetOnClick(onClick, audioName);
    }

    protected U3dObj Find(string path)
    {
        return new U3dObj(transform.Find(path));
    }

    protected GameObject FindGameObject(string path)
    {
        return transform.Find(path).gameObject;
    }

    protected void AddEvent(int eventId, EventHandler evt)
    {
        //_EventRemoveList.Add(new RemoveEventInfo() { eventId = eventId, callback = evt });
        //EventManager.AddEvent(eventId, evt);
    }

    // guide================================
    public virtual RectTransform GetGuideTrans(GuideNode guideNode)
    {
        string btnName = guideNode.btnName;
        System.Reflection.FieldInfo fieldInfo = GetType().GetField(btnName);
        U3dObj obj = (U3dObj)fieldInfo.GetValue(this);

        if (obj.gameObject.activeInHierarchy) {
            return obj.RectTransform;
        }

        return null;
    }

    public virtual void TriggerGuide(GuideNode guideNode)
    {
        //Debug.Log("TriggerGuide================================");

        string methodName = guideNode.methodName;
        System.Reflection.MethodInfo method = GetType().GetMethod(methodName, new Type[] { });      // 获取方法信息
        object[] parameters = null;
        method.Invoke(this, parameters);

        guideNode.NextStep();
    }
}
