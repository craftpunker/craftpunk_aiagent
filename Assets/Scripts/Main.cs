

using Battle;
using SimpleJSON;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoSingleton<Main>
{
    //private LockStepLogic lockStepLogic = new LockStepLogic();
    //private BattleLogic battleLogic = new BattleLogic();

    public float updataInterval = 0.5f;
    private double lastInterval;
    private int frames = 0;//
    private float fps;
    private ScreenOrientation screenOrientation;

    public FsmCompent<Main> Fsm = new FsmCompent<Main>();

    private bool isInitComplete;

    public Transform LoadingView;
    public Slider LoadingViewSlider;
    public Text SliderText;

    private void Awake()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 30;
        LoadingView = GameObject.Find("UIRoot/LoadingNode/LoadingView").transform;
        LoadingViewSlider = LoadingView.Find("Slider").GetComponent<Slider>();
        SliderText = LoadingViewSlider.transform.Find("Text (Legacy)").GetComponent<Text>();
        LoadingViewSlider.value = 0;

        isInitComplete = false;
        ResMgr.instance.Init(() =>
        {
            UiMgr.Init();
            ModuleData.Init();
            GameData.instance.Init();
            TimeMgr.instance.Init();
            MonoTimeMgr.instance.Init();
            BattleMgr.instance.Init();
            AnimMgr.instance.Init();
            BoardMgr.instance.Init();
            RedPointMgr.Init();
            MapMgr.instance.Init();
            GuideMgr.Init();

            AudioMgr.instance.PlayOneShot("ui_gameloading");
            Fsm.CreateFsm(this, new SceneLoginFsm(), new SceneMainFsm(), new SceneBattleFsm());
            if (AppConfig.Platform == "webgl")
            {
                Fsm.OnStart<SceneLoginFsm>();
            }

            isInitComplete = true;
        });
    }

    private void Start()
    {

        
        //UIMgr.instance.CreatePanel<MessageView>("MessageView", UINode.MessageNode);
        EventDispatcher<CmdEventData>.instance.AddEvent(EventName.networkAborted, (evtName, evt) =>
        {
            Fsm.ChangeFsmState<SceneLoginFsm>();

            JSONNode jsonnode = null;
            if (evt.Length > 0)
            {
                jsonnode = evt[0].JsonNode.Value;
            }

            if (jsonnode == null)
                return;

            if (UiMgr.IsOpenView<PopUpConfirmView>())
                return;

            EventDispatcher<bool>.instance.TriggerEvent(EventName.UI_ShowLoginWaitImg, false);

            PopUpConfirmMsg popUpConfirmMsg = ClassPool.instance.Pop<PopUpConfirmMsg>();
            popUpConfirmMsg.Content = jsonnode["msg"];
            popUpConfirmMsg.Btn1Txt = "Cancel";
            popUpConfirmMsg.Btn2Txt = "OK";
            popUpConfirmMsg.showWaitImg = false;
            popUpConfirmMsg.Btn1Func = () =>
            {
                UiMgr.Close<PopUpConfirmView>();
                //ConnectWebSocket(connectJsNode.ToString());
            };

            popUpConfirmMsg.Btn2Func = () =>
            {
                UiMgr.Close<PopUpConfirmView>();
            };

            UiMgr.Open<PopUpConfirmView>(null, popUpConfirmMsg);
        });
    }

    private void Update()
    {
        if (!isInitComplete && LoadingViewSlider != null)
        {
            LoadingViewSlider.value += Time.deltaTime * 0.5f;
            SliderText.text = "loading..." + Math.Round(LoadingViewSlider.value * 100, 1) + "%";
            return;
        }

        GuideMgr.Update();
        StepRun.Update();
        //lockStepLogic.UpdateLogic();
        Fsm.OnUpdate(this);

        MonoTimeMgr.instance.Update(Time.deltaTime);

        //frames++;
        //float timeNow = Time.realtimeSinceStartup;
        //if (timeNow > lastInterval + updataInterval)
        //{
        //    fps = (float)(frames / (timeNow - lastInterval));
        //    frames = 0;
        //    lastInterval = timeNow;
        //}

        if (Input.GetKeyDown(KeyCode.F5))
        {
            //UiMgr.Open<F5View>();
            if (UiMgr.IsOpenView<F5View>())
            {
                UiMgr.Close<F5View>();
            }
            else
            {
                UiMgr.Open<F5View>();
            }
        }
    }

    public void DestoryLoadingView()
    {
        if (LoadingView != null)
        {
            GameObject.Destroy(LoadingView.gameObject);
            LoadingViewSlider = null;
            LoadingView = null;
            SliderText = null;
        }
    }

    //private void OnGUI()
    //{
    //    GUI.skin.textField.fontSize = 60;
    //    GUILayout.TextField($"FPS:{fps}");
    //}
}
