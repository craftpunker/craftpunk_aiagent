
using Battle;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillCardItem : ItemBase
{
    //U3dObj ImgBg;
    U3dObj ImgIcon;
    public U3dObj TxtCost;
    public U3dObj Img_Selected;
    public U3dObj Img_Mask;
    public int Cost;

    public CardData CardData;
    public Action<int> OnBeginDragSkillCard;
    public Action<int> OnEndDragSkillCard;

    protected override void OnInit(params object[] args)
    {
        //ImgBg = Find("Img_BG");
        ImgIcon = Find("Img_Icon");
        TxtCost = Find("Img_Cost/Txt_Cost");
        Img_Selected = Find("Img_Selected");
        Img_Mask = Find("Img_Mask");

        SetBeginDrag(item.gameObject, OnBeginDrag);
        //SetDrag(item.gameObject, OnDrag);
        SetEndDrag(item.gameObject, OnEndDrag);
    }

    public void SetData(CardData cardData)
    {
        CardData = cardData;
        string cfgid = CardData.CfgId.ToString();
        ImgIcon.SetSprite("CardIconAtlas", $"icon_{cfgid.Substring(0, Math.Min(cfgid.Length, 5))}");
        TxtCost.Text.text = cardData.Cost.ToString();
        Img_Selected.SetActive(false);
        Img_Mask.SetActive(true);
    }

    public void ShowMask(bool value)
    {
        Img_Mask.SetActive(!value);
    }

    void OnBeginDrag(PointerEventData data)
    {
        if (GuideMgr.guideNodeList.Count > 0)
        {
            foreach (var item in GuideMgr.guideNodeList)
            {
                if (item.IsUseBattleSkill && item.stage == GuideNode.Stage.Guiding && item.Target.transform != transform)
                {
                    return;
                }
            }
        }

        OnBeginDragSkillCard(CardData.CfgId);
        Img_Selected.SetActive(true);
    }

    //void OnDrag(PointerEventData data)
    //{
    //    Debug.Log(2222);
    //}

    void OnEndDrag(PointerEventData data)
    {
        //Debug.Log(GameData.instance._CumTime);
        //Debug.Log(3333);
        OnEndDragSkillCard(CardData.CfgId);
        Img_Selected.SetActive(false);
    }

    protected override void OnRefresh()
    {

    }
}