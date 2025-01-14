using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class UIEventHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
    //,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Action onClick;
    private Action<PointerEventData> onPointerDown;
    private Action<PointerEventData> onPointerUp;

    //private Action<PointerEventData> onBeginDrag;
    //private Action<PointerEventData> onDrag;
    //private Action<PointerEventData> onEndDrag;

    private Action onPointerEnter;
    private Action onPointerExit;
    private Action onLongPress;
    private Action onLongPressMoreThanOnce;
    private bool isPointerDown = false;
    private bool isLongPressEnd = true;
    private bool isLongPressMoreThanOnceEnd = true;
    private float longPressTimer = 0f;
    private float longPressTime = 0.5f;
    private float longPressInterval = 0.1f;
    private float longPressMoreThanOnceTimer = 0f;
    private float longPressAcceleration = 0f;
    private float longPressMinInterval = 0.1f;
    private float longPressCurInterval = 0f;

    public static UIEventHandler Get(GameObject go)
    {
        UIEventHandler handler = go.GetComponent<UIEventHandler>();
        if (handler == null)
        {
            handler = go.AddComponent<UIEventHandler>();
        }

        return handler;
    }

    public static void Clear(GameObject go)
    {
        foreach (UIEventHandler handler in go.GetComponentsInChildren<UIEventHandler>(true))
        {
            handler.ClearDelegates();
        }
    }

    public void ClearStatus()
    {
        longPressTimer = 0f;
        longPressMoreThanOnceTimer = 0f;
        isPointerDown = false;
        isLongPressEnd = true;
        isLongPressMoreThanOnceEnd = true;
    }

    private void OnDisable()
    {
        ClearStatus();
    }

    public void OnDestroy()
    {
        this.ClearDelegates();
    }

    public void ClearDelegates()
    {
        this.onClick = null;
        this.onPointerDown = null;
        this.onPointerUp = null;
        this.onPointerEnter = null;
        this.onPointerExit = null;
        this.onLongPress = null;
        this.onLongPressMoreThanOnce = null;

        //this.onEndDrag = null;
        //this.onBeginDrag = null;
        //this.onEndDrag = null;
    }

    private void Update()
    {
        //，，ondrag，longpress timer
        if(isPointerDown)
        {
            longPressTimer += Time.deltaTime;
            longPressMoreThanOnceTimer += Time.deltaTime;
            if(!isLongPressEnd)
            {
                if(longPressTimer >= longPressTime)
                {
                    OnLongPress();
                    isLongPressEnd = true;
                    longPressTimer = 0f;
                }
            }

            if(!isLongPressMoreThanOnceEnd)
            {
                if(longPressMoreThanOnceTimer >= longPressCurInterval)
                {
                    OnLongPressMoreThanOnce();
                    //longPressMoreThanOnceTimer = 0;
                    longPressCurInterval -= longPressAcceleration;
                    if(longPressCurInterval < longPressMinInterval)
                    {
                        longPressCurInterval = longPressMinInterval;
                    }
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick();//(gameObject, eventData);
            //ClickMark.getInstance().afterBtnClick = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null)
            onPointerDown(eventData);

        isPointerDown = true;
        isLongPressEnd = false;
        isLongPressMoreThanOnceEnd = false;
        longPressCurInterval = longPressInterval;
        longPressMoreThanOnceTimer = longPressInterval - Time.deltaTime;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (onPointerUp != null)
            onPointerUp(eventData);

        longPressTimer = 0f;
        longPressMoreThanOnceTimer = 0f;
        isPointerDown = false;
        isLongPressEnd = true;
        isLongPressMoreThanOnceEnd = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onPointerEnter != null)
            onPointerEnter();//(gameObject, eventData);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (onPointerExit != null)
            onPointerExit();//(gameObject, eventData);
    }

    public void SetOnPointerEnter(Action callcack)
    {
        onPointerEnter += callcack;
    }

    public void SetOnPointerExit(Action callcack)
    {
        onPointerExit += callcack;
    }


    //public void SetOnBeginDrag(Action<PointerEventData> action) { 
    //    onBeginDrag = action;
    //}

    //public void SetOnDrag(Action<PointerEventData> action)
    //{
    //    onDrag = action;
    //}

    //public void SetOnEndDrag(Action<PointerEventData> action)
    //{
    //    onEndDrag = action;
    //}

    public void OnLongPress()
    {
        if (onLongPress != null)
        {
            onLongPress();//(gameObject);
        }
    }

    public void OnLongPressMoreThanOnce()
    {
        if(onLongPressMoreThanOnce != null)
            onLongPressMoreThanOnce();//(gameObject);

        longPressMoreThanOnceTimer = 0;
    }


    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    //print($"OnBeginDrag  {eventData.position}");
    //    onBeginDrag?.Invoke( eventData );
    //}


    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    onEndDrag?.Invoke(eventData);
    //    //print($"OnEndDrag  {eventData.position}");
    //}

    //public void OnDrag(PointerEventData eventData) {
    //    //print($"OnDrag  {eventData.position}");
    //    onDrag?.Invoke(eventData);
    //}

    /*
    public void AddInputFieldValueChangeEventListener(Action callback)
    {
        UnityEngine.UI.InputField inputField = gameObject.GetComponent<UnityEngine.UI.InputField>();
        if (inputField)
        {
            inputField.onValueChanged.AddListener(delegate { callback(gameObject); });
        }
    }

    public void AddScrollRectValueChangeEventListener(Action callback)
    {
        UnityEngine.UI.ScrollRect scrollRect = gameObject.GetComponent<UnityEngine.UI.ScrollRect>();
        if (scrollRect)
        {
            scrollRect.onValueChanged.AddListener(delegate { callback(gameObject); });
        }
    }
    */

    public void SetOnPointerDown(Action<PointerEventData> onPointerDown)
    {
        if (onPointerDown == null)
            return;

        this.onPointerDown = onPointerDown;
    }
    public void SetOnPointerUp(Action<PointerEventData> onPointerUp)
    {
        if (onPointerUp == null)
            return;

        this.onPointerUp = onPointerUp;
    }
    // public void SetOnClick(Action onClick)
    // {
    //     if (onClick == null)
    //         return;

    //     this.onClick = onClick;
    // }

    //public void SetOnClick(Action onClick, String audioEvent = "event:/UI_button_click", String bank = "se_UI")
    //{
    //    if (onClick == null)
    //        return;

    //    this.onClick = () => {
    //        onClick();
    //        if(audioEvent != null && audioEvent != "")
    //        {
    //            //AudioFmodMgr.instance.PlaySFX(audioEvent);
    //            //AudioFmodMgr.instance.Play2DOneShot(audioEvent, bank);
    //            //AudioMgr.instance.Play2DAudio(audioName);
    //        }
    //    };
    //}

    public void SetOnClick(Action onClick, string audioName)
    {
        if (onClick == null)
            return;

        this.onClick = () => {
            onClick();
            AudioMgr.instance.PlayOneShot(audioName);
        };
    }

    public void InvokeOnClick()
    {
        if (onClick != null)
            onClick.Invoke();
    }

    public void InvokeOnPointerDown()
    {
        if(onPointerDown != null)
            onPointerDown.Invoke(null);
    }

    public void InvokeOnPointerUp()
    {
        if(onPointerUp != null)
            onPointerUp.Invoke(null);
    }

    public void SetOnLongPress(Action onLongPress, float longPressTime = 0.5f)
    {
        this.longPressTime = longPressTime;
        if (onLongPress == null)
            return;

        this.onLongPress = onLongPress;
    }

    public void SetOnLongPressMoreThanOnce(Action onLongPressMoreThanOnce)
    {
        if(onLongPressMoreThanOnce == null)
            return;

        this.onLongPressMoreThanOnce = onLongPressMoreThanOnce;
    }

    void OnMouseDown() {
    }

    void OnMouseUp() {
        if (IsUILayerInput())
            return;

        if (onClick != null)
            onClick.Invoke();
    }

    private bool IsUILayerInput() {
        if (EventSystem.current == null) {
            return false;
        }
#if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject())
#else
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#endif
        {
            return true;
        }
        return false;

    }
}
