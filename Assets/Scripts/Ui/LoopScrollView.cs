using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

class ItemInfo
{
    public int dataIndex;
    public GameObject go;
    public RectTransform rect;
    public ItemInfo(GameObject go)
    {
        this.go = go;
        rect = go.GetComponent<RectTransform>();
    }
}

enum ScrollDirection
{
    Vertical = 0,
    Horizontal = 1,
}
public enum PosDir
{
    None = 0,
    Top = 1,
    Bottom = -1,
    // Left = 1,
    // Right = -1,
}

public class LoopScrollView : MonoBehaviour //, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;
    public int dataCount = 0;
    public PosDir refreshDirection = PosDir.Top;
    public bool isRefreshAsync = false;

    public float spancing = 0;
    public float top = 0;
    public float bottom = 0;
    
    ScrollRect _scrollRect = null;
    RectTransform _rectViewport = null;
    RectTransform _rectContent = null;
    RectTransform _rect = null;
    RectTransform _rectPrefab;
    Action<GameObject, int> _renderHandler;
    Func<int, Vector2> _renderSizeHandler;

    Dictionary<GameObject, ItemInfo> _dictItem = new Dictionary<GameObject, ItemInfo>();
    List<ItemInfo> _showingItemList = new List<ItemInfo>();
    Stack<ItemInfo> _itemPool = new Stack<ItemInfo>();
    Vector2[] _arrDataIndex2Position;

    int _beginDataIndex;
    int _endDataIndex;

    void Awake()
    {
    }

    void Start()
    {
    }

    void Update()
    {
        UpdateItems();
    }

    void OnDestroy()
    {
        //Release();
    }

    Dictionary<GameObject, ItemBase> _ReleaseDict;
    public void Init(Dictionary<GameObject, ItemBase> releaseDict = null)
    {
        _scrollRect = transform.GetComponent<ScrollRect>();
        _rectViewport = _scrollRect.viewport.GetComponent<RectTransform>();
        _rectContent = _scrollRect.content.GetComponent<RectTransform>();
        _rect = transform.GetComponent<RectTransform>();
        _scrollRect.onValueChanged.AddListener(OnScrollValueChange);
        _rectPrefab = itemPrefab.GetComponent<RectTransform>();

        if (refreshDirection == PosDir.Top)
        {
            _rectContent.anchorMin = new Vector2(0, 1);
            _rectContent.anchorMax = new Vector2(1, 1);
            _rectContent.pivot = new Vector2(0, 1);
        }
        else if (refreshDirection == PosDir.Bottom)
        {
            _rectContent.anchorMin = new Vector2(0, 0);
            _rectContent.anchorMax = new Vector2(1, 0);
            _rectContent.pivot = new Vector2(0, 0);
        }

        _ReleaseDict = releaseDict;
    }

    int GetDataIndexByContentPosition(Vector2 pos)
    {
        for (int i = 0; i < _arrDataIndex2Position.Length; i++)
        {
            if (refreshDirection == PosDir.Top && pos.y > _arrDataIndex2Position[i].y + (int)refreshDirection * spancing)
            {
                return Mathf.Max(i, 1);
            }
            else if (refreshDirection == PosDir.Bottom && pos.y < _arrDataIndex2Position[dataCount - i - 1].y + (int)refreshDirection * spancing)
            {
                return Mathf.Min(dataCount, dataCount - i + 1);
            }
        }

        if (refreshDirection == PosDir.Top)
        {
            return 0;
        }
        else
        {
            return dataCount;
        }
    }

    public int GetFirstDataIndexByContentPosition()
    {
        float f = 0;
        if (refreshDirection == PosDir.Top)
        {
            f = _rectContent.InverseTransformPoint(_rectViewport.position).y + _rectViewport.rect.height * (1 - _rectViewport.pivot.y);
        }
        else if (refreshDirection == PosDir.Bottom)
        {
            f = _rectContent.InverseTransformPoint(_rectViewport.position).y - _rectViewport.rect.height * _rectViewport.pivot.y;
        }
        return GetDataIndexByContentPosition(new Vector2(0, f));
    }

    public int GetLastDataIndexByContentPosition()
    {
        float f = 0;
        if (refreshDirection == PosDir.Top)
        {
            f = _rectContent.InverseTransformPoint(_rectViewport.position).y - 
                _rectViewport.rect.height * (1 - _rectViewport.pivot.y);
        }
        else if (refreshDirection == PosDir.Bottom)
        {
            f = _rectContent.InverseTransformPoint(_rectViewport.position).y + 
                _rectViewport.rect.height * _rectViewport.pivot.y;
        }
        return GetDataIndexByContentPosition(new Vector2(0, f));
    }

    void InitList(bool isInitContentPosition)
    {
        isUpdateItems = false;
        _scrollRect.StopMovement();
        _rectContent.DOKill();
        _arrDataIndex2Position = new Vector2[dataCount];
        if (refreshDirection == PosDir.Top || refreshDirection == PosDir.Bottom) {
            float _contentLenth = 0;
            for (int i = 0; i < dataCount; i++)
            {
                //print(RenderItemSize(i + 1));
                Vector2 size = RenderItemSize(i + 1);
                if (refreshDirection == PosDir.Top)
                {
                    _arrDataIndex2Position[i] = new Vector2(0, -_contentLenth - top);
                }
                else if (refreshDirection == PosDir.Bottom)
                {
                    _arrDataIndex2Position[dataCount - i - 1] = new Vector2(0, _contentLenth + bottom);
                }
                
                _contentLenth += size.y + spancing;
            }
            //_contentLenth -= spancing;
            _contentLenth = Mathf.Max(_rectPrefab.rect.height, _contentLenth - spancing + top + bottom);

            _rectContent.SetRectSizeY(_contentLenth);
            if (isInitContentPosition || _contentLenth < _rectViewport.rect.height)
            {
                _rectContent.anchoredPosition = new Vector2(_rectContent.anchoredPosition.x, 0);
            }
            else if (_contentLenth > _rectViewport.rect.height && _contentLenth - Mathf.Abs(_rectContent.anchoredPosition.y) < _rectViewport.rect.height)
            {
                _rectContent.anchoredPosition = new Vector2(_rectContent.anchoredPosition.x, (int)refreshDirection * (_contentLenth - _rectViewport.rect.height));
            }
        }

        _beginDataIndex = dataCount + 1;
        _endDataIndex = 0;
        
        if (refreshDirection == PosDir.Top)
        {
            _beginDataIndex = GetFirstDataIndexByContentPosition();
            InitTop2BottomByIndex(1);
        }
        else if (refreshDirection == PosDir.Bottom)
        {
            _endDataIndex = GetFirstDataIndexByContentPosition();
            InitBottom2TopByIndex(_showingItemList.Count);
            
        }
    }
    void RecycleAllItem()
    {
        List<ItemInfo> itemList = new List<ItemInfo>();
        foreach (var item in _dictItem.Values)
        {
            itemList.Add(item);
        }
        foreach (var item in itemList)
        {
            RecycleItem(item.go);
        }
    }

    void Refresh(bool isInitContentPosition = false)
    {
        InitList(isInitContentPosition);
        if (isRefreshAsync)
        {
            isUpdateItems = true;
        }
        else
        {
            LoopRefreshVertical();
        }
    }

    bool isUpdateItems;
    void UpdateItems() {
        if (isUpdateItems)
        {
            if (refreshDirection == PosDir.Top)
            {
                isUpdateItems = RefreshTop2Bottom();
            }
            else if(refreshDirection == PosDir.Bottom)
            {
                isUpdateItems = RefreshBottom2Top();
            }
        }
    }

    void LoopRefreshVertical()
    {
        if (refreshDirection == PosDir.Top)
        {
            if (RefreshTop2Bottom())
            {
                LoopRefreshVertical();
                return;
            }
        }
        else if(refreshDirection == PosDir.Bottom)
        {
            if (RefreshBottom2Top())
            {
                LoopRefreshVertical();
                return;
            }
        }
    }

    float lastValueY = 0;
    public void OnScrollValueChange(Vector2 vector)
    {
        if (Math.Abs(_rectContent.anchoredPosition.y - lastValueY) < 0.1)
        {
            return;
        }
        if (_rectContent.anchoredPosition.y - lastValueY > 0)
        {
            RefreshTop2Bottom();
        }
        else
        {
            RefreshBottom2Top();
        }
        lastValueY = _rectContent.anchoredPosition.y;
    }

    void InitTop2BottomByIndex(int index)
    {
        int showingCount = _showingItemList.Count;
        if (showingCount < index || showingCount == 0 || index == 0) return;
        ItemInfo item = _showingItemList[index - 1];

        int dataIndex = _beginDataIndex + index - 1;
        if (dataIndex > dataCount || dataIndex <= 0)
        {
            RecycleItem(item.go);
            InitTop2BottomByIndex(index);
        }
        else
        {
            _endDataIndex = dataIndex;
            RenderItem(item.go, dataIndex);
            item.rect.sizeDelta = RenderItemSize(dataIndex);
            item.dataIndex = dataIndex;
            //SetItemPosByIndex(index, PosDir.Bottom, true, item);
            item.rect.anchoredPosition = _arrDataIndex2Position[dataIndex - 1];
            InitTop2BottomByIndex(index + 1);
        }
    }

    void InitBottom2TopByIndex(int index)
    {
        int showingCount = _showingItemList.Count;
        if (showingCount < index || showingCount == 0 || index == 0) return;
        ItemInfo item = _showingItemList[index - 1];

        int dataIndex = _endDataIndex - (showingCount - index);
        if (dataIndex < 1)
        {
            RecycleItem(item.go);
            InitBottom2TopByIndex(index - 1);
        }
        else
        {
            _beginDataIndex = dataIndex;
            RenderItem(item.go, dataIndex);
            item.rect.sizeDelta = RenderItemSize(dataIndex);
            item.dataIndex = dataIndex;
            //SetItemPosByIndex(index, PosDir.Top, true, item);
            item.rect.anchoredPosition = _arrDataIndex2Position[dataIndex - 1];
            InitBottom2TopByIndex(index - 1);
        }
    }

    public bool RefreshTop2Bottom()
    {
        if (_endDataIndex >= dataCount)
        {
            return false;
        }
        int showingCount = _showingItemList.Count;
        ItemInfo bottomItem = null;
        if (showingCount > 0)
        {
            bottomItem = _showingItemList[showingCount - 1];
        }
        else
        {
            _beginDataIndex = 1;
        }

        bool isSet = false;
        if (showingCount <= 0 || InverseTransformY(bottomItem.rect, -1) > InverseTransformY(_rectViewport, -1))
        {
            _endDataIndex++;
            if (_endDataIndex < GetLastDataIndexByContentPosition())
            {
                Refresh();
                return false;
            }

            isSet = true;
            GetNewItem((item) =>
            {
                RenderItem(item.go, _endDataIndex);
                item.rect.sizeDelta = RenderItemSize(_endDataIndex);
                item.dataIndex = _endDataIndex;
                item.rect.anchoredPosition = _arrDataIndex2Position[_endDataIndex - 1];
                return true;
            }, PosDir.Bottom);
        }
        if (CheckOutOfRange(_showingItemList[0].go, PosDir.Top))
        {
            _beginDataIndex += 1;
        }
        return isSet;
    }

    public bool RefreshBottom2Top()
    {
        if (_beginDataIndex <= 1)
        {
            return false;
        }
        int showingCount = _showingItemList.Count;
        ItemInfo topItem = null;
        if (showingCount > 0)
        {
            topItem = _showingItemList[0];
        }
        else
        {
            _endDataIndex = dataCount;
        }

        bool isSet = false;
        if (showingCount <= 0 || InverseTransformY(topItem.rect, 1) < InverseTransformY(_rectViewport, 1))
        {
            _beginDataIndex--;
            if (_beginDataIndex > GetFirstDataIndexByContentPosition()) {
                Refresh();
                return false;
            }

            isSet = true;
            GetNewItem((item) =>
            {
                RenderItem(item.go, _beginDataIndex);
                item.rect.sizeDelta = RenderItemSize(_beginDataIndex);
                item.dataIndex = _beginDataIndex;
                item.rect.anchoredPosition = _arrDataIndex2Position[_beginDataIndex - 1];
                return true;
            }, PosDir.Top);
        }
        if (CheckOutOfRange(_showingItemList[_showingItemList.Count - 1].go, PosDir.Bottom))
        {
            _endDataIndex -= 1;
        }
        return isSet;
    }

    void GetNewItem(Func<ItemInfo, bool> callBack, PosDir dirction)
    {
        ItemInfo item;
        if (_itemPool.Count > 0)
        {
            item = _itemPool.Pop();
        }
        else
        {
            item = new ItemInfo(Instantiate(itemPrefab));
            item.go.transform.SetParentEx(_rectContent, false);
        }
        if (!_dictItem.ContainsKey(item.go))
        {
            _dictItem.Add(item.go, item);
            if ((int)dirction == 1)
            {
                _showingItemList.Insert(0, item);
            }
            else
            {
                _showingItemList.Add(item);
            }
        }
        item.go.transform.SetActiveEx(true);
        if (refreshDirection == PosDir.Top)
        {
            item.rect.anchorMax = new Vector2(0.5f, 1);
            item.rect.anchorMin = new Vector2(0.5f, 1);
            item.rect.pivot = new Vector2(0.5f, 1);
        }
        else if(refreshDirection == PosDir.Bottom)
        {
            item.rect.anchorMax = new Vector2(0.5f, 0);
            item.rect.anchorMin = new Vector2(0.5f, 0);
            item.rect.pivot = new Vector2(0.5f, 0);
        }
        callBack(item);
    }

    bool CheckOutOfRange(GameObject go, PosDir dir = PosDir.None)
    {
        if (!_dictItem.TryGetValue(go, out ItemInfo item))
        {
            return false;
        }

        bool isRecycle;
        if(dir == PosDir.Top)
        {
            isRecycle = InverseTransformY(item.rect, -1) > InverseTransformY(_rectViewport, 1);
        }
        else if(dir == PosDir.Bottom){
            isRecycle = InverseTransformY(item.rect, 1) < InverseTransformY(_rectViewport, -1);
        }
        else{
            isRecycle = InverseTransformY(item.rect, -1) > InverseTransformY(_rectViewport, 1) || InverseTransformY(item.rect, 1) < InverseTransformY(_rectViewport, -1);
        }

        if(isRecycle)
        {
            return RecycleItem(go);

        }
        return false;
    }

    bool RecycleItem(GameObject go)
    {
        if (!_dictItem.TryGetValue(go, out ItemInfo item))
        {
            item = new ItemInfo(go);
        }
        go.transform.SetActiveEx(false);
        _itemPool.Push(item);
        _dictItem.Remove(go);
        _showingItemList.Remove(item);
        return true;
    }

    float InverseTransformY(RectTransform trans, int direction)
    {
        if (direction == 1)
        {
            return _rect.InverseTransformPoint(trans.transform.position).y + trans.rect.height * (1 - trans.pivot.y);
        }
        else
        {
            return _rect.InverseTransformPoint(trans.transform.position).y - trans.rect.height * trans.pivot.y;
        }
    }

    void SetAllItemAnchorPivot(Vector2 min, Vector2 max, Vector2 pivot)
    {
        foreach (ItemInfo item in _dictItem.Values)
        {
            item.rect.pivot = new Vector2(0.5f, 0.5f);
            if (item.rect.anchorMin != min)
            {
                item.rect.anchorMin = min;
            }
            if (item.rect.anchorMax != max)
            {
                item.rect.anchorMax = max;
            }
            if (item.rect.pivot != pivot)
            {
                item.rect.pivot = pivot;
            }
        }
    }

    Vector2 GetJumpDataIndexPosition(int dataIndex) {
        if (_arrDataIndex2Position.Length == 0){
            return new Vector2(0, 0);
        }

        if (dataIndex < 1)
        {
            dataIndex = 1;
        }
        if (dataIndex > dataCount)
        {
            dataIndex = dataCount;
        }

        Vector2 itemPos = _arrDataIndex2Position[dataIndex - 1];
        if (refreshDirection == PosDir.Top || refreshDirection == PosDir.Bottom)
        {
            if (_rectContent.rect.height < _rectViewport.rect.height)
            {
                return new Vector2(0, 0);
            }

            if (_rectContent.rect.height - Mathf.Abs(itemPos.y) > _rectViewport.rect.height)
            {
                return new Vector2(0, -itemPos.y);
            }
            else
            {
                return new Vector2(0, (int)refreshDirection * (_rectContent.rect.height - _rectViewport.rect.height));
            }
        }
        return Vector2.zero;
    }

    //=============================================================
    public void SetRenderHandler(Action<GameObject, int> renderHandler)
    {
        _renderHandler = renderHandler;
        //Init();
        SetDataCount(dataCount);
    }

    public void SetRenderSizeHandler(Func<int, Vector2> renderSizeHandler)
    {
        _renderSizeHandler = renderSizeHandler;
    }

    public void SetDataCount(int dataCount, bool isInitContentPosition = false)
    {
        this.dataCount = dataCount;
        Refresh(isInitContentPosition);
    }

    public void Release()
    {
        _renderHandler = null;
        _renderSizeHandler = null;

        if (_ReleaseDict != null) {

            foreach (var item in _ReleaseDict.Values) {
                item.Release();
            }
            _ReleaseDict = null;
        }
        // foreach (var item in _itemPool)
        // {
        //     Destroy(item.go);
        // }

        // foreach (var item in _dictItem)
        // {
        //     Destroy(item.Value.go);
        // }
        // _showingItemList = new List<ItemInfo>();
    }

    public void Jump2DataIndex(int dataIndex)
    {
        _rectContent.anchoredPosition = GetJumpDataIndexPosition(dataIndex);
        Refresh(false);
    }

    public void Scroll2DataIndex(int dataIndex, float duration)
    {
        Vector2 pos = GetJumpDataIndexPosition(dataIndex);
        _rectContent.DOAnchorPos(pos, duration);
    }

    //=================================================================================
    Vector2 RenderItemSize(int dataIndex)
    {
        if (_renderSizeHandler != null)
        {
            return _renderSizeHandler(dataIndex - 1);
        }
        else
        {
            return _rectPrefab.sizeDelta;
        }
    }

    void RenderItem(GameObject go, int dataIndex)
    {
        _renderHandler?.Invoke(go, dataIndex - 1);
    }

    // ==================================================================================
    //void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    //{
    //}

    //void IDragHandler.OnDrag(PointerEventData eventData)
    //{
    //    //GetFirstDataIndexByContentPosition();
    //}

    //void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    //{
    //}
}
