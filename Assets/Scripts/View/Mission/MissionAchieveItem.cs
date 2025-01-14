using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionAchieveItem : ItemBase
{
    MissionView.AchieveData data;

    U3dObj TxtTitle;
    U3dObj MissionItemView;
    public Dictionary<GameObject, ItemBase> DictMissionAchieveSubItem = new Dictionary<GameObject, ItemBase>();

    protected override void OnInit(params object[] args)
    {
        _IsStep = true;
        TxtTitle = Find("TxtTitle");

        MissionItemView = Find("MissionItemView");

        MissionItemView.ItemView.Init(DictMissionAchieveSubItem);
        MissionItemView.ItemView.SetRenderHandler(OnRenderItem);

    }

    public void SetData(MissionView.AchieveData data) { 
        
        this.data = data;
        Refresh();
    }

    void OnRenderItem(GameObject go, int index) {
        MissionAchieveSubItem item = ItemBase.GetItem<MissionAchieveSubItem>(go, DictMissionAchieveSubItem);
        item.SetData(data.dataList[index]);
    }

    protected override void OnRefresh()
    {
        MissionItemView.ItemView.SetDataCount(data.dataList.Count);

        JSONNode subCfg = GameData.instance.TableJsonDict["TaskConf"][data.dataList[0].Key];
        TxtTitle.Text.text = subCfg["category"];

    }

    protected override void OnRelease()
    {
        MissionItemView.ItemView.Release();
    }
}
