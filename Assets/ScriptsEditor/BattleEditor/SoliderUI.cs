using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace BattleEditor
{
    public class SoliderUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        int cfgId;
        Image img_Icon;

        public GameObject soliderPrefab; // 预置的士兵模型
        private GameObject soliderShadow; // 士兵虚影
        private RectTransform rectTransform;
        private Canvas canvas;
        private Camera mainCamera;

        private bool isDragging = false;
        private Vector3 offset;

        private void Awake()
        {
            img_Icon = transform.Find("img_Icon").GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            mainCamera = Camera.main;
        }

        public void Init(int cfgId,Canvas _canvas)
        {
            this.cfgId = cfgId;
            img_Icon.sprite = EditorResManager.Instance.GetSprite(cfgId.ToString());
            canvas = _canvas;
        }

        // 当鼠标按下时调用
        public void OnPointerDown(PointerEventData eventData)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            mousePosition.z = 0;

            isDragging = true;
        }

        //开始拖动时调用
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 创建士兵虚影
            soliderShadow = Instantiate(soliderPrefab, GetWorldPosition(eventData), Quaternion.identity);
            soliderShadow.GetComponent<SoliderPrefab>().cfgId = cfgId;
            soliderShadow.GetComponent<SoliderPrefab>().level = 1;
            soliderShadow.GetComponent<SoliderPrefab>().count = 5;
            soliderShadow.GetComponent<SoliderPrefab>().UpdateSoliderData();
            soliderShadow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f); // 设置半透明
        }

        // 当拖动时调用
        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging)
            {
                if (soliderShadow != null)
                {
                    Debug.Log(eventData);
                    Vector2 v2 = GetWorldPosition(eventData);
                    Debug.Log(v2);
                    soliderShadow.transform.position = v2;
                }
            }
        }

        // 当鼠标抬起时调用
        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
        }



        public void OnEndDrag(PointerEventData eventData)
        {
            // 生成实际的士兵
            if (soliderShadow != null)
            {
                soliderShadow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1); // 恢复不透明
                                                                                            // 在这里可以添加逻辑，确定士兵是否放置在有效位置
                soliderShadow = null;
                isDragging = false;
            }
        }


        public Vector3 GetWorldPosition(PointerEventData eventData)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);
            return mainCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 0));
        }
    }

}
