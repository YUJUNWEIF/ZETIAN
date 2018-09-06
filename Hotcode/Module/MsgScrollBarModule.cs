using System;
using System.Collections.Generic;

namespace geniusbaby
{
    public struct MsgScrollBar
    {
        public string msg;
        public int count;
    }
    public class MsgScrollBarModule : Singleton<MsgScrollBarModule>, IModule
    {
        const int MaxQueque = 20;
        List<MsgScrollBar> m_loopMsgs = new List<MsgScrollBar>();
        LinkedList<MsgScrollBar> m_counterMsgs = new LinkedList<MsgScrollBar>();
        int m_index = 0;
        public Util.ParamActions boardcastEvent = new Util.ParamActions();
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit()
        {
            m_counterMsgs.Clear();
            m_loopMsgs.Clear();
            m_index = 0;
        }
        public void SyncMsgs(List<MsgScrollBar> msgs)
        {
            OnMainExit();
            DisplayMsgs(msgs);
        }
        public void DisplayMsgs(List<MsgScrollBar> msgs)
        {
            for (int index = 0; index < msgs.Count; ++index)
            {
                var msg = msgs[index];
                if (msg.count > 0)
                {
                    m_counterMsgs.AddLast(msg);
                    if (m_counterMsgs.Count > MaxQueque)
                    {
                        m_counterMsgs.RemoveFirst();
                    }
                }
                else
                {
                    m_loopMsgs.Add(msg);
                }
            }
            boardcastEvent.Fire();
        }
        public void DisplayImmediate(MsgScrollBar msg)
        {
            m_counterMsgs.AddFirst(msg);
            boardcastEvent.Fire();
        }
        public MsgScrollBar GetMessage()
        {
            var fisrt = m_counterMsgs.First;
            if (fisrt != null)
            {
                m_counterMsgs.RemoveFirst();
                return fisrt.Value;
            }
            if (m_index >= m_loopMsgs.Count) m_index = 0;

            if (m_index < m_loopMsgs.Count)
            {
                return m_loopMsgs[m_index++];
            }
            return new MsgScrollBar();
        }
    }
}
