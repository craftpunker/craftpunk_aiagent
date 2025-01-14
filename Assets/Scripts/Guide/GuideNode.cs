using Battle;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum GuideType
{
    talk = 1,
    button = 2,
    dragSoldier = 3,
    localBattle = 4,
    useBattleSkill = 5,
    WaitForBattleFinish = 6,
}

public class GuideNode
{
    public enum Stage
    {
        Checking,
        Guiding,
        Finish,
    }

    public bool IsTalk => cfg["type"] == (int)GuideType.talk;
    public bool IsButton => cfg["type"] == (int)GuideType.button;
    public bool IsDragSoldier => cfg["type"] == (int)GuideType.dragSoldier;
    public bool IsLocalBattle => cfg["type"] == (int)GuideType.localBattle;
    public bool IsUseBattleSkill => cfg["type"] == (int)GuideType.useBattleSkill;
    public bool IsWaitForBattleFinish => cfg["type"] == (int)GuideType.WaitForBattleFinish;

    public Stage stage;
    public string CfgId;
    public JSONNode cfg;
    int step;
    Dictionary<int, JSONNode> stepMap;

    public UiBase view;
    public string viewPath;
    public string btnName;
    public string methodName;

    public EntityBase Entity;

    public GuideNode(string cfgId)
    {
        CfgId = cfgId;
        stepMap = GuideMgr.guideCfgMap[CfgId];
        SetStep(1);
    }

    public int GridId;

    void SetStep(int step)
    {
        this.step = step;
        cfg = stepMap[step];
        stage = Stage.Checking;

        if (IsButton)
        {
            viewPath = cfg["strArg1"];
            btnName = cfg["strArg2"];
            methodName = cfg["strArg3"];
        }

        if (IsDragSoldier)
        {
            GridId = cfg["strArg2"];
        }
    }

    public void Update()
    {
        //DebugUtils.Log($"CheckIsCanGuidewwwwwwwwwwwwwwwww {cfg["cfgId"]}  {cfg["step"]}");
        //DebugUtils.Log("wwwwwwwwwwwwwwwww");
        if (stage == Stage.Guiding)
        {
            if (!CheckIsStopGuide())
            {
                if (IsTalk)
                {
                    CheckIsStopTalk();
                }
                else if (IsButton)
                {
                    CheckIsStopButton();
                }
                else if (IsDragSoldier)
                {
                    CheckIsStopDragSoldier();
                }
                else if (IsLocalBattle)
                {


                }
                else if (IsUseBattleSkill)
                {
                    if (Main.instance.Fsm.GetCurrState().GetType() != typeof(SceneBattleFsm))
                    {
                        UiMgr.Close<GuideView>();
                        stage = Stage.Checking;
                        SetStep(1);
                        view = null;
                        Target = null;
                        Entity = null;
                    }
                }

                else if (IsWaitForBattleFinish) {
                    //if (Main.instance.Fsm.GetCurrState().GetType() != typeof(SceneBattleFsm)) { 
                    //    //ResetStep();
                    //}
                }
            }
        }

        //DebugUtils.Log($"CheckIsCanGuidewwwwwwwwwwwwwwwww {cfg["cfgId"]}  {cfg["step"]}");
        if (stage == Stage.Checking)
        {
            if (CheckBanGuideViewOpen())
            {
                return;
            }

            GuideView guideView = UiMgr.GetView<GuideView>();
            if (guideView != null && guideView.uiStage == UiStage.show)
            {
                return;
            }

            if (IsTalk)
            {
                CheckIsCanGuideTalk();
            }
            else if (IsButton)
            {
                CheckIsCanGuideButton();
            }
            else if (IsDragSoldier)
            {
                CheckIsCanDragSoldier();
            }
            else if (IsLocalBattle)
            {
                EventDispatcher<CmdEventData>.instance.TriggerEvent(EventName.Scene_EnterFirstBattle);
                NextStep();
            }
            else if (IsUseBattleSkill)
            {
                if (Main.instance.Fsm.GetCurrState().GetType() != typeof(SceneBattleFsm))
                {
                    return;
                }

                Fix64 time = (Fix64)(int)cfg["intArg1"] / 1000;
                if (GameData.instance._CumTime <= time) {
                    return;
                }

                if (!CheckViewReady("BattleView"))
                {
                    return;
                }

                view = UiMgr.GetView<BattleView>();
                Target = view.GetGuideTrans(this);

                if (Target == null)
                {
                    return;
                }

                foreach (var item in BattleMgr.instance.SoldierList)
                {
                    //Debug.Log(item.CfgId);
                    if (item.PlayerGroup == PlayerGroup.Enemy)
                    {
                        if (item.CfgId.ToString() == cfg["strArg2"])
                        {
                            stage = Stage.Guiding;
                            Entity = item;
                            BattleMgr.instance.Pause = true;
                            UiMgr.Open<GuideView>(null, this);
                            break;
                        }
                    }
                }
            }
            else if (IsWaitForBattleFinish)
            {
                //if (Main.instance.Fsm.GetCurrState().GetType() != typeof(SceneBattleFsm))
                //{
                //    stage = Stage.Guiding;
                //}
            }
        }
    }

    public bool CheckIsStopGuide()
    {
        if (CheckBanGuideViewOpen())
        {
            stage = Stage.Checking;
            SetStep(1);
            UiMgr.Close<GuideView>();
            return true;
        }
        return false;
    }

    // talk

    public void ResetStep()
    {
        stage = Stage.Checking;
        SetStep(1);
        UiMgr.Close<GuideView>();
    }

