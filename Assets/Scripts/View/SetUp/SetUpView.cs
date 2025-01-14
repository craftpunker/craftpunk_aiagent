
using Battle;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SetUpView : UiBase
{
    U3dObj TxtPlayerIdValue;
    U3dObj SwtogMusic;
    U3dObj SwtogSound;
    U3dObj Mask;

    U3dObj BtnLanguage;
    U3dObj BtnTOS;
    U3dObj BtnPP;

    SwitchToggle SwitchToggleBgm;

    SwitchToggle SwitchToggleSound;

    SwitchToggle SwitchToggleBlood;

    protected override void OnInit()
    {
        base.OnInit();

        SwitchToggleBgm = Find("BG/SwtogMusic").gameObject.GetComponent<SwitchToggle>();
        SwitchToggleBgm.Toggle.onValueChanged.AddListener(OnSwitchBgm);

        SwitchToggleSound = Find("BG/SwtogSound").gameObject.GetComponent<SwitchToggle>();
        SwitchToggleSound.Toggle.onValueChanged.AddListener(OnSwitchSound);

        SwitchToggleBlood = Find("BG/SwitogBlood").gameObject.GetComponent<SwitchToggle>();
        SwitchToggleBlood.Toggle.onValueChanged.AddListener(OnSwitchBlood);

        TxtPlayerIdValue = Find("BG/TxtPlayerIdValue");

        BtnLanguage = Find("BG/BtnLanguage");
        SetOnClick(BtnLanguage, () =>
        {
            UiMgr.ShowTips("early access only english");
        });

        BtnTOS = Find("BG/BtnTOS");
        SetOnClick(BtnTOS, () =>
        {
            Application.OpenURL(AppConfig.TermsUrl);
        });

        BtnPP = Find("BG/BtnPP");
        SetOnClick(BtnPP, () =>
        {
            Application.OpenURL(AppConfig.PrivacyPolicyUrl);
        });

        Mask = Find("Mask");
        SetOnClick(Mask, () =>
        {
            UiMgr.Close<SetUpView>();
        });
    }

    protected override void OnShow()
    {
        base.OnShow();

        var value = PlayerPrefs.GetInt("isOnBGM") == 0 ? false : true;
        OnSwitchBgm(value);

        var value1 = PlayerPrefs.GetInt("isOnSound") == 0 ? false : true;
        OnSwitchSound(value1);

        var value2 = PlayerPrefs.GetInt("isOpenBlood") == 0 ? false : true;
        OnSwitchBlood(value2);

        TxtPlayerIdValue.Text.text = GameData.instance.UserData["uid"];

    }

    public void OnSwitchBgm(bool value)
    {
        AudioMgr.instance.SwitchBgmVolume(value);
        SwitchToggleBgm.OnSwitch(value);
    }

    public void OnSwitchSound(bool value)
    {
        AudioMgr.instance.SwitchSoundVolume(value);
        SwitchToggleSound.OnSwitch(value);
    }

    public void OnSwitchBlood(bool value)
    {
        GameData.instance.IsOpenBlood = value;
        PlayerPrefs.SetInt("isOpenBlood", value ? 1 : 0);
        SwitchToggleBlood.OnSwitch(value);
    }

    protected override void OnHide()
    {
        base.OnHide();
    }
}
