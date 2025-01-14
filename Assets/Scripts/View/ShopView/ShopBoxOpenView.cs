using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using Battle;
using UnityEngine.UIElements;

public class ShopBoxOpenView : UiBase
{
    U3dObj Mask;

    U3dObj LayoutBox;
    public U3dObj Box;
    private U3dObj ImgLight;
    private Animator boxAnimator;

    U3dObj LayoutShowReward;
    U3dObj ScrollView;
    Dictionary<GameObject, ItemBase> DictItems;

    public U3dObj BtnClose;
    U3dObj BtnContine;
    U3dObj TxtCost;
    U3dObj Toggle;

    U3dObj Mask2;

    ShopView.TreasureBoxData data;

    //Action callback;

    protected override void OnInit()
    {
        Mask = Find("Mask");
        SetOnClick(Mask, () => Close());

        LayoutBox = Find("LayoutBox");
        Box = LayoutBox.Find("Box");
        ImgLight = LayoutBox.Find("ImgLight");
        SetOnClick(Box, OnClickBox);
        boxAnimator = Box.gameObject.GetComponent<Animator>();

        LayoutShowReward = Find("LayoutShowReward");
        ScrollView = LayoutShowReward.Find("ScrollView");

        DictItems = new Dictionary<GameObject, ItemBase>();
        ScrollView.ItemView.Init(DictItems);
        ScrollView.ItemView.SetRenderHandler(OnRenderItems);

        BtnClose = LayoutShowReward.Find("BtnClose");
        SetOnClick(BtnClose, OnClickClose);

        BtnContine = LayoutShowReward.Find("BtnContine");
        SetOnClick(BtnContine, OnClickBox);

        TxtCost = BtnContine.Find("TxtCost");

        Toggle = LayoutShowReward.Find("Toggle");

        Mask2 = Find("Mask2");

        Vector3 doR0tate_vct = new Vector3(0, 0, 360); //设置旋转的角度
        float useTime_r = 5;
        ImgLight.transform.DORotate(doR0tate_vct, useTime_r, RotateMode.FastBeyond360).SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Incremental);
    }

    public void OnClickClose() {
        Close();
    }

    //UISpecialEffect curSpecialEffect;

    float cost;
    string costType;
    int animCfg;
    protected override void OnShow()
    {
        
        data = (ShopView.TreasureBoxData)Args[0];
        LayoutBox.SetActive(false);
        LayoutShowReward.SetActive(false);

        animCfg = data.cfg["animCfg"];

        //UiMgr.CreateUISpecialEffect(animCfg, (se) =>
        //{
        //    Debug.Log(1111);
        //    curSpecialEffect = se;
        //});

        //SpecialEffectFactory.instance.CreateSpecialEffect(animCfg, FixVector3.Zero, (se) =>
        //{
        //    OnCreateSe(se);
        //});

        //UiMgr.CreateUISpecialEffect(animCfg, (se) =>
        //{
        //    Debug.Log(222);
        //    OnCreateSe(se);
        //});

        JSONNode price = data.cfg["price"][0];

        if (price == null) {
            costType = string.Empty;
            cost = 0;
            TxtCost.Image.transform.SetActiveEx(false);
        }
        else {
            costType = price[0];
            cost = price[1];

            TxtCost.Image.transform.SetActiveEx(true);
            JSONNode itemCfg = GameData.instance.TableJsonDict["ItemConf"][costType];

            TxtCost.SetSprite(itemCfg["iconAtlas"], itemCfg["icon"]);
        }

        TxtCost.Text.text = cost.ToString();

        //Mask2.SetActive(false);


        Mask2.SetActive(true);
        MonoTimeMgr.instance.SetTimeAction(0.3f, () =>
        {
            OnClickBox();
        });

        AudioMgr.instance.PlayLoop("ui_chest_wait");
    }

    public void OnClickBox()
    {
        int boughtCount = ShopData.GetTreasureBoughtCount(data.cfg["cfgId"].ToString());
        if (data.cfg["dailyFree"] <= boughtCount && cost > 0 && cost > GameData.instance.PermanentData["bag_info"][costType]["num"])
        {
            UiMgr.ShowTips("not enought item");
            return;
        }

        LayoutBox.SetActive(false);
        LayoutShowReward.SetActive(false);

        if (Toggle.Toggle.isOn)
        {
            BuyBox();
        }
        else
        {
            //if (curSpecialEffect == null)
            //{
            //    UiMgr.CreateUISpecialEffect(animCfg, (se) =>
            //    {
            //        Debug.Log(3333);
            //        OnCreateSe(se);
            //        ShowBoxAnim();
            //    });
            //}

            AudioMgr.instance.StopByKey("ui_chest_wait");
            AudioMgr.instance.PlayOneShot("ui_chest_open");

            //if (curSpecialEffect == null)
            //{
            //    SpecialEffectFactory.instance.CreateSpecialEffect(animCfg, FixVector3.Zero, (se) =>
            //    {
            //        OnCreateSe(se);
            //        ShowBoxAnim();
            //    });
            //}
            ShowBoxAnim();
        }
    }

    public void ShowBoxAnim()
    {
        LayoutBox.SetActive(true);
        Mask2.SetActive(true);
        Box.SetActive(true);

        var ItemConf = GameData.instance.TableJsonDict["ItemConf"];

        int boxCfgId = data.cfg["itemCfgId"];

        var animValue = boxCfgId - 300000;
        boxAnimator.SetInteger("value", animValue);

        MonoTimeMgr.instance.SetTimeAction(0.5f, () =>
        {
            animValue += 10;
            boxAnimator.SetInteger("value", animValue);

            MonoTimeMgr.instance.SetTimeAction(0.1f, () =>
            {
                BuyBox();
            });
        });

        //curSpecialEffect.DoAnim(AnimType.Move, () => {
        //    BuyBox();
        //});

        //curSpecialEffect.DoAnim(AnimType.Move);
        //MonoTimeMgr.instance.SetTimeAction(0.1f, () =>
        //{
        //    BuyBox();
        //});
    }

    void BuyBox()
    {
        //if (curSpecialEffect != null)
        //{
        //    curSpecialEffect.Release();
        //    curSpecialEffect = null;
        //}
        ShopData.C2S_SHOP_BUY_BOX(data.cfg["cfgId"]);
        Box.SetActive(false);
    }

    //void OnCreateSe(UISpecialEffect se) {
    //    var trans = se.transform;
    //    //trans.GetComponent<Image>().raycastTarget = true;
    //    //SetOnClick(se.gameObject, OnClickBox);
    //    trans.SetParent(LayoutBox.transform);

    //    Vector3 pos = Box.transform.localPosition;
    //    //pos.z += 0.1f;
    //    //trans.localPosition = Box.transform.localPosition;
    //    trans.localPosition = pos;

    //    trans.localScale = new Vector3(1000, 1000, 1000);
    //    se.DoAnim(AnimType.Idle);
    //    curSpecialEffect = se;
    //}

    List<PopUpReceiveView.ItemData> dataList;
    public void RewardRecieve(List<PopUpReceiveView.ItemData> dataList)
    {
        this.dataList = dataList;
        ShowRewardAnim();
    }

    public void ShowRewardAnim()
    {
        Mask2.SetActive(true);
        LayoutBox.SetActive(false);
        LayoutShowReward.SetActive(true);
        ScrollView.ItemView.SetDataCount(dataList.Count);

        //BtnContine.SetActive(GetLessCount() >= 0);
        BtnClose.SetActive(true);

        BtnContine.transform.localScale = Vector3.zero;
        BtnClose.transform.localScale = Vector3.zero;

        float itemUseTime = (dataList.Count - 1) * interval + duration;

        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(itemUseTime);
        sequence.Append(BtnClose.transform.DOScale(new Vector3(1, 1, 1), 0.3f).SetEase(Ease.InOutBack));
        sequence.Append(BtnContine.transform.DOScale(new Vector3(1, 1, 1), 0.3f).SetEase(Ease.InOutBack));

        sequence.AppendCallback(() =>
        {
            Mask2.SetActive(false);
        });
    }

    float duration = 0.25f;
    float interval = 0.15f;
 
    void OnRenderItems(GameObject go, int index)
    {
        PropItem item = ItemBase.GetItem<PropItem>(go, DictItems);
        item.SetData(dataList[index]);

        item.Bg.transform.localScale = new Vector3(0, 0, 0);

        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(index * interval);
        sequence.Append(item.Bg.transform.DOScale(new Vector3(1, 1, 1), duration).SetEase(Ease.InOutBack));
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        //if (curSpecialEffect != null) {
        //    curSpecialEffect.Release();
        //    curSpecialEffect = null;
        //}
    }

    protected override void OnDestroy()
    {

    }
}
