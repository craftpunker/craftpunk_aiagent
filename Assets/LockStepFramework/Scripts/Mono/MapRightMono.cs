#if _CLIENTLOGIC_
using Battle;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapRightMono : PhysicsRayMono
{
    private Ray ray;
    private RaycastHit hit;//

    private bool isDrag;
    private float time;
    private Vector3 mouseStartPos;

    private float start2EndLength = 5f; //
    private float dragTime = 0.2f; //
    private BoxCollider boxCollider;
    int layerMask;
    public float rayLength = 100f;

    // Use this for initialization
    void Start()
    {
        isDrag = false;
        time = 0;
        boxCollider = GetComponent<BoxCollider>();
        //boxCollider.enabled = false;
        layerMask = LayerMask.GetMask("RayCast");
    }

    private void OnEnable()
    {
        MouseDown += (ray) =>
        {
            if (Physics.Raycast(ray, out hit, rayLength, layerMask))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    mouseStartPos = Input.mousePosition;
                    time = 0;
                    isDrag = true;
                }
            }
        };

        MouseUp += () =>
        {
            if (isDrag)
            {
                var length = Vector3.Distance(mouseStartPos, Input.mousePosition);
                if (length >= start2EndLength && time <= dragTime)
                {
                    var v3 = mouseStartPos - Input.mousePosition;
                    int dir = 1;

                    if (Vector3.Dot(v3, Vector3.up) <= 0)
                    {
                        dir = -1;
                    }
                    int cur_deploy = GameData.instance.PermanentData["cur_deploy"] + dir;

                    if (cur_deploy >= 4 || cur_deploy <= 0)
                        return;

                    EventDispatcher<int>.instance.TriggerEvent(EventName.UI_ChangeRightUI, dir);
                }
                isDrag = false;
            }
        };

        PhysicsRayMgr.instance.MouseDown += MouseDown;
        PhysicsRayMgr.instance.MouseUp += MouseUp;

        //EventDispatcher<bool>.instance.AddEvent(EventName.Scene_ShowRightMapHixBox, (evtName, evt) =>
        //{
        //    bool value = evt[0];
        //    boxCollider.enabled = value;
        //});
    }

    private void OnDisable()
    {
        //EventDispatcher<bool>.instance.RemoveEventByName(EventName.Scene_ShowRightMapHixBox);

        PhysicsRayMgr.instance.MouseDown -= MouseDown;
        PhysicsRayMgr.instance.MouseUp -= MouseUp;
    }

    private void Update()
    {
        if (isDrag)
            time += Time.deltaTime;
    }
}
#endif