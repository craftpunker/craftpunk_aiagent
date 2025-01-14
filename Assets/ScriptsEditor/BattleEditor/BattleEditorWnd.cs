using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleEditor
{
    public class BattleEditorWnd : MonoBehaviour
    {
        public GameObject BESoliderUI;
        Button btn_ShowSoliderPanel;
        Canvas uiCanvas;

        void Awake()
        {
            transform.Find("btn_ShowSoliderPanel").GetComponent<Button>().onClick.AddListener(InitSoliderPanel);
            uiCanvas = GetComponentInParent<Canvas>();
        }

        private void InitSoliderPanel()
        {
            Debug.Log("111");
            Transform soliderPanel = transform.Find("SoliderPanel/Viewport/Content");
            if (soliderPanel.childCount != EditorResManager.Instance.GetAllBaseSoliderData().Count)
            {
                foreach (var item in EditorResManager.Instance.GetAllBaseSoliderData())
                {
                    GameObject img = Instantiate(BESoliderUI);
                    img.transform.SetParent(soliderPanel);
                    img.transform.GetComponent<SoliderUI>().Init(int.Parse(item.Key),uiCanvas);
                }
            }
        }
    }

}
