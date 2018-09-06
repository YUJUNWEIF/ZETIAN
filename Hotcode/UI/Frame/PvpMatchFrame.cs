using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PvpMatchFrame : ILSharpScript
    {
//generate code begin
        public Text BG_S_tickDown;
        public Text BG_S_matchTime;
        public LSharpListContainer BG_PvpMatch;
        public Button BG_cancel;
        void __LoadComponet(Transform transform)
        {
            BG_S_tickDown = transform.Find("BG/S/@tickDown").GetComponent<Text>();
            BG_S_matchTime = transform.Find("BG/S/@matchTime").GetComponent<Text>();
            BG_PvpMatch = transform.Find("BG/@PvpMatch").GetComponent<LSharpListContainer>();
            BG_cancel = transform.Find("BG/@cancel").GetComponent<Button>();
        }
        void __DoInit()
        {
            BG_PvpMatch.OnInitialize();
        }
        void __DoUninit()
        {
            BG_PvpMatch.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_PvpMatch.OnShow();
        }
        void __DoHide()
        {
            BG_PvpMatch.OnHide();
        }
//generate code end
        int m_tickDown;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            __DoInit();
            BG_cancel.onClick.AddListener(() =>
            {
                if (!PvpRoomModule.Inst().full)
                {
                    GuiManager.Inst().HideFrame(api.name);
                    TcpNetwork.Inst().Send(new GMapPvpQuitRequest());
                    NetworkWatcher.Inst().DelayStop(GamePath.net.keepAlive / 2);
                }
            });
        }
        public override void OnUnInitialize()
        {
            __DoUninit();
            base.OnUnInitialize();
        }
        public override void OnShow()
        {
            base.OnShow();
            __DoShow();
            PvpRoomModule.Inst().onSync.Add(OnSync);
            PvpRoomModule.Inst().onTickDownSync.Add(OnTickDownSync);
            PvpRoomModule.Inst().onJoin.Add(OnJoin);
            Util.TimerManager.Inst().Add(OnTimer, 1000);

            OnSync();
            OnTickDownSync();
        }
        public override void OnHide()
        {
            PvpRoomModule.Inst().onSync.Rmv(OnSync);
            PvpRoomModule.Inst().onTickDownSync.Rmv(OnTickDownSync);
            PvpRoomModule.Inst().onJoin.Rmv(OnJoin);
            Util.TimerManager.Inst().Remove(OnTimer);
            __DoHide();
            base.OnHide();
        }
        void OnSync()
        {
            if (PvpRoomModule.Inst().combatType != GlobalDefine.GTypeUnknown)
            {
                BG_PvpMatch.SetValues(T.L(PvpRoomModule.Inst().players));
            }
        }
        void OnJoin(GSrvPvpNotify proto) { }
        void OnTickDownSync()
        {
            m_tickDown = PvpRoomModule.Inst().tickDown;
            OnTimer();
        }
        void OnTimer()
        {
            if (m_tickDown >= 0)
            {
                BG_S_tickDown.text = (m_tickDown / 1000).ToString();
                m_tickDown -= 1000;
                if (m_tickDown < 0) { m_tickDown = 0; }
            }
            else
            {
                BG_S_tickDown.text = string.Empty;
            }
        }
    }
}
