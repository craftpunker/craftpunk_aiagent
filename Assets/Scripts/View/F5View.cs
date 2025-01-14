
using Battle;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class TestRange
{
    public EntityBase Entity;
    public Transform Range;

    //0:radius 1:atkRange
    public void Init(EntityBase entity, int type)
    {
        Entity = entity;
        ResMgr.instance.LoadGameObjectAsync("TestRange", (go) =>
        {
            Range = go.transform;
            float size = (type == 0 ? (float)Entity.Radius : (float)Entity.AtkRange) * 2;
            Range.localScale = new Vector3(size, size, size);
        });
    }

    public void Release()
    {
        ClassPool.instance.Push(this);
        ResMgr.instance.ReleaseGameObject(Range.gameObject);
        Entity = null;
    }
}

public class TestFsm
{
    public EntityBase Entity;
    public SoldierFlagBase SoldierFlag;
    public TextMesh Text;

    //0:radius 1:atkRange
    public void Init(EntityBase entity)
    {
        Entity = entity;
        //Text = ResMgr.instance.LoadGameObject("Test/TestText").GetComponent<TextMesh>();
        ResMgr.instance.LoadGameObjectAsync("TestText", (go) =>
        {
            Text = go.GetComponent<TextMesh>();
        });
    }

    public void Init(SoldierFlagBase flag)
    {
        SoldierFlag = flag;
        ResMgr.instance.LoadGameObjectAsync("TestText", (go) =>
        {
            Text = go.GetComponent<TextMesh>();
        });
    }

    public void Release()
    {
        ClassPool.instance.Push(this);
        ResMgr.instance.ReleaseGameObject(Text.gameObject);
        Entity = null;
        SoldierFlag = null;
    }
}

public class F5View : UiBase
{
    private Toggle toggle_size;
    private Toggle toggle_atkRange;
    private Toggle toggle_fsm;
    private Toggle toggle_soldierType;
    private Toggle toggle_flagFsm;

    private bool showEntitySize;
    private bool showEntityAtkRange;
    private bool showEntityFsm;
    private bool showSoldierType;
    private bool showFlagFsm;

    private List<TestRange> testSizeRanges;
    private List<TestRange> testAtkRanges;
    private List<TestFsm> textMeshes;

    private InputField InputFieldAction;
    private InputField InputFieldValue;
    private InputField InputFieldResult;
    private InputField InputFieldMsg;
    private Button ButtonGMSend;
    private Button ButtonExportJson;
    private Button ButtonClear;
    private Button ButtonMsg;

    protected override void OnInit()
    {
        toggle_size = transform.Find("Toggle_Size").GetComponent<Toggle>();
        toggle_size.isOn = false;

        toggle_atkRange = transform.Find("Toggle_AtkRange").GetComponent<Toggle>();
        toggle_atkRange.isOn = false;

        toggle_fsm = transform.Find("Toggle_FSM").GetComponent<Toggle>();
        toggle_fsm.isOn = false;

        toggle_soldierType = transform.Find("Toggle_SoldierType").GetComponent<Toggle>();
        toggle_soldierType.isOn = false;

        toggle_flagFsm = transform.Find("Toggle_FlagFsm").GetComponent<Toggle>();
        toggle_flagFsm.isOn = false;

        InputFieldAction = transform.Find("CmdInput/InputFieldAction").GetComponent<InputField>();
        InputFieldValue = transform.Find("CmdInput/InputFieldValue").GetComponent<InputField>();
        ButtonGMSend = transform.Find("CmdInput/ButtonGMSend").GetComponent<Button>();
        ButtonExportJson = transform.Find("ButtonExportJson").GetComponent<Button>();

        InputFieldResult = transform.Find("CmdInput/InputFieldResult").GetComponent<InputField>();
        InputFieldMsg = transform.Find("CmdInput/InputFieldMsg").GetComponent<InputField>();
        ButtonClear = transform.Find("CmdInput/ButtonClear").GetComponent<Button>();
        ButtonMsg = transform.Find("CmdInput/ButtonMsg").GetComponent<Button>();
    }

