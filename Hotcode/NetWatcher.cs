
using UnityEngine;
using System;
using System.Collections.Generic;
using Util;

namespace geniusbaby.LSharpScript
{
    public class NetworkWatcher : Singleton<NetworkWatcher>, IGameEvent
    {
        public void OnStartGame()
        {
            HttpNetwork.onCommunicateDoing.Add(OnCommuncationDoing);
            HttpNetwork.onCommunicateSucceed.Add(OnCommuncationSucceed);
            HttpNetwork.onCommunicateFailed.Add(OnCommuncationFailed);
            HttpNetwork.onSessionInvalid.Add(OnSessionInValid);
        }
        public void OnStopGame()
        {
            HttpNetwork.onCommunicateDoing.Rmv(OnCommuncationDoing);
            HttpNetwork.onCommunicateSucceed.Rmv(OnCommuncationSucceed);
            HttpNetwork.onCommunicateFailed.Rmv(OnCommuncationFailed);
            HttpNetwork.onSessionInvalid.Rmv(OnSessionInValid);
        }
        void OnCommuncationDoing()
        {
            GuiManager.Instance.ShowFrame(typeof(NetWatchFrame).Name);
        }
        void OnCommuncationSucceed()
        {
            GuiManager.Instance.HideFrame(typeof(NetWatchFrame).Name);
        }
        void OnCommuncationFailed(string error)
        {
            GuiManager.Instance.HideFrame(typeof(NetWatchFrame).Name);
            var frame = GuiManager.Instance.ShowFrame(typeof(MessageBoxFrame).Name);
            var script = T.As<MessageBoxFrame>(frame);
            script.SetDelegater(OnRetryCommunicate);
            script.SetDesc(error);
        }
        void OnSessionInValid()
        {
            var frame = GuiManager.Instance.ShowFrame(typeof(MessageBoxFrame).Name);
            var script = T.As<MessageBoxFrame>(frame);
            script.SetDelegater(OnRetryCommunicate);
            script.SetDesc("session invalid", MsgBoxType.Mbt_Ok);
        }
        void OnRetryCommunicate()
        {
            StateManager.Instance.ChangeState<LoginState>();
        }


        public IPvpManager pvp { get; private set; }
        public void Start(IPvpManager pvp, pps.SrvFight addr, pps.GMapPvpReport report)
        {
            this.pvp = pvp;
            pvp.PvpEnter(addr, report, NetworkNotReachable);
        }
        public void Stop()
        {
            if (pvp != null)
            {
                pvp.PvpLeave();
            }
        }
        public void DelayStop(int delaySec)
        {
            TcpNetwork.Instance.DelayStop(delaySec);
        }
        void NetworkNotReachable()
        {
            var frame = GuiManager.Inst().ShowFrame(typeof(MessageBoxFrame).Name);
            var script = T.As<MessageBoxFrame>(frame);
            script.SetDesc("No network", MsgBoxType.Mbt_Ok);
            script.SetDelegater(WhenNoNet);
        }
        public void Replaced()
        {
            Stop();
            var frame = GuiManager.Inst().ShowFrame(typeof(MessageBoxFrame).Name);
            var script = T.As<MessageBoxFrame>(frame);
            script.SetDesc("Be replaced by others");
            script.SetDelegater(WhenNoNet);
        }
        void WhenNoNet()
        {
            if (pvp != null)
            {
                pvp.PvpLeave();
            }
            StateManager.Instance.ReplaceAll<LoginState>();
        }
    }
}
