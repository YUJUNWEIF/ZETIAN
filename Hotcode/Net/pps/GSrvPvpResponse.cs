using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GSrvPvpResponseImpl : IProtoImpl<GSrvPvpResponse>
	{
//generate code begin
		public int PId() { return PID.GSrvPvpResponse; }
		public GSrvPvpResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                //var rpc = HttpNetwork.Inst().Rpc as GSrvPvpRequest;
                //GuiManager.Instance.ShowFrame(typeof(LSharpScript.PvpMatchFrame).Name);
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
