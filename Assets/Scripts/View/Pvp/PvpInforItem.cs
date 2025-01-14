using Battle;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class PvpInforItem : ItemBase
{
    JSONNode data;
    U3dObj ImgRank;
    U3dObj TxtPoint;
    U3dObj ImgBg;
    U3dObj Content;

    public PvpInforView PvpInforView;

    string pvpIcon = "PvpAtlas";
    protected override void OnInit(params object[] args)
    {
        base.OnInit(args);
        ImgRank = Find("ImgRank");
        TxtPoint = Find("TxtPoint");
        ImgBg = Find("ImgBg");
        Content = Find("Content");
    }

    public void SetData(JSONNode data, int curStage = -1 )
    {
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            var child = Content.transform.GetChild(i);
            child.SetActiveEx(false);
        }

        this.data = data;
        var itemTable = GameData.instance.TableJsonDict["ItemConf"];
        var seasonaRewardItem = data["seasonaRewardItem"];
        ImgRank.SetSprite(pvpIcon, data["icon"]);
        TxtPoint.Text.text = data["needScore"][0];
        if (data["cfgId"] == curStage)
        {
            ImgBg.SetSprite(pvpIcon, "Ui_PVP_infor_display");
        }
        else
        {
            ImgBg.SetSprite(pvpIcon, "Ui_PVP_infor_bg");
        }

        for (int i = 0; i < seasonaRewardItem.Count; i++)
        {
            var item = seasonaRewardItem[i];
            var itemId = item[0];
            var child = Content.transform.GetChild(i);
            var itemData = itemTable[itemId.ToString()];
            ResMgr.instance.LoadSpriteAsync(itemData["iconAtlas"], itemData["icon"], (icon) =>
            {
                child.Find("Icon").GetComponent<Image>().sprite = icon;
                child.Find("Count").GetComponent<Text>().text = item[1];
                child.SetActiveEx(true);
            });
            //child.SetSprite("Atlas/Pvp", "Ui_PVP_fail");
        }

        Refresh();
    }
}
