using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using Unity.Mathematics;
using SimpleJSON;

public class ResAnim : Singleton<ResAnim>
{
    public enum ResType { 
        gold,
        Diamonds,
    }

    //U3dObj
    Queue<GameObject> goldUIQuene;
    Queue<GameObject> DiamondsUIQuene;

    string GoldAnimUI = "GoldAnimUI";
    string DiamondAnimUI = "DiamondAnimUI";

    public ResAnim()
    {
        goldUIQuene = new Queue<GameObject>();
        DiamondsUIQuene = new Queue<GameObject>();
    }

    //itemList = [cfgid:count, ]
    public void ShowUIAnim(JSONNode itemList, Vector3 beginPos)
    {
        foreach (var item in itemList)
        {
            int itemCfgId = Convert.ToInt32(item.Key);
            ShowUIAnim(itemCfgId, beginPos, item.Value);
        }
    }

    public void ShowUIAnim(int itemCfgId, Vector3 beginPos, int count) {
        if (itemCfgId == 100001)
        {
            ShowUIAnim(ResType.gold, beginPos, count);
        }
        else if (itemCfgId == 100002) {
            ShowUIAnim(ResType.Diamonds, beginPos, count);

        }
    }

    public void ShowUIAnim(ResType type, Vector3 beginPos, int count) {

        //int resCount = count / 100;
        //resCount = Mathf.Min(10, resCount);

        int resCount = 10;

        TopShowResView topShowResView = UiMgr.GetView<TopShowResView>();

        if (topShowResView == null) {
            return;
        }

        Vector3 endPos = Vector3.zero;
        Queue<GameObject> quene = null;

        if (type == ResType.gold) {
            endPos = topShowResView.GetGoldPos();
            quene = goldUIQuene;
        }
        else if(type == ResType.Diamonds)
        {
            endPos = topShowResView.GetDiamondsPos();
            quene = DiamondsUIQuene;
        }

        for (int i = 0; i < resCount; i++) {
            int index = i;

            GetUIItem(type, (item) =>
            {
                item.transform.SetActiveEx(true);
                item.transform.position = beginPos;
                StartUIAnim(item.transform, endPos, index, () => {
                    item.SetActive(false);
                    quene.Enqueue(item);
                });
            });
        }
    }

    //float duration = 0.25f;
    float interval = 0.1f;
    void StartUIAnim(Transform item, Vector3 endPos, int index, Action endCalback) {

        System.Random random = new System.Random();
        float randomAngle = (float)(random.NextDouble() * 360);

        Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);
        //Quaternion rotation = quaternion.Euler(0, 0, randomAngle);

        Vector3 dir = rotation * Vector3.right;
        float randomDistance = (float)random.Next(200, 1500) / 10;
        //Debug.Log($"{randomAngle}  {dir}  {index * interval} ========");


        Sequence sequence = DOTween.Sequence();

        //sequence.SetDelay(index * 0.05f);

        //sequence.Append(item.DOMove(item.position + dir * randomDistance, 0.1f + index * interval).SetEase(Ease.Linear));
        sequence.Append(item.DOMove(item.position + dir * randomDistance, 0.5f).SetEase(Ease.OutExpo));
        sequence.AppendInterval(index * interval);

        sequence.Append(item.DOMove(endPos, 0.5f).SetEase(Ease.InBack));

        sequence.AppendCallback(() =>
        {
            endCalback();
        });
    }

    void GetUIItem(ResType type, Action<GameObject> callback) {
        Queue<GameObject> quene = null;
        string resName = "";

        if (type == ResType.gold) {
            quene = goldUIQuene;
            resName = GoldAnimUI;
        }
        else if(type == ResType.Diamonds)
        {
            quene = DiamondsUIQuene;
            resName = DiamondAnimUI;
        }

        if (quene.Count > 0)
        {
            callback(quene.Dequeue());
        }
        else {
            ResMgr.instance.LoadGameObjectAsync(resName, (go) =>
            {
                go.transform.SetParent(UiMgr.Manager.UIRoot.transform, false);
                callback(go);
            });
        }
    }
}
