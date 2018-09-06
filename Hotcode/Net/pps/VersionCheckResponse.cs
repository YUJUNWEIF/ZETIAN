using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class VersionCheckResponseImpl : IProtoImpl<VersionCheckResponse>
	{
//generate code begin
		public int PId() { return PID.VersionCheckResponse; }
		public VersionCheckResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret != 0)
            {
                if (proto.cfgNeedUp)
                {
                    var frame = GuiManager.Instance.ShowFrame(typeof(LSharpScript.MessageBoxFrame).Name);
                    var script = LSharpScript.T.As<LSharpScript.MessageBoxFrame>(frame);
                    script.SetDesc("quit game", LSharpScript.MsgBoxType.Mbt_Ok);
                    script.SetDelegater(() =>
                    {
                        UnityEngine.Application.Quit();
                    });
                }
            }
        }
	}
}
