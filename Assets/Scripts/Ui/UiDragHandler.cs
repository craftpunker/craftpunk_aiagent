using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Action<PointerEventData> onBeginDrag;
    private Action<PointerEventData> onDrag;
    private Action<PointerEventData> onEndDrag;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static UiDragHandler Get(GameObject go)
    {
        UiDragHandler handler = go.GetComponent<UiDragHandler>();
        if (handler == null)
        {
            handler = go.AddComponent<UiDragHandler>();
        }

        return handler;
    }

    public static void Clear(GameObject go)
    {
        foreach (UiDragHandler handler in go.GetComponentsInChildren<UiDragHandler>(true))
        {
            handler.ClearDelegates();

            Destroy(handler);
            //handler
            //handler.GetComponent
        }
    }

    public void ClearDelegates() {

        onBeginDrag = null;
        onDrag = null;
        onEndDrag = null;
    }

    public void SetOnBeginDrag(Action<PointerEventData> action, string audioName)
    {
        onBeginDrag = (peData) =>
        {
            action(peData);
            AudioMgr.instance.PlayOneShot(audioName);
        };
    }

    public void SetOnDrag(Action<PointerEventData> action)
    {
        onDrag = action;
    }

    public void SetOnEndDrag(Action<PointerEventData> action, string audioName)
    {
        onEndDrag = (peData) =>
        {
            action(peData);
            AudioMgr.instance.PlayOneShot(audioName);
        };
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        //print($"OnBeginDrag  {eventData.position}");
        onBeginDrag?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(eventData);
        //print($"OnEndDrag  {eventData.position}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        //print($"OnDrag  {eventData.position}");
        onDrag?.Invoke(eventData);
    }
}
