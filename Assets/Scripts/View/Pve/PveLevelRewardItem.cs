

using Battle;
using UnityEngine;
using UnityEngine.UI;

public class PveLevelRewardItem : ItemBase
{
    U3dObj TxtCount;
    U3dObj ImgIcon;
    U3dObj ImgBg;

    int count;
    string icon;
    string iconAtlas;
    string bgIcon;
    string bgAtlas;

    string atlasSoldierIcon = "Atlas/SoldierIcon";
    string atlasCommon = "Atlas/Common";
    string atlasPve = "Atlas/Pve";

    protected override void OnInit(params object[] args)
    {
        TxtCount = Find("TxtCount");
        ImgIcon = Find("Icon");
        ImgBg = new U3dObj(transform);
    }

    public void SetData(int count, string itemCfgid) 
    {
        this.count = count;
        var itemTable = GameData.instance.TableJsonDict["ItemConf"];
        var itemData = itemTable[itemCfgid];
        if (itemCfgid == GameConfig.Gold)
        {
            icon = "ui_reward_gold_icon";
            iconAtlas = atlasPve;
            bgIcon = "ui_Treasure chest_bg_b";
            bgAtlas = atlasCommon;
        }
        else
        {
            icon = itemData["icon"];
            iconAtlas = atlasSoldierIcon;
            bgIcon = "ui_Treasure chest_bg_a";
            bgAtlas = atlasCommon;
        }

        //Debug.Log(this.count);
        //Debug.Log(iconName);
        Refresh();

    }

    protected override void OnRefresh()
    {
        TxtCount.Text.text = count.ToString();
        ImgIcon.SetSprite(iconAtlas, icon, true);
        ImgBg.SetSprite(bgAtlas, bgIcon, true);
    }

    protected override void OnRelease()
    {
        ResMgr.instance.ReleaseGameObject(gameObject);
    }
}
