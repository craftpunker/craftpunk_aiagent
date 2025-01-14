
using Battle;
using SimpleJSON;
using UnityEngine;

//
public class PropItem : ItemBase
{
    PopUpReceiveView.ItemData Data;
    public U3dObj Bg;
    U3dObj ImgIcon;
    U3dObj TxtCount;

    protected override void OnInit(params object[] args)
    {
        base.OnInit(args);
        Bg = Find("Bg");
        ImgIcon = Bg.Find("ImgIcon");
        TxtCount = Bg.Find("TxtCount");
    }

    public void SetData(PopUpReceiveView.ItemData data)
    {
        Data = data;
        var itemTable = GameData.instance.TableJsonDict["ItemConf"];
        var itemTableData = itemTable[data.ItemCfgId];
        TxtCount.Text.text = data.count.ToString();
        ImgIcon.SetSprite(itemTableData["iconAtlas"], itemTableData["icon"]);
        item.SetSprite(itemTableData["bgIconAtlas"], itemTableData["bgIcon"]);
    }
}