    protected override void OnShow()
    {
        toggle_size.onValueChanged.AddListener((value) =>
        {
            showEntitySize = value;
            if (value)
            {
                if (testSizeRanges == null)
                    testSizeRanges = new List<TestRange>();

                var count = BattleMgr.instance.SoldierList.Count;

                for (int i = 0; i < count; i++)
                {
                    var testRange = ClassPool.instance.Pop<TestRange>();
                    testRange.Init(BattleMgr.instance.SoldierList[i], 0);
                    testSizeRanges.Add(testRange);
                }
            }
            else
            {
                if (testSizeRanges == null)
                    return;

                foreach (var testRange in testSizeRanges)
                {
                    testRange.Release();
                }

                testSizeRanges.Clear();
            }
        });

        toggle_atkRange.onValueChanged.AddListener((value) =>
        {
            showEntityAtkRange = value;
            if (value)
            {
                if (testAtkRanges == null)
                    testAtkRanges = new List<TestRange>();

                var count = BattleMgr.instance.SoldierList.Count;

                for (int i = 0; i < count; i++)
                {
                    var testRange = ClassPool.instance.Pop<TestRange>();
                    testRange.Init(BattleMgr.instance.SoldierList[i], 1);
                    testAtkRanges.Add(testRange);
                }
            }
            else
            {
                if (testAtkRanges == null)
                    return;

                foreach (var testRange in testAtkRanges)
                {
                    testRange.Release();
                }

                testAtkRanges.Clear();
            }
        });

        toggle_fsm.onValueChanged.AddListener((value) =>
        {
            showEntityFsm = value;
            if (value)
            {
                if (textMeshes == null)
                    textMeshes = new List<TestFsm>();

                var count = BattleMgr.instance.SoldierList.Count;

                for (int i = 0; i < count; i++)
                {
                    var testFsm = ClassPool.instance.Pop<TestFsm>();
                    testFsm.Init(BattleMgr.instance.SoldierList[i]);
                    textMeshes.Add(testFsm);
                }
            }
            else
            {
                if (textMeshes == null)
                    return;

                foreach (var textMeshe in textMeshes)
                {
                    textMeshe.Release();
                }

                textMeshes.Clear();
            }
        });

        toggle_soldierType.onValueChanged.AddListener((value) =>
        {
            showSoldierType = value;
            if (value)
            {
                if (textMeshes == null)
                    textMeshes = new List<TestFsm>();

                var count = BattleMgr.instance.SoldierList.Count;

                for (int i = 0; i < count; i++)
                {
                    var testFsm = ClassPool.instance.Pop<TestFsm>();
                    testFsm.Init(BattleMgr.instance.SoldierList[i]);
                    textMeshes.Add(testFsm);
                }
            }
            else
            {
                if (textMeshes == null)
                    return;

                foreach (var textMeshe in textMeshes)
                {
                    textMeshe.Release();
                }

                textMeshes.Clear();
            }
        });

        toggle_flagFsm.onValueChanged.AddListener((value) =>
        {
            showFlagFsm = value;
            if (value)
            {
                if (textMeshes == null)
                    textMeshes = new List<TestFsm>();

                var count = BattleMgr.instance.SoldierFlagDict.Count;

                for (int i = 0; i < count; i++)
                {
                    var testFsm = ClassPool.instance.Pop<TestFsm>();
                    testFsm.Init(BattleMgr.instance.SoldierFlagDict.ElementAt(i).Value);
                    textMeshes.Add(testFsm);
                }
            }
            else
            {
                if (textMeshes == null)
                    return;

                foreach (var textMeshe in textMeshes)
                {
                    textMeshe.Release();
                }

                textMeshes.Clear();
            }
        });

        ButtonGMSend.onClick.AddListener(() =>
        {
            string action = InputFieldAction.text;
            string value = InputFieldValue.text;

            JSONObject msg = new JSONObject();
            msg.Add("cmd", "C2S_GM_OPERATE");

            JSONObject data = new JSONObject();
            data.Add("action", action);
            data.Add("value", value);
            msg.Add("data", data);
            WebSocketMain.instance.SendWebSocketMessage(msg);

            if (InputFieldResult.text == "")
            {
                InputFieldResult.text = InputFieldResult.text + "operate:" + action + " " + value;
            }
            else 
            {
                InputFieldResult.text = InputFieldResult.text + "\n" + "operate:" + action + " " + value;
            }
        });

        ButtonClear.onClick.AddListener(() =>
        {
            InputFieldResult.text = "";
        });

        ButtonMsg.onClick.AddListener(() =>
        {
            string text = InputFieldMsg.text;
            var jsonNode = JSONNode.Parse(text);
            WebSocketMain.instance.SendWebSocketMessage(jsonNode.AsObject);
        });

        ButtonExportJson.onClick.AddListener(() =>
        {
            List<ToJsonSoldierData> soldierDatas = new List<ToJsonSoldierData>();   
            foreach (var item in GameData.instance.PlayerBattleData.troops)
            {
                ToJsonSoldierData d = new ToJsonSoldierData();
                d.cfgId = item.Value.cfgId;
                d.level = item.Value.level;
                d.posGrid = GameUtils.FindNearestGrid(new Vector3((float)item.Value.pos.x, (float)item.Value.pos.y));
                soldierDatas.Add(d);
            }
            string text = "";
            for (int i = 0; i < soldierDatas.Count; i++)
            {
                text += string.Format("{0},{1},{2}", soldierDatas[i].cfgId, soldierDatas[i].level, soldierDatas[i].posGrid);
                if (i < soldierDatas.Count-1)
                {
                    text += ";";
                }
            }
            GUIUtility.systemCopyBuffer = text;
        });

        EventDispatcher<string>.instance.AddEvent(EventName.UI_GMOperateRet, (evtName, evt) =>
        {
            var msg = evt[0];
            if (InputFieldResult.text == "")
            {
                InputFieldResult.text = InputFieldResult.text + msg;
            }
            else
            {
                InputFieldResult.text = InputFieldResult.text + "\n" + msg;
            }
        });
    }

