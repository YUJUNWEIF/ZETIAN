//using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby
{
    public enum TimeType
    {
        Time = 0,
        RealTime = 1,
        SyncTime = 2,
        DisplayTime = 3,
    }
    public class WaitSeconds : Util.IYieldInstruction
    {
        TimeType m_timeType;
        float m_end;
        public static Func<TimeType, float> timeGetter;
        WaitSeconds(float sec, TimeType timeType)
        {
            m_end = sec + timeGetter(m_timeType = timeType);
        }
        public bool done
        {
            get
            {
                return timeGetter(m_timeType) > m_end;
            }
        }
        public static IEnumerator Delay(float delaySec, TimeType timeType = TimeType.Time)
        {
            yield return new WaitSeconds(delaySec, timeType);
        }
        public static IEnumerator Delay(System.Action action, float delaySec, TimeType timeType = TimeType.Time)
        {
            yield return new WaitSeconds(delaySec, timeType);
            action();
        }
    }
}
