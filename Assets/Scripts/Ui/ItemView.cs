using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    public Transform Item;
    Action<GameObject, int> _RenderHandler;
    Queue<Transform> _ItemPool;
    List<Transform> _ShowingList = new List<Transform>();
    Dictionary<GameObject, ItemBase> _ReleaseDict;

    public void Init(Dictionary<GameObject, ItemBase> releaseDict = null)
    {
        _ReleaseDict = releaseDict;
        _ItemPool = new Queue<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.SetActiveEx(false);
            _ItemPool.Enqueue(child);
        }
    }

    public void SetDataCount(int count) {

        int excuteCount = Math.Max(_ShowingList.Count, count);

        Transform item = null;

        for (int i = 0; i < excuteCount; i++) {
            if (i + 1 <= count)
            {
                if (_ShowingList.Count < i + 1)
                {
                    if (_ItemPool.Count > 0)
                    {
                        item = _ItemPool.Dequeue();
                        _ShowingList.Add(item);
                    }
                    else
                    {
                        item = Instantiate(Item);
                        item.SetParent(transform, false);
                        _ShowingList.Add(item);
                    }
                }
                _ShowingList[i].SetActiveEx(true);
                _RenderHandler(_ShowingList[i].gameObject, i);
            }
            else {
                int index = _ShowingList.Count - 1;
                item = _ShowingList[index];
                _ShowingList.RemoveAt(index);
                item.SetActiveEx(false);
                _ItemPool.Enqueue(item);
            }
        }
    }

    public void SetRenderHandler(Action<GameObject, int> handler) {
        _RenderHandler = handler;
    }

    public void Release() {
        _RenderHandler = null;

        if (_ReleaseDict != null) { 
            foreach (var item in _ReleaseDict.Values) { item.Release(); }
        }
        _ReleaseDict = null;
    }

}
