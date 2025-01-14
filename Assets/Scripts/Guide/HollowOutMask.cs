using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mask
/// </summary>
public class HollowOutMask : MaskableGraphic, ICanvasRaycastFilter
{
    [SerializeField]
    private RectTransform _target;
    public Bounds targetBound;
    Vector3 posOffset;
    Vector3 sizeOffset;

    private Vector3 _targetMin = Vector3.zero;
    private Vector3 _targetMax = Vector3.zero;
 
    private bool _canRefresh = true;
    private Transform _cacheTrans = null;

    private float starTime = 0;
    private float duration = 0;
    
    int status = 0;
    [HideInInspector]
    public int Status {
        get { return status; }
    }
    
    const int STATUS_PLAYING = 1;
    const int STATUS_FINISH = 2;

    Action<Vector3, Vector3> animPopulateMeshCallBack;

    //private bool duration;

    protected override void Awake()
    {
        //HollowOutMask ho;
        //ho.color
        base.Awake();
        _cacheTrans = GetComponent<RectTransform>();
    }
 
    void Update()
    {
        _canRefresh = true;
        _RefreshView();
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetTargetObj(RectTransform target, Vector3 posOffset, Vector3 sizeOffset)
    {
        _canRefresh = true;
        _target = target;

        this.posOffset = posOffset;
        this.sizeOffset = sizeOffset;
        //_RefreshView();
    }
 
    private void _SetTarget(Vector3 tarMin, Vector3 tarMax)
    {
        if (tarMin == _targetMin && tarMax == _targetMax) {
            status = STATUS_FINISH;
            return;
        }
        
        _targetMin = tarMin;
        _targetMax = tarMax;
        SetAllDirty();
        animPopulateMeshCallBack?.Invoke(_targetMin, _targetMax);
    }

    public void PlayAnim(float duration)
    {
        //this.animPopulateMeshCallBack = animPopulateMeshCallBack;
        starTime = Time.time;
        this.duration = duration;
        status = STATUS_PLAYING;
    }

    public void SetAnimPopulateMeshCallBack(Action<Vector3, Vector3> animPopulateMeshCallBack)
    {
        this.animPopulateMeshCallBack = animPopulateMeshCallBack;
    }

    
    private void _RefreshView()
    {
        if (!_canRefresh) return;
        _canRefresh = false;
 
        if (null == _target)
        {
            _SetTarget(Vector3.zero, Vector3.zero);
            //SetAllDirty();
        }
        else
        {
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_cacheTrans, _target);
            targetBound = bounds;

            float screenWidth = UnityEngine.Screen.width;
            float screenHeight = UnityEngine.Screen.height;

            Vector2 pivot = rectTransform.pivot;
            Vector3 min = new Vector3(screenWidth * (pivot.x - 1), screenHeight * (pivot.y - 1), 0);
            Vector3 max = new Vector3(screenWidth * pivot.x, screenHeight * pivot.y, 0);

            bounds.min += posOffset;
            bounds.max += posOffset;

            Vector3 boundsMin = bounds.min + posOffset;
            Vector3 boundsMax = bounds.max + posOffset;

            boundsMin.x -= sizeOffset.x / 2;
            boundsMin.y -= sizeOffset.y / 2;

            boundsMax.x += sizeOffset.x / 2;
            boundsMax.y += sizeOffset.y / 2;

            //bounds.min.x -= sizeOffset.x / 2;


            Vector3 tarMin = boundsMin;
            Vector3 tarMax = boundsMax;

            if (status == STATUS_PLAYING) {
                float time = (Time.time - starTime) / duration;
                tarMin.x = Mathf.SmoothStep(min.x, boundsMin.x, time);
                tarMin.y = Mathf.SmoothStep(min.y, boundsMin.y, time);
                tarMax.x = Mathf.SmoothStep(max.x, boundsMax.x, time);
                tarMax.y = Mathf.SmoothStep(max.y, boundsMax.y, time);
            }

            _SetTarget(tarMin, tarMax);
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (_targetMin == Vector3.zero && _targetMax == Vector3.zero)
        {
            base.OnPopulateMesh(vh);
            return;
        }
 
        vh.Clear();
 
        // 
        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;
 
        Vector2 selfPiovt = rectTransform.pivot;
        Rect selfRect = rectTransform.rect;
        float outerLx = -selfPiovt.x * selfRect.width;
        float outerBy = -selfPiovt.y * selfRect.height;
        float outerRx = (1 - selfPiovt.x) * selfRect.width;
        float outerTy = (1 - selfPiovt.y) * selfRect.height;
        // 0 - Outer:LT
        vert.position = new Vector3(outerLx, outerTy);

        // print(outerLx);
        vh.AddVert(vert);
        // 1 - Outer:RT
        vert.position = new Vector3(outerRx, outerTy);
        vh.AddVert(vert);
        // 2 - Outer:RB
        vert.position = new Vector3(outerRx, outerBy);
        vh.AddVert(vert);
        // 3 - Outer:LB
        vert.position = new Vector3(outerLx, outerBy);
        vh.AddVert(vert);
 
        // 4 - Inner:LT
        vert.position = new Vector3(_targetMin.x, _targetMax.y);
        vh.AddVert(vert);
        // 5 - Inner:RT
        vert.position = new Vector3(_targetMax.x, _targetMax.y);
        vh.AddVert(vert);
        // 6 - Inner:RB
        vert.position = new Vector3(_targetMax.x, _targetMin.y);
        vh.AddVert(vert);
        // 7 - Inner:LB
        vert.position = new Vector3(_targetMin.x, _targetMin.y);
        vh.AddVert(vert);

        // vh.AddTriangle(0, 2, 3);

        // vh.AddTriangle(0, 1, 2);
        // vh.AddTriangle(3, 1, 2);
 
        // 
        vh.AddTriangle(4, 0, 1);
        vh.AddTriangle(4, 1, 5);
        vh.AddTriangle(5, 1, 2);
        vh.AddTriangle(5, 2, 6);
        vh.AddTriangle(6, 2, 3);
        vh.AddTriangle(6, 3, 7);
        vh.AddTriangle(7, 3, 0);
        vh.AddTriangle(7, 0, 4);
    }
 
    bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        if (null == _target) return true;
        // （）
        return !RectTransformUtility.RectangleContainsScreenPoint(_target, screenPos, eventCamera);
    }
}