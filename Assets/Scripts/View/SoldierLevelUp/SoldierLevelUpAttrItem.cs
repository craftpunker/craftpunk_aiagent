using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierLevelUpAttrItem : ItemBase
{
    U3dObj TextAttr1;
    U3dObj TextAttr2;
    U3dObj ImgUp;

    // Start is called before the first frame update
    protected override void OnInit(params object[] args)
    {
        TextAttr1 = Find("TextAttr1");
        TextAttr2 = Find("TextAttr2");
        ImgUp = Find("ImgUp");

    }

    public void SetAttr1(string attr) {

        TextAttr1.Text.text = attr;
    }

    public void SetAttr2(string attr)
    {

        TextAttr2.Text.text = attr;
    }

    public void ShowAttr1(bool value)
    {
        TextAttr1.SetActive(value);
    }

    public void ShowImgUp(bool value)
    {
        ImgUp.SetActive(value);
    }

    protected override void OnRelease()
    {

    }

    protected override void OnRefresh()
    {

    }
}
