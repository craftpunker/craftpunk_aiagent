using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGoodsItem : ItemBase
{
    public string CfgId;

    //public U3dObj Bg;

    public U3dObj Bg;
    public U3dObj ImgBenefit;
    public U3dObj TxtBenefit;
    public U3dObj TxtLessCount;
    public U3dObj ImgGoods;
    public U3dObj TxtCount;
    public U3dObj TxtPrice;
    public U3dObj ImgPrice;


    protected override void OnInit(params object[] args)
    {
        //Bg = new U3dObj(transform);
        Bg = Find("Bg");

        ImgBenefit = Bg.Find("ImgBenefit");
        TxtBenefit = ImgBenefit.Find("TxtBenefit");
        TxtLessCount = Bg.Find("TxtLessCount");
        TxtLessCount.Text.text = string.Empty;
        ImgGoods = Bg.Find("ImgGoods");
        TxtCount = Bg.Find("TxtCount");
        TxtPrice = Bg.Find("TxtPrice");
        ImgPrice = TxtPrice.Find("ImgPrice");

        SetOnClick(Bg, OnClickItem);
    }

    public void SetPriceIcon(bool isShow) {

        Vector3 pos = TxtPrice.RectTransform.localPosition;

        if (isShow)
        {
            pos.x = 19;
            TxtPrice.RectTransform.localPosition = pos;
            ImgPrice.Image.transform.SetActiveEx(true);
        }
        else {
            pos.x = 0;
            TxtPrice.RectTransform.localPosition = pos;
            ImgPrice.Image.transform.SetActiveEx(false);
        }
    }

    public void SetPriceColorRed(bool isRed) {

        if (isRed && TxtPrice != null && TxtPrice.Text != null)
        {
            TxtPrice.Text.color = Color.red;
        }
        else {
            TxtPrice.Text.color = Color.white;
        }
    }

    protected override void OnRefresh()
    {

    }

    public virtual void OnClickItem() {
        Debug.Log("OnClickItem");
        //debug.Log("")
    }

    public virtual void OnResChange()
    {
        SetPriceColorRed(false);
    }

    protected override void OnRelease()
    {

    }
}
