using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public enum TweenObjectsMoveType
{
    //Left2Right,
    //Right2Left,
    //Bottom2Top,
    ScaleOut,
    ScaleIn,
    MoveAnchorX,
    MoveAnchorY,
}

[Serializable]
public struct TransformAni
{
    public RectTransform rectTrans;
    public int index;
    public TweenObjectsMoveType TweenType;
    public float moveDistance;
    public Vector3 beginScale;
    public Ease ease;
    public bool notUseDefaultValue;
    public float duration;
    public float interval;
}

public class TweenObjectsMove : MonoBehaviour
{
    public float defaultDuration = 0.3f;
    public float defaultInterval = 0.1f;
    public List<TransformAni> transformAniList;
    Sequence sequence;

    [HideInInspector]
    public CanvasGroup canvasGroup;
    public bool isTest = true;

    public void startTween() {
        completeAllAni();

        sequence = DOTween.Sequence();

        for (int i = 0; i < transformAniList.Count; i++) {
            if (transformAniList[i].rectTrans != null) {

                TransformAni transformAni = transformAniList[i];

                float interval = defaultInterval;
                float duration = defaultDuration;
                if (transformAni.notUseDefaultValue) {
                    interval = transformAni.interval;
                    duration = transformAni.duration;
                }

                RectTransform trans = transformAni.rectTrans;
                float startTime = interval * transformAni.index;
                //float moveDistance;
                Vector3 beginScale;
                switch (transformAni.TweenType)
                {
                    //case TweenObjectsMoveType.Left2Right:
                    //    if (transformAni.moveDistance != 0) { moveDistance = transformAni.moveDistance; }
                    //    else { moveDistance = UnityEngine.Screen.width; }
                    //    MovePosX(trans, startTime, duration, transformAni.ease, moveDistance, -1);
                    //    break;

                    //case TweenObjectsMoveType.Right2Left:
                    //    if (transformAni.moveDistance != 0) { moveDistance = transformAni.moveDistance; }
                    //    else { moveDistance = UnityEngine.Screen.width; }
                    //    MovePosX(trans, startTime, duration, transformAni.ease, moveDistance, 1);
                    //    break;
                    //case TweenObjectsMoveType.Bottom2Top:
                    //    if (transformAni.moveDistance != 0) { moveDistance = transformAni.moveDistance; }
                    //    else { moveDistance = UnityEngine.Screen.height; }
                    //    MovePosY(trans, startTime, duration, transformAni.ease, moveDistance, -1);
                    //    break;

                    case TweenObjectsMoveType.ScaleOut:
                        if (transformAni.beginScale != Vector3.zero) { beginScale = transformAni.beginScale; }
                        else { beginScale = new Vector3(0.01f, 0.01f, 0.01f); }
                        ScaleAni(trans, startTime, duration, transformAni.ease, beginScale);
                        break;

                    case TweenObjectsMoveType.ScaleIn:
                        if (transformAni.beginScale != Vector3.zero) { beginScale = transformAni.beginScale; }
                        else { beginScale = new Vector3(10f, 10f, 10f); }
                        ScaleAni(trans, startTime, duration, transformAni.ease, beginScale);
                        break;
                    case TweenObjectsMoveType.MoveAnchorX:
                        MoveAnchorPosX(trans, startTime, duration, transformAni.ease, transformAni.moveDistance);
                        break;

                    case TweenObjectsMoveType.MoveAnchorY:
                        MoveAnchorPosY(trans, startTime, duration, transformAni.ease, transformAni.moveDistance);
                        break;

                    default:
                        break;
                }
            }
        }
    }

    void MoveAnchorPosX(RectTransform objTransfom, float startTime, float duration, Ease ease, float moveDistance)
    {
        if (sequence == null)
        {
            return;
        }
        Vector3 pos = objTransfom.anchoredPosition3D;
        float endX = pos.x;
        pos.x = pos.x + moveDistance;
        objTransfom.anchoredPosition = pos;
        sequence.Insert(startTime, objTransfom.DOAnchorPosX(endX, duration).SetEase(ease));
    }

    void MoveAnchorPosY(RectTransform objTransfom, float startTime, float duration, Ease ease, float moveDistance)
    {
        if (sequence == null)
        {
            return;
        }
        Vector3 pos = objTransfom.anchoredPosition3D;
        float endY = pos.y;
        pos.y = pos.y + moveDistance;
        objTransfom.anchoredPosition = pos;
        sequence.Insert(startTime, objTransfom.DOAnchorPosY(endY, duration).SetEase(ease));
    }

    void MovePosX(Transform objTransfom, float startTime, float duration, Ease ease, float moveDistance, int moveDirection = 1)
    {
        if (sequence == null)
        {
            return;
        }
        Vector3 pos = objTransfom.localPosition;
        float endX = pos.x;
        pos.x = pos.x + moveDistance * moveDirection;
        objTransfom.localPosition = pos;
        sequence.Insert(startTime, objTransfom.DOLocalMoveX(endX, duration).SetEase(ease));
    }

    void MovePosY(Transform objTransfom, float startTime, float duration, Ease ease, float moveDistance, int moveDirection = 1)
    {
        if (sequence == null)
        {
            return;
        }
        Vector3 pos = objTransfom.localPosition;
        float endY = pos.y;
        pos.y = pos.y + moveDistance * moveDirection;
        objTransfom.localPosition = pos;
        sequence.Insert(startTime, objTransfom.DOLocalMoveY(endY, duration).SetEase(ease));
    }

    void ScaleAni(Transform objTransfom, float startTime, float duration, Ease ease, Vector3 beginScale)
    {
        if (sequence == null)
        {
            return;
        }
        Vector3 endScale = objTransfom.localScale;
        objTransfom.localScale = beginScale;
        sequence.Insert(startTime, objTransfom.DOScale(endScale, duration).SetEase(ease));
    }


    void completeAllAni()
    {
        foreach (TransformAni t in transformAniList) {
            t.rectTrans.DOComplete();
        }

        if (sequence != null) {
            sequence.Complete();
            sequence = null;
        }
    }

    void OnEnable() {
        if (isTest) {
            startTween();
        }
    }

    void Start()
    {
        //startTween();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy() {
        if (sequence != null) {
            sequence.Kill();
            sequence = null;
        }
    }
}
