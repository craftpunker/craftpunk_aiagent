using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PowerChestItem : ItemBase
{
    public JSONNode cfg;
    U3dObj TxtNeedPower;
    public U3dObj BtnClaim;
    U3dObj PowerSlider;
    U3dObj ImgIcon;
    U3dObj TxtAmount;

    U3dObj RewardAnim;


    string pvpIcon = "PvpAtlas";
    protected override void OnInit(params object[] args)
    {
        base.OnInit(args);
        PowerSlider = Find("PowerSlider");
        TxtNeedPower = Find("TxtNeedPower");

        BtnClaim = Find("BtnClaim");
        SetOnClick(BtnClaim.gameObject, OnBtnClaim);

        ImgIcon = BtnClaim. Find("ImgIcon");
        TxtAmount = BtnClaim.Find("TxtAmount");

        RewardAnim = Find("BtnClaim/RewardAnim");

    }

    void OnBtnClaim() {

        if (!isCanFetch) {
            return;
        }

        //Debug.Log(this.data["cfgId"]);

        JSONObject jsObj = new JSONObject();
        jsObj.Add("cmd", "C2S_GET_POWER_REWARD");
        JSONObject data = new JSONObject();
        

        data.Add("cfgId", this.cfg["cfgId"]);

        jsObj.Add("data", data);
        WebSocketMain.instance.SendWebSocketMessage(jsObj);
    }

    public void SetData(JSONNode data)
    {
        this.cfg = data;
        
        JSONObject jso = new JSONObject();
        //jso.Add(data.Keys[0])
        //SetOnClick(BtnClaim,Cmd.instance.C2S_GET_POWER_REWARD())

        if ( data == null ) { }
        Refresh();
    }

    bool isCanFetch;

    protected override void OnRefresh()
    {
        TxtNeedPower.Text.text = cfg["needPower"];

        isCanFetch = false;

        float curPower = GameData.instance.PermanentData["power"];
        float itemPower = curPower;
        float itemNeedPower = cfg["needPower"];

        JSONNode lastCfg = GameData.instance.TableJsonDict["PowerChestConf"][(cfg["cfgId"] - 1).ToString()];
        if (lastCfg != null)
        {
            itemPower = math.max(0, curPower - lastCfg["needPower"]);
            itemNeedPower -= lastCfg["needPower"];
        }

        PowerSlider.Slider.value = itemPower / itemNeedPower;

        JSONNode reawrdCfg = cfg["reward"][0];
        JSONNode rewardItemCfg = GameData.instance.TableJsonDict["ItemConf"][reawrdCfg[0].ToString()];

        ImgIcon.SetSprite(rewardItemCfg["iconAtlas"], rewardItemCfg["icon"], false, () => {
            //ImgIcon.SetSpriteRectBaseW(90);
        });

        BtnClaim.SetSprite(rewardItemCfg["bgIconAtlas"], rewardItemCfg["bgIcon"]);

        TxtAmount.Text.text = reawrdCfg[1];

        JSONNode powerRwdData = GameData.instance.PermanentData["power_rwd"];
        JSONNode subPowerRwdData = null;

        if(powerRwdData != null){
            subPowerRwdData = powerRwdData[cfg["cfgId"].ToString()];
        }

        if (subPowerRwdData != null)
        {
            BtnClaim.SetImgGray(true);
        }
        else {
            BtnClaim.SetImgGray(false);
        }

        isCanFetch = subPowerRwdData == null && curPower >= cfg["needPower"];
        RewardAnim.SetActive(isCanFetch);
        BtnClaim.Button.interactable = isCanFetch;
    }

    public Vector3 GetSliderPos() {
        float len = PowerSlider.Slider.value * PowerSlider.RectTransform.rect.height;
        Vector3 pos = PowerSlider.RectTransform.position;
        pos.y += len;
        return pos;
    }

    protected override void OnRelease()
    {

    }
}
