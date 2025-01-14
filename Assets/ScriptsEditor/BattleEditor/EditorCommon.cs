using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleEditor
{
    public class EditorCommon
    {
        public static string Vector2ToFixVector2String(Vector2 vector)
        {
            return string.Format("[{0},{1}]", vector.x, vector.y);
        }
        public static string EscapeJsonString(string jsonString)
        {
            return jsonString
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        public static bool CheckIsOdd(int number)
        {
            return number % 2 != 0 ? true : false;
        }

    }
}

