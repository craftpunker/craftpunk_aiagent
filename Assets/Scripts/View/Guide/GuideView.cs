using Battle;
using SimpleJSON;
using System.Reflection;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class GuideView : UiBase
{
    public GuideNode guideNode;
    JSONNode cfg;

    U3dObj LayoutMask;
    U3dObj Mask;
    U3dObj TargetReplace;
    public HollowOutMask HollowOutMask;
    U3dObj Button;
    U3dObj Finger;

    Animator fingerAnimator;


    U3dObj LayoutTalk;

    U3dObj BgTalk;
    U3dObj ImgTalk;

    RectTransform _Target;

    RectTransform _TargetCopy;

    protected override void OnInit()
    {
        LayoutMask = Find("LayoutMask");
        Mask = LayoutMask.Find("Mask");
        HollowOutMask = Mask.transform.GetComponent<HollowOutMask>();
        TargetReplace = LayoutMask.Find("TargetReplace");
        Finger = Find("Finger");
        fingerAnimator = Finger.transform.GetComponent<Animator>();
        fingerAnimator.SetInteger("status", 0);

        Button = LayoutMask.Find("Button");
        SetOnClick(Button, OnClickButton);

        LayoutTalk = Find("LayoutTalk");
        SetOnClick(LayoutTalk, OnClickTalk);

        BgTalk = LayoutTalk.Find("BgTalk");
        ImgTalk = BgTalk.Find("ImgTalk");

        //UiMgr.CreateUISpecialEffect(40001, (se) =>
        //{
        //    curSpecialEffect = se;
        //    curSpecialEffect.transform.SetParent(BgTalk.transform);
        //    curSpecialEffect.transform.localPosition = ImgTalk.transform.localPosition;
        //    curSpecialEffect.transform.localScale = new Vector3(1000, 1000, 1000);
        //    curSpecialEffect.DoAnim(AnimType.Idle);
        //});
    }

    public void SetTarget(RectTransform target) {
        _Target = target;
    }

    UISpecialEffect curSpecialEffect;
    Sequence fingerSequence;
    //args = [guideNode]
    protected override void OnShow()
    {
        guideNode = (GuideNode)Args[0];
        cfg = guideNode.cfg;
        LayoutMask.SetActive(false);
        LayoutTalk.SetActive(false);

        Button.SetActive(false);

        Finger.SetActive(false);
        fingerAnimator.SetInteger("status", 0);
        Finger.Image.rectTransform.anchoredPosition = Vector2.zero;
        Finger.SetSprite("GuideAtlas", "hand.00001");

        Color color = HollowOutMask.color;
        color.a = (float)guideNode.cfg["maskA"] / 1000;
        
        HollowOutMask.color = color;
        LayoutTalk.Image.color = color;

        if (fingerSequence != null) {
            fingerSequence.Kill();
            fingerSequence = null;
        }

        if (guideNode.IsTalk)
        {
            LayoutTalk.SetActive(true);
            LayoutTalk.Text.text = cfg["strArg1"];
            //30028
        }
        else if (guideNode.IsButton)
        {
            Button.SetActive(true);
            Button.Image.color = color;
            Finger.SetActive(true);
            LayoutMask.SetActive(true);
            _Target = guideNode.Target;
            HollowOutMask.SetTargetObj(_Target, Vector3.zero, Vector3.zero);


            _TargetCopy = GameObject.Instantiate(guideNode.Target);
            UIEventHandler[] UIEventHandlerList = _TargetCopy.GetComponentsInChildren<UIEventHandler>();
            if (UIEventHandlerList.Length > 0)
            {
                for (int i = UIEventHandlerList.Length - 1; i >= 0; i--)
                {
                    if (UIEventHandlerList[i].gameObject != _TargetCopy.gameObject)
                    {
                        GameObject.Destroy(UIEventHandlerList[i]);
                    }
                }
            }
            UIEventHandler.Get(_TargetCopy.gameObject).SetOnClick(OnClickButton, "ui_button_click");
            _TargetCopy.SetParent(LayoutMask.transform, false);
            _TargetCopy.position = guideNode.Target.position;

            int fingerPosCfg = guideNode.cfg["fingerPos"];
            if (fingerPosCfg == 1)
            {
                Finger.RectTransform.rotation = Quaternion.Euler(0, 0, 0);
                fingerAnimator.SetInteger("status", 2);
            }
            else if (fingerPosCfg == 2)
            {
                Finger.RectTransform.rotation = Quaternion.Euler(0, 0, 90);
                fingerAnimator.SetInteger("status", 1);
            }
            else if (fingerPosCfg == 3)
            {
                Finger.RectTransform.rotation = Quaternion.Euler(0, 180, 0);
                fingerAnimator.SetInteger("status", 1);
            }
            else if (fingerPosCfg == 4)
            {
                Finger.RectTransform.rotation = Quaternion.Euler(0, 0, 0);
                fingerAnimator.SetInteger("status", 1);
            }
        }
        else if (guideNode.IsDragSoldier)
        {
            LayoutMask.SetActive(true);
            Finger.SetActive(true);

            HollowOutMask.SetTargetObj(TargetReplace.RectTransform, Vector3.zero, Vector3.zero);

            _Target = guideNode.Target;

            //fingerAnimator.SetInteger("status", 0);
            Finger.RectTransform.rotation = Quaternion.Euler(0, 0, 0);

            Vector3 beginPos = _Target.transform.position;

            string key = guideNode.cfg["strArg2"];
            JSONNode gridCenterCfg = GameData.instance.TableJsonDict["GridConf"][key]["center"];

            Vector3 gridWorldPos = new Vector3((float)gridCenterCfg[0] / 1000, (float)gridCenterCfg[1] / 1000, 0);
            Vector3 endPos = Camera.main.WorldToScreenPoint(gridWorldPos);


            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(LayoutMask.RectTransform, _Target);
            Vector2 rightMax = bounds.max;
            Vector2 rightMin = bounds.min;

            Vector2 leftMax = Camera.main.WorldToScreenPoint(new Vector3(gridWorldPos.x + 2.28f, gridWorldPos.y + 3, gridWorldPos.z));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(LayoutMask.RectTransform, leftMax, null, out leftMax);
            Vector2 leftMin = Camera.main.WorldToScreenPoint(new Vector3(gridWorldPos.x - 2.28f, gridWorldPos.y - 3, gridWorldPos.z));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(LayoutMask.RectTransform, leftMin, null, out leftMin);

            Vector2 max = new Vector2(rightMax.x, Mathf.Max(rightMax.y, leftMax.y));
            Vector2 min = new Vector2(leftMin.x, Mathf.Min(rightMin.y, leftMin.y));

            TargetReplace.RectTransform.sizeDelta = new Vector2(max.x - min.x, max.y - min.y);
            TargetReplace.RectTransform.anchoredPosition = new Vector2(min.x + (max.x - min.x) / 2, min.y + (max.y - min.y) / 2);

            Finger.transform.position = beginPos;
            Finger.SetActive(false);

            Vector3 moveEndPos = endPos;
            moveEndPos.y += 60;

            StartDragFingerAnim(beginPos, moveEndPos);
        }
        else if (guideNode.IsUseBattleSkill) {
            LayoutMask.SetActive(true);
            Finger.SetActive(true);

            //BattleView battleView = UiMgr.GetView<BattleView>();
            _Target = guideNode.Target;

            Vector3 beginPos = _Target.position;

            beginPos.y += _Target.rect.height / 2; 

            Vector3 worldPos = guideNode.Entity.Fixv3LogicPosition.ToVector3();
            Vector3 endPos = Camera.main.WorldToScreenPoint(worldPos);

            //HollowOutMask.SetTargetObj(_Target, Vector3.zero, Vector3.zero);

            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(LayoutMask.RectTransform, _Target);

            Vector2 max = new Vector2(Mathf.Max(bounds.max.x, endPos.x), endPos.y);
            Vector2 min = new Vector2(Mathf.Min(bounds.min.x, endPos.x), bounds.min.y);

            TargetReplace.RectTransform.sizeDelta = new Vector2(max.x - min.x, max.y - min.y);
            TargetReplace.RectTransform.anchoredPosition = new Vector2(min.x + (max.x - min.x) / 2, min.y + (max.y - min.y) / 2);
            HollowOutMask.SetTargetObj(TargetReplace.RectTransform, Vector3.zero, Vector3.zero);

            StartDragFingerAnim(beginPos, endPos);
        }

        HollowOutMask.SetAnimPopulateMeshCallBack((min, max) =>
        {
            if (_TargetCopy != null)
            {
                _TargetCopy.position = guideNode.Target.position;
            }

            Button.RectTransform.anchoredPosition = new Vector2(min.x + (max.x - min.x) / 2, min.y + (max.y - min.y) / 2);
            Button.RectTransform.sizeDelta = new Vector2(max.x - min.x + 5, max.y - min.y + 5);
            //Button.RectTransform.sizeDelta = new Vector2(max.x - min.x + 1, max.y - min.y + 1);
            UpdateFingerPos();
        });
    }

    void StartDragFingerAnim(Vector3 beginPos, Vector3 endPos) {
        fingerSequence = DOTween.Sequence();

        //µã»÷
        fingerSequence.AppendInterval(0.1f);
        fingerSequence.AppendCallback(() => {
            Finger.SetActive(true);
            Finger.transform.position = beginPos;
            fingerAnimator.SetInteger("status", 3);
        });
        fingerSequence.AppendInterval(1.5f);

        //ÒÆ¶¯
        fingerSequence.Append(Finger.transform.DOMove(endPos, 1f));

        //ÊÍ·Å
        fingerSequence.AppendCallback(() =>
        {
            fingerAnimator.SetInteger("status", 4);
        });
        fingerSequence.AppendInterval(1.5f);

        //Òþ²Ø
        fingerSequence.AppendCallback(() =>
        {
            Finger.SetActive(false);
        });
        fingerSequence.AppendInterval(2);

        fingerSequence.SetLoops(999999999);
    }

    void UpdateFingerPos() {
        int fingerPosCfg = guideNode.cfg["fingerPos"];
        Vector2 pos = Button.RectTransform.anchoredPosition;

        if (fingerPosCfg == 1)
        {
            //pos.y = pos.y - Button.RectTransform.sizeDelta.y / 2 - Finger.RectTransform.sizeDelta.y / 2;
            pos.y = pos.y - Button.RectTransform.sizeDelta.y / 2;
        }
        else if (fingerPosCfg == 2)
        {
            //pos.y = pos.y + Button.RectTransform.sizeDelta.y / 2 + Finger.RectTransform.sizeDelta.x / 2;
            pos.y = pos.y + Button.RectTransform.sizeDelta.y / 2;
        }
        else if (fingerPosCfg == 3) {
            //pos.x = pos.x - Button.RectTransform.sizeDelta.x / 2 - Finger.RectTransform.sizeDelta.x / 2;
            pos.x = pos.x - Button.RectTransform.sizeDelta.x / 2;
        }
        else if (fingerPosCfg == 4) {
            //pos.x = pos.x + Button.RectTransform.sizeDelta.x / 2 + Finger.RectTransform.sizeDelta.y / 2;
            pos.x = pos.x + Button.RectTransform.sizeDelta.x / 2;
        }

        Finger.RectTransform.anchoredPosition = pos;
    }

    void OnClickTalk() {
        guideNode.NextStep();
        Close();
    }

    void OnClickButton()
    {
        guideNode.view.TriggerGuide(guideNode);
        Close();
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        if (_TargetCopy != null) {
            UIEventHandler.Clear(_TargetCopy.gameObject);
            GameObject.Destroy(_TargetCopy.gameObject);
            _TargetCopy = null;
        }

        _Target = null;
        HollowOutMask.SetTargetObj(TargetReplace.RectTransform, Vector3.zero, Vector3.zero);

        if (fingerSequence != null)
        {
            fingerSequence.Kill();
            fingerSequence = null;
        }
    }

    protected override void OnDestroy()
    {
        guideNode = null;
        _Target = null;

        if (fingerSequence != null)
        {
            fingerSequence.Kill();
            fingerSequence = null;
        }

        if (curSpecialEffect != null) {
            curSpecialEffect.Release();
            curSpecialEffect = null;
        }

    }
}
