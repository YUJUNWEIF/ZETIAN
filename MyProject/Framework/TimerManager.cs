using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class TimerManager : Singleton<TimerManager>
    {
        TimerHierarchyManager impl = new TimerHierarchyManager();
        public Util.ParamActions onFrameUpdate = new Util.ParamActions();
        float m_lastVerifTime;
        long m_srvTimeMs;
        long m_realTimeMs;
        int m_deltaMs;
        public void Synchronize(long iclock)
        {
            m_srvTimeMs = iclock * 1000;
            m_lastVerifTime = Time.realtimeSinceStartup;
            CorrectUtc();
        }
        public void Add(Action timer, long intervalMs)
        {
            impl.Add(timer, intervalMs, -1, m_realTimeMs);
        }
        public void Add(Action timer, long intervalMs, int repeat)
        {
            impl.Add(timer, intervalMs, repeat, m_realTimeMs);
        }
        public bool Remove(Action timer)
        {
            return impl.Remove(timer);
        }
        public bool Exist(Action timer)
        {
            return impl.Exist(timer);
        }
        public void FrameUpdate(float deltaTimeSec)
        {
            CorrectUtc();
            m_deltaMs = Mathf.RoundToInt(deltaTimeSec * 1000);
            impl.Update(m_realTimeMs, m_deltaMs);
            onFrameUpdate.Fire();
        }
        public long RealTimeMS() { return m_realTimeMs; }
        public int DeltaTimeMS() { return m_deltaMs; }
        void CorrectUtc()
        {
            m_realTimeMs = m_srvTimeMs + (long)((Time.realtimeSinceStartup - m_lastVerifTime) * 1000);
        }
    }
}