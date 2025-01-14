using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Web3View : UiBase
{
    //LayoutAsk
    U3dObj LayoutAsk;
    U3dObj BtnInterested;
    U3dObj BtnUnInterested;

    //LayoutRegist
    U3dObj LayoutRegist;
    U3dObj BtnCloseRegist;

    U3dObj LayoutChain;
    Web3ChainItem web3ChainItem;
    U3dObj ImgArrow;

    U3dObj ChainScrollView;
    Dictionary<GameObject, ItemBase> DictChainItem;
    List<JSONNode> ChainDataList = new List<JSONNode>();

    U3dObj LayoutInput;
    U3dObj BtnHide;
    U3dObj BtnCopy;

    U3dObj BtnRegist;

    //LayoutConfirm
    U3dObj LayoutConfirm;

    U3dObj TxtConfirm;
    U3dObj TxtConfirmChain;
    U3dObj BtnConfirm;
    U3dObj BtnCancel;

    protected override void OnInit()
    {
        //LayoutAsk
        LayoutAsk = Find("LayoutAsk");
        BtnInterested = LayoutAsk.Find("BtnInterested");
        SetOnClick(BtnInterested, OnBtnInterested);
        BtnUnInterested = LayoutAsk.Find("BtnUnInterested");
        SetOnClick(BtnUnInterested, OnBtnUnInterested);

        //LayoutRegist
        LayoutRegist = Find("LayoutRegist");
        BtnCloseRegist = LayoutRegist.Find("BtnCloseRegist");
        SetOnClick(BtnCloseRegist, () => Close());

        LayoutChain = LayoutRegist.Find("LayoutChain");
        SetOnClick(LayoutChain, OnClickLayoutChain);
        ImgArrow = LayoutChain.Find("ImgArrow");

        web3ChainItem = ItemBase.GetItem<Web3ChainItem>(LayoutChain.FindGo("Web3ChainItem"));
        web3ChainItem.SetClickCallBack((cfg) =>
        {
            OnClickLayoutChain();
        });

        ChainScrollView = LayoutRegist.Find("ChainScrollView");
        DictChainItem = new Dictionary<GameObject, ItemBase>();
        ChainScrollView.LoopScrollView.Init(DictChainItem);
        ChainScrollView.LoopScrollView.SetRenderHandler(OnRenderChainItem);

        LayoutInput = LayoutRegist.Find("LayoutInput");
        //InputField = LayoutChain.Find("InputField");
        BtnHide = LayoutInput.Find("BtnHide");
        SetOnClick(BtnHide, OnBtnHide);
        BtnCopy = LayoutInput.Find("BtnCopy");
        SetOnClick(BtnCopy, OnBtnCopy);

        BtnRegist = LayoutRegist.Find("BtnRegist");
        SetOnClick(BtnRegist, OnBtnRegist);

        //LayoutConfirm
        LayoutConfirm = Find("LayoutConfirm");

        TxtConfirm = LayoutConfirm.Find("TxtConfirm");
        TxtConfirmChain = LayoutConfirm.Find("TxtConfirmChain");
        BtnConfirm = LayoutConfirm.Find("BtnConfirm");
        SetOnClick(BtnConfirm, OnBtnConfirm);
        BtnCancel = LayoutConfirm.Find("BtnCancel");
        SetOnClick(BtnCancel, OnBtnCancel);

    }

    protected override void OnShow()
    {
        EventDispatcher<CmdEventData>.instance.AddEvent("web3_status", OnWeb3Status);

        if (GameData.instance.PermanentData["web3_status"] == 1) {
            LayoutAsk.SetActive(false);
            LayoutRegist.SetActive(true);
            LayoutConfirm.SetActive(false);
            BtnRegist.SetActive(false);
            RefreshRegist();
            return;
        }

        BtnRegist.SetActive(true);
        LayoutAsk.SetActive(true);
        LayoutRegist.SetActive(false);
        LayoutConfirm.SetActive(false);

        ChainDataList.Clear();

        foreach (var item in GameData.instance.TableJsonDict["ChainConf"].Values)
        {
            ChainDataList.Add(item);
        }

        ChainScrollView.LoopScrollView.SetDataCount(ChainDataList.Count);

        web3ChainItem.SetData(ChainDataList[0]);
    }

    //LayoutAsk
    void OnBtnInterested()
    {
        LayoutAsk.SetActive(false);
        LayoutRegist.SetActive(true);
        LayoutConfirm.SetActive(false);

        RefreshRegist();
    }

    void OnBtnUnInterested()
    {
        Close();
    }

    void OnWeb3Status(string evt, CmdEventData[] actions) {
        OnShow();
    }

    //LayoutRegist

    bool isHide = false;

    void RefreshRegist(bool isReset = true) { 
        ChainScrollView.SetActive(false);
        ImgArrow.RectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        if (isReset) {
            isHide = false;
            BtnHide.SetSprite("Web3Atlas", "Ui_web_open");
            LayoutInput.InputField.contentType = UnityEngine.UI.InputField.ContentType.Standard;
            LayoutInput.InputField.ForceLabelUpdate();
        }

        int web3Status = GameData.instance.PermanentData["web3_status"];

        if (web3Status == 1)
        {
            JSONNode chainCfg = GameData.instance.TableJsonDict["ChainConf"];
            string chain = GameData.instance.PermanentData["web3_chain"];
            string address = GameData.instance.PermanentData["web3_address"];
            LayoutInput.InputField.text = address;
            LayoutInput.InputField.interactable = false;

            foreach (var item in chainCfg.Values)
            {
                if (item["name"] == chain)
                {
                    web3ChainItem.SetData(item);
                    break;
                }
            }
        }
        else {
            LayoutInput.InputField.interactable = true;
        }
    }

    void OnClickLayoutChain() {
        if (GameData.instance.PermanentData["web3_status"] == 1)
        {
            return;
        }

        ChainScrollView.SetActive(!ChainScrollView.gameObject.activeSelf);

        if (ChainScrollView.gameObject.activeSelf)
        {
            ImgArrow.RectTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else {
            ImgArrow.RectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    void OnBtnRegist() {

        if(LayoutInput.InputField.text.Trim() == string.Empty){
            UiMgr.ShowTips("input chain first");
            return;
        }

        LayoutAsk.SetActive(false);
        LayoutRegist.SetActive(false);
        LayoutConfirm.SetActive(true);
        RefreshConfirm();
    }

    void OnBtnHide() {
        isHide = !isHide;
        if (isHide)
        {
            BtnHide.SetSprite("Web3Atlas", "Ui_web_close");
            LayoutInput.InputField.contentType = UnityEngine.UI.InputField.ContentType.Password;
            LayoutInput.InputField.ForceLabelUpdate();
        }
        else {
            BtnHide.SetSprite("Web3Atlas", "Ui_web_open");
            LayoutInput.InputField.contentType = UnityEngine.UI.InputField.ContentType.Standard;
            LayoutInput.InputField.ForceLabelUpdate();
        }
    }
    void OnBtnCopy() {
        UnityEngine.GUIUtility.systemCopyBuffer = LayoutInput.InputField.text;
        UiMgr.ShowTips("copy succeed");
    }

    void OnRenderChainItem(GameObject go, int index)
    {
        Web3ChainItem item = ItemBase.GetItem<Web3ChainItem>(go, DictChainItem);
        item.SetData(ChainDataList[index]);
        item.SetClickCallBack(OnClickChainItem);
    }

    void OnClickChainItem(JSONNode cfg) {
        web3ChainItem.SetData(cfg);
        OnClickLayoutChain();
    }

    //LayoutConfirm

    void RefreshConfirm() {
        string name = web3ChainItem.cfg["name"].ToString().Trim();

        TxtConfirm.Text.text = $"You need to bind <color=#FF3800>{name}</color> chain";
        TxtConfirmChain.Text.text = LayoutInput.InputField.text.Trim();
    }

    void OnBtnConfirm() {
        JSONObject jsObj = new JSONObject();
        jsObj.Add("cmd", "C2S_SUBMIT_WEB3_INFO");
        JSONObject data = new JSONObject();
        data.Add("chain", web3ChainItem.cfg["name"]);
        data.Add("address", LayoutInput.InputField.text);
        jsObj.Add("data", data);
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    void OnBtnCancel() {
        LayoutAsk.SetActive(false);
        LayoutRegist.SetActive(true);
        LayoutConfirm.SetActive(false);
        RefreshRegist(false);
    }

    //====================

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        EventDispatcher<CmdEventData>.instance.RemoveEvent("web3_status", OnWeb3Status);
    }

    protected override void OnDestroy()
    {
        ChainScrollView.LoopScrollView.Release();
        web3ChainItem.Release();
    }
}
