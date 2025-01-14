
using UnityEngine;
using DG.Tweening;

using UnityEngine.UI;

public class MessageView : UiBase
{
    private Text m_text;


    protected override void OnInit()
    {
        
    }

    protected override void OnShow()
    {
        //EventDispatcher<int>.instance.AddEvent(EventName.UI_ShowTip, ShowDoTweenTip);
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        //EventDispatcher<int>.instance.RemoveEvent(EventName.UI_ShowTip, ShowDoTweenTip);
    }

    protected override void OnDestroy()
    {

    }

    //private void ShowDoTweenTip(string evtName, int[] args)
    //{
    //    //string tip = (string)args[0];

    //    ResMgr.instance.LoadGameObjectAsync("TipUI", (go) =>
    //    {
    //        var rect = go.GetComponent<RectTransform>();
    //        go.transform.parent = transform;
    //        rect.anchoredPosition = Vector3.zero;
    //        go.GetComponent<Text>().text = args[0].ToString();
    //        rect.DOAnchorPos3DY(300, 3).onComplete += () => { 
    //            go.gameObject.SetActive(false);
    //            ResMgr.instance.ReleaseGameObject(go);
    //        };
    //    });
    //}
    
    public void ShowTip(string tips)
    {
        //string tip = (string)args[0];
        ResMgr.instance.LoadGameObjectAsync("TipUI", (go) =>
        {
            var rect = go.GetComponent<RectTransform>();
            go.transform.SetParent(transform, false);
            rect.anchoredPosition3D = new Vector3(0, 200, 0);
            rect.SetLocalScale(1f);
            go.GetComponent<Text>().text = tips;
            rect.DOAnchorPos3DY(500, 3).onComplete += () =>
            {
                go.gameObject.SetActive(false);
                ResMgr.instance.ReleaseGameObject(go);
            };
        });
    }
}
