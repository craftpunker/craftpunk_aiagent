using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiMgr : MonoBehaviour
{
    public static UiManager Manager;

    public static void Init()
    {
        Manager = new UiManager();
    }

    public static UiBase Open(string name, Action callBack = null, params object[] args)
    {
        return Manager.Open(name, callBack, args);
    }

    public static UiBase Open<T>(Action callBack = null, params object[] args) where T : UiBase, new()
    {
        return Manager.Open<T>(callBack, args);
    }

    public static void Close<T>() where T : UiBase
    {
        Manager.Close<T>();
    }

    public static void Close(string name)
    {
        Manager.Close(name);
    }

    public static void Hide<T>() where T : UiBase
    {
        Manager.HideView<T>();
    }

    public static UiBase GetView(string name)
    {
        return Manager.GetView(name);
    }

    public static T GetView<T>() where T : UiBase
    {
        return Manager.GetView<T>();
    }

    public static bool IsOpenView<T>() where T : UiBase
    {
        var view = GetView<T>();

        if (view == null || view.uiStage != UiStage.show)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Update()
    {
        if (Manager != null)
        {
            Manager.Update();
        }
    }

    public static void Destroy(UiBase ui)
    {
        Manager.Destroy(ui);
    }

    public static void CloseAll()
    {
        Manager.CloseAll();
    }

    public static string GetUiAssetPath<T>() {

        return Manager.GetUiAssetPath<T>();
    }

    public static string GetUiAssetPath(Type type)
    {
        return Manager.GetUiAssetPath(type);
    }

    public static void CreateUISpecialEffect(int animCfgid, Action<UISpecialEffect> callBack)
    {
        Manager.CreateUISpecialEffect(animCfgid, callBack);
    }

    public static void SetUIType(UIType uiType, UiBase view)
    {
        Manager.SetUIType(uiType, view);
    }

    public static void ShowTips(string tips)
    {
        Manager.ShowTips(tips);
    }
}
