#if _CLIENTLOGIC_
using DG.Tweening;
using UnityEngine;

public class DoTweenAnimUtil
{
    public static void ScaleAni(Transform objTransfom, float startTime, float duration, Ease ease, Vector3 beginScale)
    {
        Vector3 endScale = objTransfom.localScale;
        objTransfom.localScale = beginScale;
        DOTween.Sequence().Insert(startTime, objTransfom.DOScale(endScale, duration).SetEase(ease));
    }
}
#endif