    public void CheckIsStopTalk()
    {
        bool isInBattle = Main.instance.Fsm.GetCurrState().GetType() == typeof(SceneBattleFsm);
        bool isBattleTalk = cfg["intArg1"] > 0;

        if ((isBattleTalk && !isInBattle) || (!isBattleTalk && isInBattle))
        {
            ResetStep();
            return;
        }

        //if ((isBattleTalk && !isInBattle) || (!isBattleTalk && isInBattle))
        //{
        //    stage = Stage.Checking;
        //    UiMgr.Close<GuideView>();
        //    //ResetStep();
        //    return;
        //}

        string viewName = cfg["strArg2"];
        if (!string.IsNullOrEmpty(viewName))
        {
            if (!CheckViewReady(viewName))
            {
                ResetStep();
                return;
            }
        }
    }

    public void CheckIsCanGuideTalk()
    {
        //DebugUtils.Log($"dddddddddddddddddd  {cfg["cfgId"]}  {cfg["step"]} {cfg["intArg1"]}  {cfg["intArg1"] == 1}");

        bool isInBattle = Main.instance.Fsm.GetCurrState().GetType() == typeof(SceneBattleFsm);
        bool isBattleTalk = cfg["intArg1"] > 0;

        if ((isInBattle && isBattleTalk) || (!isInBattle && !isBattleTalk))
        {
            if (isBattleTalk)
            {
                Fix64 time = (Fix64)(int)cfg["intArg1"] / 1000;
                if (GameData.instance._CumTime <= time)
                {
                    return;
                }
                else {
                    BattleMgr.instance.Pause = true;

                }
            }

            string viewName = cfg["strArg2"];
            if (!string.IsNullOrEmpty(viewName))
            {
                if (!CheckViewReady(viewName))
                {
                    return;
                }
            }

            UiMgr.Open<GuideView>(null, this);
            stage = Stage.Guiding;
        }
    }

    public bool CheckViewReady(string viewName)
    {
        UiBase view = UiMgr.GetView(viewName);
        return view != null && view.uiStage == UiStage.show && !view.IsLoading;

    }

    // button
    public void CheckIsStopButton()
    {
        if (view != null)
        {
            if (view.uiStage != UiStage.show || view.IsLoading)
            {
                //DebugUtils.Log($"qqqqqqqqqqqqqqqqqqqqqqq {view.uiStage != UiStage.show}  {view.IsLoading}");
                view = null;
                stage = Stage.Checking;
                SetStep(1);
                UiMgr.Close<GuideView>();

                return;
            }
        }
    }

    public RectTransform Target;
    public void CheckIsCanGuideButton()
    {
        view = UiMgr.GetView(viewPath);

        if (view != null && view.uiStage == UiStage.show && !view.IsLoading && view.transform != null)
        {
            Target = view.GetGuideTrans(this);

            if (Target != null)
            {
                stage = Stage.Guiding;
                UiMgr.Open<GuideView>(null, this);
            }
        }
    }

    // dragSoldier
    void CheckIsStopDragSoldier()
    {
        if (view != null)
        {
            if (view.uiStage != UiStage.show || view.IsLoading)
            {
                //DebugUtils.Log($"qqqqqqqqqqqqqqqqqqqqqqq {view.uiStage != UiStage.show}  {view.IsLoading}");
                view = null;
                stage = Stage.Checking;
                SetStep(1);
                UiMgr.Close<GuideView>();
                SetAllTroopsColliderEnable(true);
                return;
            }
        }
    }

    void CheckIsCanDragSoldier()
    {

        //DebugUtils.Log("CheckIsCanDragSoldieraaaaaaaaaaaaaaaaaaaa");

        view = UiMgr.GetView<SoldierLevelUpView>();
        //this.view = view;

        if (view != null && view.uiStage == UiStage.show && !view.IsLoading)
        {

            //DebugUtils.Log("pppppppppppppppppppppppp");

            Target = view.GetGuideTrans(this);
            if (Target != null)
            {
                //DebugUtils.Log("pppppppppppppppppppppppp");
                stage = Stage.Guiding;
                UiMgr.Open<GuideView>(null, this);
                SetAllTroopsColliderEnable(false);
            }
        }
    }

    void SetAllTroopsColliderEnable(bool isEnable)
    {
        //DebugUtils.Log($"ddddddddddd   {isEnable}");

        foreach (var item in GameData.instance.TroopsMonos.Values)
        {
            DebugUtils.Log(item.transform.name);
            item.transform.GetComponent<BoxCollider>().enabled = isEnable;
        }
    }

    // =======================================
    static List<string> banGuideView = new List<string>() { UiMgr.GetUiAssetPath<LoginView>(), UiMgr.GetUiAssetPath<LoadingView>() };
    static bool CheckBanGuideViewOpen()
    {

        foreach (var item in banGuideView)
        {
            UiBase view = UiMgr.GetView(item);
            if (view != null && view.uiStage == UiStage.show)
            {
                return true;
            }
        }

        return false;
    }

    public void NextStep()
    {
        stage = Stage.Finish;
        Entity = null;
        Target = null;
        view = null;

        if (IsTalk) {
            if (cfg["intArg1"] > 0) {
                BattleMgr.instance.Pause = false;
            }
        }
        else if(IsDragSoldier)
        {
            SetAllTroopsColliderEnable(true);
        }
        else if (IsUseBattleSkill)
        {
            BattleMgr.instance.Pause = false;
        }

        if (cfg["next"].Count > 0)
        {
            foreach (var item in cfg["next"][0].Values)
            {
                GuideMgr.AddGuide(item.ToString());
            }
            GuideMgr.SaveGuideData();
        }

        if (stepMap.ContainsKey(step + 1))
        {
            SetStep(step + 1);
        }
        else
        {
            GuideMgr.RemoveGuide(this);
            GuideMgr.SaveGuideData();
            //if (cfg["next"].Count <= 0) {
            //    GuideMgr.SaveGuideData();
            //}
        }
    }
}
