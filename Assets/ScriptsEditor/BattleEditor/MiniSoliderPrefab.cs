using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleEditor
{
    public class MiniSoliderPrefab : MonoBehaviour
    {
        Vector2 offsetV2;

        public void InitOffset(Vector2 vector2)
        {
            offsetV2 = vector2;
        }

        public void UpdatePosition(Vector2 vector2)
        {
            transform.position = vector2 + offsetV2;
        }
    }
}

