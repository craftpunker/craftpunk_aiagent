
using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MissionView : UiBase
{
    Dictionary<RedPointBase, U3dObj> dictRedPoint;

    int showingIndex = 0;

    U3dObj BtnClose;
    U3dObj ImgTime;
    U3dObj TxtTime;
    U3dObj LayoutOptions;
    List<U3dObj> BtnOptionList;

    List<U3dObj> LayoutFunctionList;

    U3dObj Bg1;
    U3dObj Bg2;

    //LayoutDaily
    U3dObj LayoutDaily;

    U3dObj LayoutDailyProgress;
    U3dObj DailySlider;
    List<MissionActBoxItem> DailyRewardBoxList;

    U3dObj DailyScrollView;
    Dictionary<GameObject, ItemBase> MissionDailyItems = new Dictionary<GameObject, ItemBase>();


    // LayoutWeekly
    U3dObj LayoutWeekly;
    U3dObj LayoutWeeklyProgress;
    U3dObj WeeklySlider;
    List<MissionActBoxItem> WeeklyRewardBoxList;

    U3dObj WeeklyScrollView;
    Dictionary<GameObject, ItemBase> MissionWeeklyItems = new Dictionary<GameObject, ItemBase>();

    //LayoutAchieve
    U3dObj LayoutAchieve;

    U3dObj AchieveScrollView;
    Dictionary<GameObject, ItemBase> AchieveItems = new Dictionary<GameObject, ItemBase>();

    //============================data
    JSONNode dailyData;
    List<string> dailyDataKeyList = new List<string>();


    JSONNode weeklyData;
    List<string> weeklyDataKeyList = new List<string>();


    protected override void OnInit()
    {
        U3dObj bg = Find("Bg");
        Bg1 = bg.Find("Bg1");
        Bg2 = bg.Find("Bg2");

        BtnClose = bg.Find("BtnClose");
        SetOnClick(BtnClose, OnClickClose);

        //ImgTime = bg.Find("ImgTime");
        TxtTime = bg.Find("TxtTime");

        LayoutOptions = bg.Find("LayoutOptions");



        BtnOptionList = new List<U3dObj>();

        for (int i = 1; i < 4; i++)
        {
            U3dObj btn = LayoutOptions.Find("BtnOption" + i);

            BtnOptionList.Add(btn);

            int index = i;
            SetOnClick(btn, () =>
            {
                OnClickOption(index);
            });
        }

        //red===============================
        dictRedPoint = new Dictionary<RedPointBase, U3dObj>();
        dictRedPoint.Add(RedPointMgr.GetRedPoint(typeof(DailyMissionRedPoint)), BtnOptionList[0]);
        dictRedPoint.Add(RedPointMgr.GetRedPoint(typeof(WeeklyMissionRedPoint)), BtnOptionList[1]);
        dictRedPoint.Add(RedPointMgr.GetRedPoint(typeof(AchieveMissionRedPoint)), BtnOptionList[2]);
        //=====================

        //LayoutDaily
        LayoutDaily = bg.Find("LayoutDaily");
        LayoutDailyProgress = LayoutDaily.Find("LayoutDailyProgress");
        DailySlider = LayoutDailyProgress.Find("Slider");

        DailyRewardBoxList = new List<MissionActBoxItem>();
        for (int i = 1; i < 6; i++)
        {

            U3dObj box = LayoutDailyProgress.Find("Box" + i);
            MissionActBoxItem item = ItemBase.GetItem<MissionActBoxItem>(box.gameObject);
            DailyRewardBoxList.Add(item);
        }

        DailyScrollView = LayoutDaily.Find("DailyScrollView");
        DailyScrollView.LoopScrollView.Init(MissionDailyItems);
        DailyScrollView.LoopScrollView.SetRenderHandler(OnRenderDailyItem);

        //LayoutWeekly
        LayoutWeekly = bg.Find("LayoutWeekly");
        LayoutWeeklyProgress = LayoutWeekly.Find("LayoutWeeklyProgress");
        WeeklySlider = LayoutWeeklyProgress.Find("Slider");


        WeeklyRewardBoxList = new List<MissionActBoxItem>();
        for (int i = 1; i < 6; i++)
        {
            U3dObj box = LayoutWeeklyProgress.Find("Box" + i);
            MissionActBoxItem item = ItemBase.GetItem<MissionActBoxItem>(box.gameObject);
            WeeklyRewardBoxList.Add(item);
        }

        WeeklyScrollView = LayoutWeekly.Find("WeeklyScrollView");
        WeeklyScrollView.LoopScrollView.Init(MissionWeeklyItems);
        WeeklyScrollView.LoopScrollView.SetRenderHandler(OnRenderWeeklyItem);

        //LayoutAchieve
        LayoutAchieve = bg.Find("LayoutAchieve");

        AchieveScrollView = LayoutAchieve.Find("AchieveScrollView");
        AchieveScrollView.LoopScrollView.Init(AchieveItems);
        AchieveScrollView.LoopScrollView.SetRenderHandler(OnRenderAchieveItem);
        AchieveScrollView.LoopScrollView.SetRenderSizeHandler(OnRenderAchieveItemSize);

        // =================================
        LayoutFunctionList = new List<U3dObj> { LayoutDaily, LayoutWeekly, LayoutAchieve };
    }

    string[] dailyEvents = new string[] { EventName.d_task, EventName.d_atc, EventName.da_reward };
    string[] weeklyEvents = new string[] { EventName.w_task, EventName.w_atc, EventName.wa_reward };
    string[] AchieveEvents = new string[] { "a_task" };
    protected override void OnShow()
    {
        foreach (var item in dailyEvents)
        {
            EventDispatcher<CmdEventData>.instance.AddEvent(item, OnDailyEvent);
        }

        foreach (var item in weeklyEvents)
        {
            EventDispatcher<CmdEventData>.instance.AddEvent(item, OnWeeklyEvent);
        }

        foreach (var item in AchieveEvents)
        {
            EventDispatcher<CmdEventData>.instance.AddEvent(item, OnAchieveEvent);
        }

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.finish_daily_task, OnFinishDailyTask);
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.finish_week_task, OnFinishWeekTask);

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.daily_act_reward, OnDailyActReward);
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.week_act_reward, OnWeeklyActReward);
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.finish_achieve_task, OnAchieveTask);

        EventDispatcher<RedPointBase>.instance.AddEvent(EventName.Red_Point, OnRedPointChange);

        RefreshRedPoint();
        OnClickOption(1);
    }

    void OnDailyEvent(string evtName, CmdEventData[] evt)
    {
        if (showingIndex == 1)
        {
            OnClickOption(1);
        }
    }

    void OnFinishDailyTask(string evtName, CmdEventData[] evt)
    {
        if (showingIndex == 1)
        {
            JSONNode data = evt[0].JsonNode.Value["data"];
            ShowTaskRewardAnim(data, MissionDailyItems);
        }
    }

    void OnFinishWeekTask(string evtName, CmdEventData[] evt)
    {
        if (showingIndex == 2)
        {
            JSONNode data = evt[0].JsonNode.Value["data"];
            ShowTaskRewardAnim(data, MissionWeeklyItems);
        }
    }

    void ShowTaskRewardAnim(JSONNode data, Dictionary<GameObject, ItemBase> dictItem)
    {

        foreach (var item in dictItem.Values)
        {
            MissionDailyItem missionDailyItem = (MissionDailyItem)item;

            if (data["cfgId"] == missionDailyItem.taskCfg["cfgId"].ToString())
            {
                ResAnim.instance.ShowUIAnim(data["rewardItems"], missionDailyItem.BtnReceive.transform.position);
            }
        }
    }

    void OnDailyActReward(string evtName, CmdEventData[] evt)
    {
        if (showingIndex == 1)
        {
            JSONNode data = evt[0].JsonNode.Value["data"];
            ShowActRewardAnim(data, DailyRewardBoxList);
        }
    }

    void OnWeeklyActReward(string evtName, CmdEventData[] evt)
    {
        if (showingIndex == 2)
        {
            JSONNode data = evt[0].JsonNode.Value["data"];
            ShowActRewardAnim(data, WeeklyRewardBoxList);
        }
    }

    void OnAchieveTask(string evtName, CmdEventData[] evt)
    {
        if (showingIndex == 3)
        {
            JSONNode data = evt[0].JsonNode.Value["data"];

            foreach (var item in AchieveItems.Values)
            {
                MissionAchieveItem missionAchieveItem = (MissionAchieveItem)item;

                foreach (var subItem in missionAchieveItem.DictMissionAchieveSubItem.Values)
                {
                    MissionAchieveSubItem missionAchieveSubItem = (MissionAchieveSubItem)subItem;
                    if (missionAchieveSubItem.cfg["cfgId"].ToString() == data["cfgId"])
                    {
                        Vector3 pos = missionAchieveSubItem.GetAnimPos();

                        foreach (var rewards in data["rewardItems"])
                        {
                            ResAnim.instance.ShowUIAnim(Convert.ToInt32(rewards.Key), pos, rewards.Value);
                        }
                    }
                }
            }
        }
    }

    void ShowActRewardAnim(JSONNode data, List<MissionActBoxItem> itemList)
    {
        foreach (var item in itemList)
        {
            if (data["cfgId"] == item.key)
            {
                foreach (var reawrd in data["rewardItems"])
                {
                    ResAnim.instance.ShowUIAnim(Convert.ToInt32(reawrd.Key), item.ImgBox.transform.position, reawrd.Value);
                }
            }
        }
    }

    void OnWeeklyEvent(string evtName, CmdEventData[] evt)
    {
        if (showingIndex == 2)
        {
            OnClickOption(2);
        }
    }
    // red========================
    void OnRedPointChange(string evtName, RedPointBase[] evt)
    {
        RedPointBase redPoint = evt[0];

        if (dictRedPoint.TryGetValue(redPoint, out U3dObj obj))
        {
            obj.SetRedPoint(redPoint.IsRed);
        }
    }

    void RefreshRedPoint()
    {
        //BtnOptionList
        foreach (var item in dictRedPoint)
        {
            item.Value.SetRedPoint(item.Key.IsRed);
        }
    }
    //=============================

    void OnAchieveEvent(string evtName, CmdEventData[] evt)
    {
        if (showingIndex == 3)
        {
            OnClickOption(3);
        }
    }


    void OnClickClose()
    {
        this.Close();
    }

    void OnClickOption(int index)
    {
        showingIndex = index;

        for (int i = 0; i < 3; i++)
        {
            if (i == index - 1)
            {
                BtnOptionList[i].SetSprite("MissionAtlas", "Ui_task_anniu_b");
                LayoutFunctionList[i].SetActive(true);
            }
            else
            {
                BtnOptionList[i].SetSprite("MissionAtlas", "Ui_task_anniu_a");
                LayoutFunctionList[i].SetActive(false);
            }
        }

        if (index == 1)
        {
            Bg1.SetActive(true);
            Bg2.SetActive(false);
            RefreshDaily();

        }
        else if (index == 2)
        {
            Bg1.SetActive(true);
            Bg2.SetActive(false);
            RefreshWeekly();
        }
        else if (index == 3)
        {
            Bg1.SetActive(false);
            Bg2.SetActive(true);
            RefreshAchieve();
        }
    }

    // daily=========================================
    void RefreshDaily()
    {
        TxtTime.SetActive(true);
        JSONNode act = GameData.instance.DailyData["d_atc"];

        float progress = 0;
        if (act != null)
        {
            progress = act;
        }
        DailySlider.Slider.value = progress / 100;

        for (int i = 0; i < 5; i++)
        {
            MissionActBoxItem item = DailyRewardBoxList[i];
            string key = (i + 1).ToString();
            JSONNode cfg = GameData.instance.TableJsonDict["TaskRewardConf"][key];
            item.SetData(key, cfg);
        }

        dailyData = GameData.instance.DailyData["d_task"];
        dailyDataKeyList.Clear();
        foreach (var item in dailyData.Keys)
        {
            dailyDataKeyList.Add(item.ToString());
        }
        dailyDataKeyList.Sort(SortMissionFunc);
        DailyScrollView.LoopScrollView.SetDataCount(dailyData.Count);
    }

    void OnRenderDailyItem(GameObject obj, int index)
    {
        MissionDailyItem item = ItemBase.GetItem<MissionDailyItem>(obj, MissionDailyItems);

        string key = dailyDataKeyList[index];
        item.SetData(key, dailyData[key], MissionDailyItem.MissionItemType.Daily);
    }

    int SortMissionFunc(string a, string b) {
        JSONNode data;
        if (showingIndex == 1)
        {
            data = dailyData;
        }
        else {
            data = weeklyData;
        }

        JSONNode dataA = data[a];
        JSONNode dataB = data[b];

        if (dataA["draw"] == dataB["draw"])
        {
            if (dataA["draw"] == 0)
            {
                bool isCanFetchA = dataA["cur"] >= dataA["need"];
                bool isCanFetchB = dataB["cur"] >= dataB["need"];
                if (isCanFetchA != isCanFetchB)
                {
                    return isCanFetchA ? -1 : 1;
                }
            }
            return Convert.ToInt32(a) < Convert.ToInt32(b) ? -1 : 1;
        }
        else
        {
            return dataA["draw"] == 0 ? -1 : 1;
        }

    }

    //weekly================================================
    void RefreshWeekly()
    {
        TxtTime.SetActive(true);
        JSONNode act = GameData.instance.WeeklyData["w_atc"];
        float progress = 0;
        if (act != null)
        {
            progress = act;
        }
        WeeklySlider.Slider.value = progress / 100;


        for (int i = 0; i < 5; i++)
        {
            MissionActBoxItem item = WeeklyRewardBoxList[i];
            string key = (i + 1 + 5).ToString();

            JSONNode cfg = GameData.instance.TableJsonDict["TaskRewardConf"][key];
            item.SetData(key, cfg);
        }

        weeklyData = GameData.instance.WeeklyData["w_task"];

        weeklyDataKeyList.Clear();
        foreach (var item in weeklyData.Keys)
        {
            weeklyDataKeyList.Add(item.ToString());
        }

        weeklyDataKeyList.Sort(SortMissionFunc);

        WeeklyScrollView.LoopScrollView.SetDataCount(weeklyData.Count);
    }

    void OnRenderWeeklyItem(GameObject obj, int index)
    {
        MissionDailyItem item = ItemBase.GetItem<MissionDailyItem>(obj, MissionWeeklyItems);

        string key = weeklyDataKeyList[index];
        item.SetData(key, weeklyData[key], MissionDailyItem.MissionItemType.Weekly);
    }

    //Achieve
    public class AchieveData
    {
        public int type;
        public string category;
        public List<KeyValuePair<string, JSONNode>> dataList = new List<KeyValuePair<string, JSONNode>>();
    }

    public List<AchieveData> AchieveDataList = new List<AchieveData>();


    void RefreshAchieve()
    {
        TxtTime.SetActive(false);
        AchieveDataList.Clear();

        JSONNode taskData = GameData.instance.PermanentData["a_task"];
        JSONNode taskCfg = GameData.instance.TableJsonDict["TaskConf"];

        foreach (var item in taskData)
        {
            int type = Convert.ToInt16(item.Key) / 100;
            JSONNode cfg = taskCfg[item.Key];
            string category = cfg["category"];

            //AchieveData achieveData = AchieveDataList.Find(t => t.type == type);

            AchieveData achieveData = AchieveDataList.Find(t => t.category == category);
            if (achieveData == null)
            {
                achieveData = new AchieveData();
                AchieveDataList.Add(achieveData);
                achieveData.category = category;
                achieveData.type = type;
            }

            if (type < achieveData.type)
            {
                achieveData.type = type;
            }
            achieveData.dataList.Add(item);
        }

        AchieveDataList.Sort((a, b) =>
        {
            return a.type < b.type ? -1 : 1;
        });

        AchieveScrollView.LoopScrollView.SetDataCount(AchieveDataList.Count);

    }
    void OnRenderAchieveItem(GameObject go, int index)
    {
        MissionAchieveItem item = ItemBase.GetItem<MissionAchieveItem>(go, AchieveItems);
        item.SetData(AchieveDataList[index]);
    }

    Vector2 OnRenderAchieveItemSize(int index)
    {
        AchieveData data = AchieveDataList[index];
        int count = data.dataList.Count;

        float subItemLen = 94;
        float spancing = 7;

        float len = (subItemLen + spancing) * count - spancing + 119 + 40;

        return new Vector2(1209, len);
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0)) {

            List<MissionActBoxItem> list = null;

            if (showingIndex == 1)
            {
                list = DailyRewardBoxList;
            }
            else if (showingIndex == 2) {
                list = WeeklyRewardBoxList;
            }

            if (list != null) {
                foreach (var item in list)
                {
                    item.SetRewardShow(false);
                }
            }
        }

        float lessTime = 0;
        if (showingIndex == 1)
        {
            lessTime = ModuleData.DayEnd - Time.time;
        }
        else if (showingIndex == 2)
        {
            lessTime = ModuleData.WeekEnd - Time.time;
        }

        TimeUtils.TimeHMS t = TimeUtils.DHMS(lessTime);

        if (t.Day > 0)
        {
            TxtTime.Text.text = $"{t.Day}Day {t.Hour}:{t.Minute}:{t.Second}";
        }
        else
        {
            TxtTime.Text.text = $"{t.Hour}:{t.Minute}:{t.Second}";
        }
    }

    protected override void OnHide()
    {
        foreach (var item in dailyEvents)
        {
            EventDispatcher<CmdEventData>.instance.RemoveEvent(item, OnDailyEvent);
        }

        foreach (var item in weeklyEvents)
        {
            EventDispatcher<CmdEventData>.instance.RemoveEvent(item, OnWeeklyEvent);
        }

        foreach (var item in AchieveEvents)
        {
            EventDispatcher<CmdEventData>.instance.RemoveEvent(item, OnAchieveEvent);
        }

        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.finish_daily_task, OnFinishDailyTask);
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.finish_week_task, OnFinishWeekTask);
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.finish_achieve_task, OnAchieveTask);

        EventDispatcher<RedPointBase>.instance.RemoveEvent(EventName.Red_Point, OnRedPointChange);
    }

    protected override void OnDestroy()
    {
        DailyScrollView.LoopScrollView.Release();
        WeeklyScrollView.LoopScrollView.Release();
        AchieveScrollView.LoopScrollView.Release();

        foreach (var item in DailyRewardBoxList)
        {

            item.Release();
        }

        foreach (var item in WeeklyRewardBoxList)
        {
            item.Release();
        }
    }
}
