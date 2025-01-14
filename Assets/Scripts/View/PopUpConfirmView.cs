
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PopUpConfirmMsg
{
    public string Content;
    public UnityAction Btn1Func;
    public UnityAction Btn2Func;
    public string Btn1Txt;
    public string Btn2Txt;
    public bool showWaitImg;
}

//二次确定框
public class PopUpConfirmView : UiBase
{
    U3dObj confirmPanel;

    U3dObj txtContent;
    U3dObj btn1;
    U3dObj btn2;
    U3dObj btnTxt1;
    U3dObj btnTxt2;
    U3dObj imgWait;
    bool showWaitImg;
    //float waitTime = 10;
    //float currTime;


    protected override void OnInit()
    {
        confirmPanel = Find("ConfirmPanel");
        txtContent = confirmPanel.Find("TxtContent");
        btn1 = confirmPanel.Find("Btn1");
        btn2 = confirmPanel.Find("Btn2");
        btnTxt1 = btn1.Find("Text");
        btnTxt2 = btn2.Find("Text");
        imgWait = Find("ImgWait");
    }

    protected override void OnShow()
    {
        showWaitImg = false;
        //currTime = 0;
        imgWait.SetActive(false);
        if (Args.Length == 0)
        {
            confirmPanel.SetActive(false);
        }
        else
        {
            confirmPanel.SetActive(true);

            PopUpConfirmMsg warningMsg = Args[0] as PopUpConfirmMsg;
            txtContent.Text.text = warningMsg.Content;
            btn1.Button.onClick.AddListener(warningMsg.Btn1Func);
            btn2.Button.onClick.AddListener(warningMsg.Btn2Func);
            btnTxt1.Text.text = warningMsg.Btn1Txt;
            btnTxt2.Text.text = warningMsg.Btn2Txt;

            if (warningMsg.showWaitImg)
            {
                btn2.Button.onClick.AddListener(() =>
                {
                    showWaitImg = warningMsg.showWaitImg;
                    imgWait.SetActive(true);
                    Vector3 doR0tate_vct = new Vector3(0, 0, 360); //设置旋转的角度
                    float useTime_r = 5;
                    imgWait.transform.DORotate(doR0tate_vct, useTime_r, RotateMode.FastBeyond360).SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart);
                    confirmPanel.SetActive(false);
                });
            }
        }
    }

    protected override void OnUpdate()
    {
        //if (showWaitImg)
        //{
        //    currTime += Time.deltaTime;

        //    if (currTime > waitTime)
        //    {
        //        UiMgr.Close<PopUpConfirmView>();
        //    }
        //}
    }

    protected override void OnHide()
    {
        btn1.Button.onClick.RemoveAllListeners();
        btn2.Button.onClick.RemoveAllListeners();
    }

    protected override void OnDestroy()
    {

    }
}
