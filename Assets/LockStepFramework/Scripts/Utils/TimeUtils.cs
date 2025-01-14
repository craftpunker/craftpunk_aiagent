#if _CLIENTLOGIC_
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUtils
{
    private static float secondsPerMonth = 30.44f * 24 * 3600;

    public struct TimeHMS {
        public int Day;
        public int Hour;
        public int Minute;
        public int Second;
    }

    public struct TimeMD
    {
        public int Month;
        public int Day;
    }

    public static TimeHMS DHMS(float s) {

        float day;
        float hour;
        float minute;
        float second;

        day = MathF.Floor(s / (24 * 3600));

        hour = Mathf.Floor((s - day * 24 * 3600) / 3600);
        minute = Mathf.Floor((s - day * 24 * 3600 - hour * 3600) / 60);
        second = s % 60;

        return new TimeHMS() {Day = (int)day,  Hour = (int)hour, Minute = (int)minute, Second = (int)second };
    }

    public static TimeMD MD(float s)
    {
        float month;
        float day;

        month = (int)MathF.Floor(s / secondsPerMonth);
        s -= month * secondsPerMonth;
        day = (int)MathF.Floor(s / (24 * 3600));

        return new TimeMD() {Month = (int)month, Day = (int)day};
    }
}
#endif
