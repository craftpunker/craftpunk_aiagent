using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleEditor
{
    public class SoliderPrefab : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public int cfgId;
        public int level;

        public GameObject miniSolider;

        float soliderInterval = 0.35f;
        float soliderWeight = 0.38f;
        float soliderHeight = 0.38f;

        [HideInInspector]
        public string name;
        [HideInInspector]
        public string model;
        [HideInInspector]
        public int radius;
        [HideInInspector]
        public int moveSpeed;
        [HideInInspector]
        public int maxHp;
        [HideInInspector]
        public int atk;
        //[HideInInspector]
        public int count;
        [HideInInspector]
        public int atkSpeed;
        [HideInInspector]
        public int atkRange;
        [HideInInspector]
        public int inAtkRange;
        [HideInInspector]
        public int soldierType;
        [HideInInspector]
        public Vector2 atkSkillShowPos;
        [HideInInspector]
        public int atkSkillId;
        [HideInInspector]
        public Vector2 center;
        [HideInInspector]
        public int atkReadyTime;
        [HideInInspector]
        public int deadSkillId;
        [HideInInspector]
        public int bornSkillId;
        //[HideInInspector]
        public Vector2 pos;

        private bool isDragging = false;
        private Vector3 offset;

        public void UpdateSoliderData()
        {
            BattleSoliderData data = EditorResManager.Instance.GetBattleSoliderData(cfgId, level);


            for (int i = this.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(this.transform.GetChild(i).gameObject);
            }
            Debug.Log(data.cfgId + "   " + data.level);

            UpdateSoliderData(data);
        }

        public void UpdateSoliderData(BattleSoliderData data)
        {
            cfgId = data.cfgId;
            name = data.name;
            model = data.model;
            level = data.level;
            radius = (int)data.radius;
            moveSpeed = (int)data.moveSpeed;
            maxHp = (int)data.maxHp;
            atk = (int)data.atk;
            count = data.count;
            atkSpeed = (int)data.atkSpeed;
            atkRange = (int)data.atkRange;
            inAtkRange = (int)data.inAtkRange;
            soldierType = data.soldierType;
            atkSkillShowPos = data.atkSkillShowPos.ToVector2();
            atkSkillId = data.atkSkillId;
            center = data.center.ToVector2();
            atkReadyTime = data.atkReadyTime;
            deadSkillId = data.deadSkillId;
            bornSkillId = data.bornSkillId;
            InitMiniSolider(count);
        }

        public void InitMiniSolider(int count)
        {
            int maxSoldiersPerColumn = 4;           //一排士兵数量
            int colCount = maxSoldiersPerColumn;    //列数，本质等于一列多少个士兵
            int rowCount = Mathf.CeilToInt(count / (float)maxSoldiersPerColumn);   //计算多少排
            int soldiersInLastRow = count % colCount;   // 最后一排多少个兵，用以居中最后士兵

            //Debug.Log(string.Format("有{0}排，有{1}列,最后一列有{2}", rowCount, colCount,soldiersInLastRow));

            for (int i = 0; i < count; i++)
            {
                int row = i / maxSoldiersPerColumn; //第几排士兵
                int col = i % maxSoldiersPerColumn; //第几列士兵

                if (row == rowCount - 1 && soldiersInLastRow > 0)
                {
                    Debug.Log("我是最后一排");
                    colCount = soldiersInLastRow;
                }

                Vector2 position = CalculatePosition(row, col, rowCount, colCount);
                GameObject go = Instantiate(miniSolider);
                go.transform.SetParent(this.transform);
                go.GetComponent<MiniSoliderPrefab>().InitOffset(position);
                go.GetComponent<SpriteRenderer>().sprite = EditorResManager.Instance.GetSprite(cfgId.ToString());
                go.name = string.Format("第{0}排，第{1}列", row, col);
                //Debug.Log( string.Format("我是第{0}排，第{1}列士兵，我的位置是{2}", row, col, position));
                go.GetComponent<MiniSoliderPrefab>().UpdatePosition(transform.localPosition);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row">士兵排</param>
        /// <param name="col">士兵列</param>
        /// <param name="rowAmount">总排数</param>
        /// <param name="colAmount">所在排的列数</param>
        /// <returns></returns>
        private Vector2 CalculatePosition(int row, int col, int rowAmount, int colAmount)
        {
            //Debug.Log(colAmount);
            bool rowIsOdd = EditorCommon.CheckIsOdd(rowAmount);
            bool colIsOdd = EditorCommon.CheckIsOdd(colAmount);
            float x = rowIsOdd ? ((rowAmount / 2.0f - 0.5f) - row) * (soliderInterval + soliderWeight) : ((rowAmount / 2.0f - row) - 0.5f) * (soliderInterval + soliderWeight);
            float y = colIsOdd ? ((colAmount / 2.0f - 0.5f) - col) * (soliderInterval + soliderHeight) : ((colAmount / 2.0f - col) - 0.5f) * (soliderInterval + soliderHeight);
            //Debug.Log(new Vector2(x, y));
            return new Vector2(x, y);
        }

        public void UpdateMiniSoliderPosition()
        {

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("aaa");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            mousePosition.z = 0;

            isDragging = true;
            offset = transform.position - mousePosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
                mousePosition.z = 0;
                transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pos.x = transform.position.x;
            pos.y = transform.position.y;
            isDragging = false;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SoliderPrefab))]
    public class BattleEditorSolider : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SoliderPrefab solider = (SoliderPrefab)target;

            if (GUILayout.Button("更新军队数值"))
            {
                solider.UpdateSoliderData();
            }

        }
    }
#endif
}
