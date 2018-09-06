using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class MsgBroadcastPanel : ILSharpScript
    {
//generate code begin
        public Text clip_msg;
        void __LoadComponet(Transform transform)
        {
            clip_msg = transform.Find("clip/@msg").GetComponent<Text>();
        }
        void __DoInit()
        {
        }
        void __DoUninit()
        {
        }
        void __DoShow()
        {
        }
        void __DoHide()
        {
        }
//generate code end
        public float speed;
        public string msg;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        MsgScrollBar m_msgBoardcast;
        Util.Coroutine m_coroutine;
        RectTransform m_cachedClipRc;
        RectTransform m_cachedContentRc;
        public override void OnShow()
        {
            base.OnShow();
            m_cachedClipRc = api.GetComponent<RectTransform>();
            m_cachedContentRc = clip_msg.GetComponent<RectTransform>();

            MsgScrollBarModule.Instance.boardcastEvent.Add(OnBoardcastEvent);
            if (m_coroutine != null) { m_coroutine.pause = false; }
            else
            {
                OnBoardcastEvent();
            }
        }
        public override void OnHide()
        {
            if (m_coroutine != null) { m_coroutine.pause = true; }
            MsgScrollBarModule.Instance.boardcastEvent.Rmv(OnBoardcastEvent);
            base.OnHide();
        }
        IEnumerator RollLeft()
        {
            while (true)
            {
                var rc = m_cachedContentRc.rect;
                if (rc.width != clip_msg.preferredWidth)
                {
                    var sizeDelta = new Vector2(rc.width, rc.height);
                    sizeDelta.x = clip_msg.preferredWidth;
                    m_cachedContentRc.sizeDelta = sizeDelta;
                }
                var loc = m_cachedContentRc.anchoredPosition;
                loc.x -= speed * Time.deltaTime;
                m_cachedContentRc.anchoredPosition = loc;
                if (loc.x <= -clip_msg.preferredWidth)
                {
                    loc.x = m_cachedClipRc.rect.width;
                    m_cachedContentRc.anchoredPosition = loc;
                    yield return WaitSeconds.Delay(1f);
                    break;
                }
                yield return null;
            }
            msg = null;
            m_coroutine = null;
            OnBoardcastEvent();
        }
        void OnBoardcastEvent()
        {
            if (msg == null)
            {
                if (m_msgBoardcast.msg == null || m_msgBoardcast.count <= 0)
                {
                    m_msgBoardcast = MsgScrollBarModule.Instance.GetMessage();
                }
                if (m_msgBoardcast.msg != null)
                {
                    --m_msgBoardcast.count;
                    msg = m_msgBoardcast.msg;
                    clip_msg.text = msg;

                    var loc = m_cachedContentRc.anchoredPosition;
                    loc.x = m_cachedClipRc.rect.width;
                    m_cachedContentRc.anchoredPosition = loc;

                    m_coroutine = api.coroutineHelper.StartCoroutine(RollLeft());
                }
            }
        }
    }
}
