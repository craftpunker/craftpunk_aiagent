using Battle;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

namespace BattleEditor
{
    public class BattleEditorManager : MonoBehaviour
    {
        public static BattleEditorManager Instance;

        private void Awake()
        {
            Instance = this;
            GetComponent<EditorResManager>().Init();
            GameObject cam = Camera.main.gameObject;
            Physics2DRaycaster phy2d;
            if (!cam.TryGetComponent(out phy2d))
            {
                cam.AddComponent<Physics2DRaycaster>();
            }

        }

        public static Dictionary<string, TroopsData> troopsDataDict = new Dictionary<string, TroopsData>();

        // 物体的预制件
        public GameObject root;
        public GameObject testSolider;
        public int soliderCount;

        // 创建物体的方法
        public void CreateSolider()
        {
            for (int i = root.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(root.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < soliderCount; i++)
            {
                GameObject go = Instantiate(testSolider);
                go.transform.SetParent(root.transform);
                BattleSoliderData data = EditorResManager.Instance.GetBattleSoliderData(10001, 1);
                go.GetComponent<SoliderPrefab>().UpdateSoliderData(data);
            }

        }

        public void CopyToClipboard()
        {
            string textToCopy = "";

            PlayerData testPlayer = new PlayerData();
            testPlayer.cfgid = 10001;
            testPlayer.name = "player1";
            testPlayer.winRate = 500;
            testPlayer.icon = "gpamopof";
            testPlayer.score = 200;
            testPlayer.troops = new List<BattleSoliderDataJson>();


            for (int i = 0; i < root.transform.childCount; i++)
            {
                GameObject go = root.transform.GetChild(i).gameObject;
                BattleSoliderDataJson testSoliderDataJson = new BattleSoliderDataJson(go.GetComponent<SoliderPrefab>());
                testPlayer.troops.Add(testSoliderDataJson);
            }
            // 将对象转换为JSON字符串
            textToCopy = "\"" + EditorCommon.EscapeJsonString(JsonUtility.ToJson(testPlayer)) + "\";";
            GUIUtility.systemCopyBuffer = textToCopy;
            Debug.Log("Text copied to clipboard: " + textToCopy);
        }

    }

#if UNITY_EDITOR
        // 自定义编辑器类
        [CustomEditor(typeof(BattleEditorManager))]
        public class TestManagerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                BattleEditorManager battleEditorManager = BattleEditorManager.Instance;

                if (GUILayout.Button("初始化军队"))
                {
                    battleEditorManager.CreateSolider();
                }
                if (GUILayout.Button("添加1支军队"))
                {
                    Debug.Log("未实现"); 
                }
                if (GUILayout.Button("将军队信息导出至剪贴板"))
                {
                    battleEditorManager.CopyToClipboard();
                }
            }
        }
#endif
}

