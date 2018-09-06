using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapPvpNoSessionNotifyImpl : IProtoImpl<GMapPvpNoSessionNotify>
	{
//generate code begin
		public int PId() { return PID.GMapPvpNoSessionNotify; }
		public GMapPvpNoSessionNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var frame = GuiManager.Instance.ShowFrame(typeof(LSharpScript.MessageBoxFrame).Name);
            var script = LSharpScript.T.As<LSharpScript.MessageBoxFrame>(frame);
            script.SetDesc("game is over!", LSharpScript.MsgBoxType.Mbt_Ok);
            script.SetDelegater(() =>
            {
                var pvp = LSharpScript.NetworkWatcher.Inst().pvp;
                if (pvp != null) { pvp.OnNosession(); }
            });
        }
	}
}
