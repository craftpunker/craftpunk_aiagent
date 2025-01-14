
using Battle;
using DG.Tweening;
using UnityEngine;

using UnityEngine.UI;

public class LoginView : UiBase
{
    InputField inputUsername;
    InputField inputPassword;
    U3dObj btnLogin;
    U3dObj toggleRemberMe;

    protected override void OnInit()
    {
        inputUsername = Find("Login/InputUserName").transform.GetComponent<InputField>();
        inputPassword = Find("Login/InputPassword").transform.GetComponent<InputField>();
        btnLogin = Find("Login/BtnLogin");
        toggleRemberMe = Find("Login/TogRemeber");
        SetOnClick(btnLogin, OnLogin);
        
    }

    void OnLogin()
    {
        if (inputUsername.text == string.Empty)
            return;

        WebSocketMain.instance.GetHttpAccount2Session(inputUsername.text);

        EventDispatcher<bool>.instance.TriggerEvent(EventName.UI_ShowLoginWaitImg, true);


        if (toggleRemberMe.Toggle.isOn)
        {
            PlayerPrefs.SetString("username", inputUsername.text);
        }
        else
        {
            PlayerPrefs.DeleteKey("username");
        }
    }

    protected override void OnShow()
    {
        Find("Login").SetActive(false);
        Find("Wait").SetActive(false);


        //toggleRemberMe.Toggle.onValueChanged.AddListener(HandleToggleValueChanged);

        if (AppConfig.Platform == "discord")
        {
            //WebSocketMain.instance.GetHttpAccount2Session("");

        }
        else
        {
            Find("Login").SetActive(true);
        }

        EventDispatcher<bool>.instance.AddEvent(EventName.UI_S2C_USER_LOGIN, (evtName, evts) =>
        {
            GameData.LoadingValue = 0;
            UiMgr.Open<LoadingView>();
            UiMgr.Close<LoginView>();
            
        });

        EventDispatcher<bool>.instance.AddEvent(EventName.UI_ShowLoginWaitImg, (evtName, evts) =>
        {
            bool value = evts[0];

            var imgWait = Find("Wait/ImgWait");
            imgWait.transform.DOKill();
            Find("Wait").SetActive(value);

            if (value == true)
            {
                Vector3 doR0tate_vct = new Vector3(0, 0, 360); //设置旋转的角度
                float useTime_r = 5;
                imgWait.transform.DORotate(doR0tate_vct, useTime_r, RotateMode.FastBeyond360).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
            }
        });

        if (PlayerPrefs.HasKey("username"))
        {
            string username = PlayerPrefs.GetString("username");
            inputUsername.text = username;

            toggleRemberMe.Toggle.isOn = true;
        }

        Main.instance.DestoryLoadingView();
    }

    //private void HandleToggleValueChanged(bool value)
    //{
    //    Debug.Log(value);
    //}

    protected override void OnUpdate()
    {
        
    }

    protected override void OnHide()
    {
        toggleRemberMe.Toggle.onValueChanged.RemoveAllListeners();
        EventDispatcher<bool>.instance.RemoveEventByName(EventName.UI_S2C_USER_LOGIN);
        EventDispatcher<bool>.instance.RemoveEventByName(EventName.UI_ShowLoginWaitImg);
    }

    protected override void OnDestroy()
    {

    }
}
