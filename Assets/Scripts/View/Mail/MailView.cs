
using Battle;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MailView : UiBase
{
    U3dObj BgMail;
    U3dObj MailScrollView;
    U3dObj PropScrollView;
    U3dObj TxtContent;
    U3dObj BtnReceive;
    U3dObj BtnDelete;
    U3dObj Content;
    U3dObj TxtTitle;
    U3dObj BtnDeleteAll;
    U3dObj BtnReadAndCollect;
    U3dObj BtnClose;
    U3dObj NoMailBtnClose;
    U3dObj NoMail;
    U3dObj Mail;

    Dictionary<GameObject, ItemBase> MailItemList = new Dictionary<GameObject, ItemBase>();
    Dictionary<GameObject, ItemBase> MailPropItems = new Dictionary<GameObject, ItemBase>();
    //Dictionary<GameObject, ItemBase> _MailPropList = new Dictionary<GameObject, ItemBase>();

    private JSONNode MailData;

    private MailItem SelectMailItem;

    protected override void OnInit()
    {
        base.OnInit();

        NoMail = Find("NoMail");
        Mail = Find("Mail");

        BgMail = Find("Mail/BgMail");

        MailScrollView = BgMail.Find("MailScrollView");
        MailScrollView.LoopScrollView.Init(MailItemList);
        MailScrollView.LoopScrollView.SetRenderHandler(OnMailRenderItem);

        PropScrollView = BgMail.Find("PropScrollView");
        Content = PropScrollView.Find("Viewport/Content");
        //PropScrollView.LoopScrollView.Init(_MailPropList);
        //PropScrollView.LoopScrollView.SetRenderHandler(OnPropRenderItem);

        TxtContent = BgMail.Find("TxtContent");
        TxtTitle = BgMail.Find("TxtTitle");

        BtnReceive = Find("Mail/BtnReceive");
        SetOnClick(BtnReceive.gameObject, OnBtnReceive);

        BtnDelete = Find("Mail/BtnDelete");
        SetOnClick(BtnDelete.gameObject, OnBtnDelete);

        BtnDeleteAll = Find("Mail/BtnDeleteAll");
        SetOnClick(BtnDeleteAll.gameObject, OnBtnDeleteAll);

        BtnReadAndCollect = Find("Mail/BtnReadAndCollect");
        SetOnClick(BtnReadAndCollect.gameObject, OnBtnReadAndCollect);

        BtnClose = Find("Mail/BtnClose");
        SetOnClick(BtnClose.gameObject, OnBtnClose);

        NoMailBtnClose = Find("NoMail/BtnClose");
        SetOnClick(NoMailBtnClose.gameObject, OnBtnClose);

        for (int i = 0; i < Content.transform.childCount; i++)
        {
            var child = Content.transform.GetChild(i);
            ItemBase.GetItem<MailPropItem>(child, MailPropItems, this);
            //MailPropItems.Add(child.gameObject, ItemBase.GetItem<MailPropItem>(child, MailPropItems, this));
        }
    }

    private void ResetScrollViewDataCount()
    {
        MailData = GameData.instance.MailsData;
        MailScrollView.LoopScrollView.SetDataCount(MailData.Count);

        if (MailData.Count > 0)
        {
            Mail.SetActive(true);
            NoMail.SetActive(false);
        }
        else
        {
            Mail.SetActive(false);
            NoMail.SetActive(true);
        }
    }

    protected override void OnShow()
    {
        base.OnShow();

        Reflash();
        TxtContent.Text.text = "";
        TxtTitle.Text.text = "";
        //NoMail.gameObject.SetActive(false);
        //Mail.gameObject.SetActive(true);
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.mail, OnMailReceive);

        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.mailReward, OnRewardReceive);
    }

    void OnMailReceive(string evtName, CmdEventData[] evt) {
        var data = evt[0].JsonNode.Value[0];

        //if (data["read"] == 1) //
        //{
        //    return;
        //}
        Reflash();
        MailScrollView.LoopScrollView.SetRenderHandler(OnMailRenderItem);
    }

    void OnRewardReceive(string evtName, CmdEventData[] evt)
    {
        JSONNode data = evt[0].JsonNode;

        if (data["id"] == 0) {
            foreach (var reward in data["rewardItems"])
            {
                ResAnim.instance.ShowUIAnim(Convert.ToInt32(reward.Key), BtnReadAndCollect.transform.position, reward.Value);
            }
            return;
        }

        foreach (var item in MailItemList.Values) {
            MailItem mailItem = (MailItem)item;
            if (mailItem.MailData["id"] == data["id"]) {

                foreach (var reward in data["rewardItems"]) {
                    ResAnim.instance.ShowUIAnim(Convert.ToInt32(reward.Key), BtnReceive.transform.position, reward.Value);
                }
            }
        }
    }

    private void Reflash()
    {
        ResetScrollViewDataCount();
        BtnReceive.SetActive(false);
        BtnDelete.SetActive(false);
        HideAllPropItem();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnMailRenderItem(GameObject obj, int index)
    {
        MailItem item = ItemBase.GetItem<MailItem>(obj, MailItemList, this);
        var data = MailData[index];
        item.SetData(data, index);
        item.MailView = this;

        if (item.IsSelect)
        {
            item.ImgSelected.SetActive(true);
            BtnDelete.SetActive(true);

            if (item.MailData["rewards"].Count > 0)
            {
                var rewards = data["rewards"];
                for (int i = 0; i < rewards.Count; i++)
                {
                    var prop = MailPropItems.ElementAt(i);
                    var reward = rewards[i];
                    prop.Key.SetActiveEx(true);
                    MailPropItem propItem = prop.Value as MailPropItem;
                    propItem.SetData(reward);
                }

                if (item.MailData["draw"] == 0)
                {
                    BtnReceive.SetActive(true);
                }
            }
        }
    }

    private void OnPropRenderItem(GameObject obj, int index)
    {
        //MailPropItem propItem = ItemBase.GetItem<MailPropItem>(obj, _MailPropList, this);
    }

    private void OnBtnReceive()
    {
        if (SelectMailItem != null)
        {
            JSONObject jsonObj = new JSONObject();
            JSONObject jsonObj1 = new JSONObject();
            jsonObj1.Add("id", SelectMailItem.MailData["id"]);
            jsonObj.Add("data", jsonObj1);
            Cmd.instance.C2S_DRAW_ONE_MAIL(jsonObj);
        }
    }

    private void OnBtnDelete()
    {
        PopUpConfirmMsg popUpConfirmMsg = ClassPool.instance.Pop<PopUpConfirmMsg>();
        popUpConfirmMsg.Content = "Did you delete this mail?";
        popUpConfirmMsg.Btn1Txt = "Cancel";
        popUpConfirmMsg.Btn2Txt = "Confirm";
        popUpConfirmMsg.showWaitImg = true;
        popUpConfirmMsg.Btn1Func = () =>
        {
            UiMgr.Close<PopUpConfirmView>();
        };

        popUpConfirmMsg.Btn2Func = () =>
        {
            if (SelectMailItem != null)
            {
                TxtContent.Text.text = "";
                TxtTitle.Text.text = "";
                BtnReceive.SetActive(false);
                BtnDelete.SetActive(false);
                HideAllPropItem();

                JSONObject jsonObj = new JSONObject();
                JSONObject jsonObj1 = new JSONObject();
                jsonObj1.Add("id", SelectMailItem.MailData["id"]);
                jsonObj.Add("data", jsonObj1);
                Cmd.instance.C2S_DELETE_ONE_MAIL(jsonObj);
            }
            UiMgr.Close<PopUpConfirmView>();
        };

        UiMgr.Open<PopUpConfirmView>(null, popUpConfirmMsg);
    }

    private void OnBtnDeleteAll()
    {
        PopUpConfirmMsg popUpConfirmMsg = ClassPool.instance.Pop<PopUpConfirmMsg>();
        popUpConfirmMsg.Content = "Do you want to delete all read emails with one click(unclaimed emails will not be deleted)";
        popUpConfirmMsg.Btn1Txt = "Cancel";
        popUpConfirmMsg.Btn2Txt = "Confirm";
        popUpConfirmMsg.showWaitImg = true;
        popUpConfirmMsg.Btn1Func = () =>
        {
            UiMgr.Close<PopUpConfirmView>();
        };

        popUpConfirmMsg.Btn2Func = () =>
        {
            JSONObject jsonObj = new JSONObject();
            JSONObject jsonObj1 = new JSONObject();
            jsonObj.Add("data", jsonObj1);
            Cmd.instance.C2S_DELETE_ALL_MAIL(jsonObj);
            UiMgr.Close<PopUpConfirmView>();
        };

        UiMgr.Open<PopUpConfirmView>(null, popUpConfirmMsg);
    }

    private void OnBtnReadAndCollect()
    {
        JSONObject jsonObj = new JSONObject();
        JSONObject jsonObj1 = new JSONObject();
        jsonObj.Add("data", jsonObj1);
        Cmd.instance.C2S_READ_DRAW_ALL_MAIL(jsonObj);
    }

    private void OnBtnClose()
    {
        UiMgr.Close<MailView>();
    }

    public void OnMailItemSelected(MailItem item)
    {
        Reflash();

        var data = item.MailData;
        TxtContent.Text.text = data["content"];
        TxtTitle.Text.text = data["title"];
        var rewards = data["rewards"];

        BtnDelete.SetActive(true);
        if (rewards.Count > 0)
        {
            for (int i = 0; i < rewards.Count; i++)
            {
                var prop = MailPropItems.ElementAt(i);
                var reward = rewards[i];
                prop.Key.SetActiveEx(true);
                MailPropItem propItem = prop.Value as MailPropItem;
                propItem.SetData(reward);
            }

            BtnReceive.SetActive(true);
        }

        if (item.MailData["draw"] == 1)
        {
            BtnReceive.SetActive(false);
        }

        if (SelectMailItem != null)
        {
            SelectMailItem.ImgSelected.SetActive(false);
            SelectMailItem.IsSelect = false;
        }

        SelectMailItem = item;
        SelectMailItem.ImgSelected.SetActive(true);
        SelectMailItem.IsSelect = true;

        if (item.MailData["read"] == 0)
        {
            JSONObject jsonObj = new JSONObject();
            JSONObject jsonObj1 = new JSONObject();
            jsonObj1.Add("id", item.MailData["id"]);
            jsonObj.Add("data", jsonObj1);
            Cmd.instance.C2S_READ_ONE_MAIL(jsonObj);
        }
    }

    private void HideAllPropItem()
    {
        for (int i = 0; i < MailPropItems.Count; i++)
        {
            var prop = MailPropItems.ElementAt(i);
            prop.Key.SetActive(false);
        }
    }

    private void ResetBtnReceive()
    {
        if (SelectMailItem.MailData["draw"] == 1)
        {
            BtnReceive.SetActive(false);
        }
        else
        {
            if (SelectMailItem.MailData["rewards"].Count > 0)
            {
                BtnReceive.SetActive(true);
            }
        }
    }

    protected override void OnHide()
    {
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.mail, OnMailReceive);
        EventDispatcher<CmdEventData>.instance.RemoveEvent(EventName.mailReward, OnRewardReceive);

        foreach (var kv in MailItemList)
        {
            MailItem item = kv.Value as MailItem;
            item.IsSelect = false;
            item.ImgSelected.SetActive(false);
        }

        SelectMailItem = null;
        MailData = null;
        base.OnHide();
    }
}