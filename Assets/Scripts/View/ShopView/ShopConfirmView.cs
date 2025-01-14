using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopConfirmView : UiBase
{

    U3dObj TxtTitle;
    U3dObj BtnClose;
    U3dObj ImgIcon;
    public U3dObj BtnConfirm;

    U3dObj ImgCost;

    // Start is called before the first frame update
    protected override void OnInit()
    {
        TxtTitle = Find("Bg/TxtTitle");
        BtnClose = Find("Bg/BtnClose");
        SetOnClick(BtnClose, () =>
        {
            Close();
        });

        ImgIcon = Find("Bg/ImgIcon");
        BtnConfirm = Find("Bg/BtnConfirm");
        ImgCost = BtnConfirm.Find("Text/ImgCost");

        SetOnClick(BtnConfirm, OnBtnConfirm);
    }

    public void OnBtnConfirm() {
        if (!CheckCostEnought()) {
            return;
        }

        data.callback?.Invoke();
        Close();
    }

    public class ShopConfirmViewData {
        public Action callback;

        public string Title;
        //public JSONNode itemCfg;

        public string IconAtlas;
        public string Icon;

        public string CostAtlas;
        public string CostIcon;

        public string CostType;
        public string Cost;
    }

    ShopConfirmViewData data;
    //args = [itemCfg, costItemCfg]
    protected override void OnShow()
    {
        data = (ShopConfirmViewData)Args[0];

        TxtTitle.Text.text = data.Title;

        ImgIcon.SetSprite(data.IconAtlas, data.Icon, true, () => {
            //ImgIcon.SetSpriteRectBaseW(256);
        });

        BtnConfirm.Text.text = data.Cost;

        Vector2 textPos = new Vector2(0, 0);

        if (string.IsNullOrEmpty(data.CostAtlas))
        {
            ImgCost.SetActive(false);
            BtnConfirm.Text.transform.GetComponent<RectTransform>().anchoredPosition = textPos;
        }
        else {
            ImgCost.SetActive(true);
            ImgCost.SetSprite(data.CostAtlas, data.CostIcon);

            textPos.x = 12.9f;
            BtnConfirm.Text.transform.GetComponent<RectTransform>().anchoredPosition = textPos;
        }

        ImgIcon.SetSprite(data.IconAtlas, data.Icon);

        BtnConfirm.SetImgGray(!CheckCostEnought());
    }

    bool CheckCostEnought() {
        if (string.IsNullOrEmpty(data.CostType))
        {
            return true;
        }

        int count = GameData.instance.PermanentData["bag_info"][data.CostType.ToString()]["num"];

        return count >= Convert.ToInt32(data.Cost);
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {

    }

    protected override void OnDestroy()
    {

    }
}
