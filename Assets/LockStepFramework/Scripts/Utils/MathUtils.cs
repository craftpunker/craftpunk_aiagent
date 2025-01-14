
using System;

namespace Battle
{
    public class MathUtils
    {
        
        public static bool CheckRect(FixVector2 originDL, FixVector2 originTR, FixVector2 targetDL, FixVector2 targetTR)
        {
            if (Fix64.Max(originDL.x, targetDL.x) <= Fix64.Min(originTR.x, targetTR.x) && Fix64.Max(originDL.y, targetDL.y) <= Fix64.Min(originTR.y, targetTR.y))
            {
                return true;
            }

            return false;
        }

        public static float CombineToFloat(int integerPart, int decimalPart)
        {
            // 
            float decimalValue = decimalPart / (float)Math.Pow(10, decimalPart.ToString().Length);

            // 
            float result = integerPart + decimalValue;

            return result;
        }

        public static FixVector3 Bezier2(FixVector3 p0, FixVector3 p1, FixVector3 p2, Fix64 t)
        {
            var t1 = 1 - t;
            return t1 * t1 * p0 + 2 * t * t1 * p1 + t * t * p2;
        }

        public static FixVector3 Bezier2Angle(FixVector3 p0, FixVector3 p1, FixVector3 p2, Fix64 t)
        {
            Fix64 u = 1 - t;
            return 2 * u * (p1 - p0) + 2 * t * (p2 - p1);
        }

        //
        public static FixVector3 MoveStraight(Fix64 timeScale, FixVector3 start2End)
        {
            return start2End * timeScale;
        }
    }
}
