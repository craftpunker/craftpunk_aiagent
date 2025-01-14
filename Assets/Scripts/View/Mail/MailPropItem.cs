
using Battle;
using SimpleJSON;
using UnityEngine;

public class MailPropItem : ItemBase
{
    private JSONNode Data;
    U3dObj ImgIcon;
    U3dObj TxtCount;

    protected override void OnInit(params object[] args)
    {
        base.OnInit(args);
        ImgIcon = Find("ImgIcon");
        TxtCount = Find("TxtCount");
    }

    public void SetData(JSONNode data)
    {
        Data = data;
        var itemTable = GameData.instance.TableJsonDict["ItemConf"];
        var itemTableData = itemTable[data[0].ToString()];
        TxtCount.Text.text = data[1];
        ImgIcon.SetSprite(itemTableData["iconAtlas"], itemTableData["icon"]);
        item.SetSprite(itemTableData["bgIconAtlas"], itemTableData["bgIcon"]);
    }
}