    protected override void OnUpdate()
    {
        if (showEntitySize)
        {
            for (int i = testSizeRanges.Count - 1; i >= 0; i--)
            {
                var testRange = testSizeRanges[i];

                if (testRange.Entity.BKilled)
                {
                    testRange.Release();
                    testSizeRanges.Remove(testRange);
                    continue;
                }

                testRange.Range.position = testRange.Entity.Fixv3LogicPosition.ToVector3();
            }
        }

        if (showEntityAtkRange)
        {
            for (int i = testAtkRanges.Count - 1; i >= 0; i--)
            {
                var testRange = testAtkRanges[i];

                if (testRange.Entity.BKilled)
                {
                    testRange.Release();
                    testAtkRanges.Remove(testRange);
                    continue;
                }

                testRange.Range.position = testRange.Entity.Fixv3LogicPosition.ToVector3();
            }
        }

        if (showEntityFsm)
        {
            for (int i = textMeshes.Count - 1; i >= 0; i--)
            {
                var testMesh = textMeshes[i];

                if (testMesh.Entity.BKilled)
                {
                    testMesh.Release();
                    textMeshes.Remove(testMesh);
                    continue;
                }

                testMesh.Text.transform.position = testMesh.Entity.Fixv3LogicPosition.ToVector3();
                testMesh.Text.text = testMesh.Entity.Fsm?.GetCurrState().ToString();
                //testMesh.Text.text = testMesh.Entity.Hp.ToString();
            }
        }

        if (showSoldierType)
        {
            for (int i = textMeshes.Count - 1; i >= 0; i--)
            {
                var testMesh = textMeshes[i];

                if (testMesh.Entity.BKilled)
                {
                    testMesh.Release();
                    textMeshes.Remove(testMesh);
                    continue;
                }

                testMesh.Text.transform.position = testMesh.Entity.Fixv3LogicPosition.ToVector3();
                //testMesh.Text.text = testMesh.Entity.Fsm?.GetCurrState().ToString();
                testMesh.Text.text = testMesh.Entity.SoldierType.ToString();
            }
        }

        if (showFlagFsm)
        {
            for (int i = textMeshes.Count - 1; i >= 0; i--)
            {
                var testMesh = textMeshes[i];

                if (testMesh.SoldierFlag.BKilled)
                {
                    testMesh.Release();
                    textMeshes.Remove(testMesh);
                    continue;
                }

                testMesh.Text.transform.position = testMesh.SoldierFlag.Pos.ToVector3();
                //testMesh.Text.text = testMesh.Entity.Fsm?.GetCurrState().ToString();
                testMesh.Text.text = testMesh.SoldierFlag.Fsm?.GetCurrState().ToString();
            }
        }
    }

    protected override void OnHide()
    {
        toggle_size.onValueChanged.RemoveAllListeners();
        toggle_atkRange.onValueChanged.RemoveAllListeners();
        toggle_fsm.onValueChanged.RemoveAllListeners();
        toggle_soldierType.onValueChanged.RemoveAllListeners();
        ButtonGMSend.onClick.RemoveAllListeners();
        ButtonClear.onClick.RemoveAllListeners();
        ButtonMsg.onClick.RemoveAllListeners();
        EventDispatcher<int>.instance.RemoveEventByName(EventName.UI_GMOperateRet);
    }

    protected override void OnDestroy()
    {

    }

    public class ToJsonSoldierData
    {
        public int cfgId;
        public int level;
        public int posGrid;
    }
}
