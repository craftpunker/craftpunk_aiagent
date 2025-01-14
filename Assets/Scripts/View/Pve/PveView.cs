using Battle;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PveView : UiBase
{
    U3dObj LayoutBattle;
    U3dObj LayoutInfo;
    U3dObj InfoSlider;
    U3dObj ImgLevel;
    U3dObj BtnReward;
    public U3dObj BtnBattle;
    
    U3dObj LayoutReward;
    U3dObj btnClose;
    U3dObj LevelScrollView;
    Dictionary<GameObject, ItemBase> _ItemList = new Dictionary<GameObject, ItemBase>();
    U3dObj pveProgress;

    JSONNode pveCfg;
    JSONNode curCfg;


    protected override void OnInit()
    {
        //Find
        LayoutBattle = Find("LayoutBattle"); //new U3dObj(Panel.Find(""));

        LayoutInfo = LayoutBattle.Find("LayoutInfo");

        InfoSlider = LayoutInfo.Find("InfoSlider");
        InfoSlider.Text.text = "All Clean";
        ImgLevel = LayoutInfo.Find("ImgLevel");
        ImgLevel.Text.text = "";
        BtnReward = LayoutInfo.Find("BtnReward");
        SetOnClick(BtnReward.gameObject, OnBtnReward);

        BtnBattle = LayoutBattle.Find("BtnBattle");
        SetOnClick(BtnBattle.gameObject, OnBtnBattle);


        LayoutReward = Find("LayoutReward");  //new U3dObj(Panel.Find("LayoutReward"));

        LevelScrollView = LayoutReward.Find("LevelScrollView");
        LevelScrollView.LoopScrollView.Init();
        LevelScrollView.LoopScrollView.SetRenderHandler(OnRenderItem);

        btnClose = LayoutReward.Find("BtnClose"); // new U3dObj(Panel.Find("Bg/BtnClose"));
        SetOnClick(btnClose.gameObject, OnClickClose);

        pveProgress = Find("LayoutBattle/PveProgress");
        var pass5 = pveProgress.transform.GetChild(4);
        var pass10 = pveProgress.transform.GetChild(9);
        pveCfg = GameData.instance.TableJsonDict["PveConf"];

        SetOnClick(pass5.gameObject, () =>
        {
            var passPve = (GameData.instance.PermanentData["pass_pve"] - 10000) % 10;
            var passPve5 = GameData.instance.PermanentData["pass_pve"] - passPve + 5;
            var name = pveCfg[passPve5.ToString()]["name"];

            var passObj = pass5.Find("ImgBox").gameObject;
            if (passObj.activeSelf)
            {
                passObj.SetActive(false);
            }
            else
            {
                var imgBoxTxt = pass5.Find("ImgBox/TxtCount").GetComponent<Text>();
                imgBoxTxt.text = $"Win to {name.Value} to get a reard";
                passObj.SetActive(true);
            }
        });

        SetOnClick(pass10.gameObject, () =>
        {
            var passPve = (GameData.instance.PermanentData["pass_pve"] - 10000) % 10;
            var passPve10 = GameData.instance.PermanentData["pass_pve"] - passPve + 10;
            var name = pveCfg[passPve10.ToString()]["name"];

            var passObj = pass10.Find("ImgBox").gameObject;
            if (passObj.activeSelf)
            {
                passObj.SetActive(false);
            }
            else
            {
                var imgBoxTxt = pass10.Find("ImgBox/TxtCount").GetComponent<Text>();
                imgBoxTxt.text = $"Win to {name.Value} to get a reard";
                passObj.SetActive(true);
            }
        });

    }

    protected override void OnShow()
    {
        string pveCfgId = GameData.instance.PermanentData["pass_pve"];
        string nextId = pveCfg[pveCfgId]["nextId"];

        pveProgress.transform.GetChild(4).Find("ImgBox").SetActiveEx(false);
        pveProgress.transform.GetChild(9).Find("ImgBox").SetActiveEx(false);

        if (nextId != "0")
        {
            SetInfo(pveCfg[nextId]);
        }
        else
            UiMgr.ShowTips("You've beaten all the mission");

        string mapName = pveCfg[nextId]["mapName"];
        MapMgr.instance.ChangeMap(1, mapName);

        var passPve = (GameData.instance.PermanentData["pass_pve"] - 10000) % 10;
        ResetPassPve();
        for (int i = 0; i < passPve; i++)
        {
            var child = pveProgress.transform.GetChild(i);
            if (i == 4)
            {
                child.GetChild(0).GetChild(0).gameObject.SetActive(false);
                child.GetChild(0).GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                child.GetChild(0).gameObject.SetActive(false);
                child.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    private void ResetPassPve()
    {
        var childCount = pveProgress.transform.childCount;
        for (int i = 0; i < childCount - 1; i++)
        {
            var child = pveProgress.transform.GetChild(i);
            if (i == 4)
            {
                child.GetChild(0).GetChild(0).gameObject.SetActive(true);
                child.GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                child.GetChild(0).gameObject.SetActive(true);
                child.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        
    }

    protected override void OnDestroy()
    {
        LevelScrollView.LoopScrollView.Release();
    }

    //================================

    void OnClickClose() {
        LayoutBattle.SetActive(true);
        LayoutReward.SetActive(false);
        //UIMgr.instance.ClosePanel("Pve/PveView");
    }

    void OnBtnReward()
    {
        LayoutBattle.SetActive(false);
        OpenReward();
        
    }

    void OpenReward() {
        DebugUtils.Log("OpenReward");

        LayoutReward.SetActive(true);
        LevelScrollView.LoopScrollView.SetDataCount(pveCfg.Count);
    }

    void SetInfo(JSONNode info) {
        curCfg = info;
        ImgLevel.Text.text = info["levelNumber"];

        InfoSlider.Text.text = info["name"];
        GameData.instance.EnemyBattleJsonObj = BattleMgr.instance.GetDisPlayTroopsJsonObjByLayout(info["cfgId"]);
        EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, true);
    }

    private void CallBack()
    {

    }

    void OnRenderItem(GameObject obj, int index)
    {
        PveLevelItem item = ItemBase.GetItem<PveLevelItem>(obj, _ItemList, this);
        item.SetData(pveCfg[index]);
    }

    public void OnBtnBattle()
    {
        if (curCfg == null)
        {
            UiMgr.ShowTips("You've beaten all the mission");
            return;
        }
        JSONObject jsonObj = new JSONObject();
        jsonObj.Add("data", new JSONObject());
        jsonObj["data"].Add("cfgId", curCfg["cfgId"]);
        Cmd.instance.C2S_CHALLENGE_PVE(jsonObj);
        //LayoutReward.SetActive(true);
    }

    //public override void Show(GameObject panel)
    //{
    //    base.Show(panel);

        
    //}

    //public override void Update()
    //{
    //    base.Update();
    //}

    //public override void BindEvent()
    //{
    //    base.BindEvent();
    //}

    private void OnStartGame()
    {

    }

    //public override void Close()
    //{
    //    LevelScrollView.LoopScrollView.Release();
    //    base.Close();
    //}
}
