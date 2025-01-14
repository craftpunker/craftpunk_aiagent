using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimUtil
{
    public static Sequence StartRotation(RectTransform trans) {

        trans.localRotation = Quaternion.Euler(0f, 0f, 0f);

        Sequence sequence = DOTween.Sequence();

        //sequence.SetDelay(index * 0.05f);

        //sequence.Append(item.DOMove(item.position + dir * randomDistance, 0.1f + index * interval).SetEase(Ease.Linear));
        sequence.Append(trans.DORotate(new Vector3(0, 0, -180), 4));
        sequence.Append(trans.DORotate(new Vector3(0, 0, -360), 4));

        //sequence.AppendCallback(() =>
        //{
        //    trans.localRotation = Quaternion.Euler(0f, 0f, 0f);
        //});

        sequence.SetLoops(-1, LoopType.Incremental);



        return sequence;
    }

}
