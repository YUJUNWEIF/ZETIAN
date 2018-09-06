using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapLinkInfoNotifyImpl : IProtoImpl<GMapLinkInfoNotify>
	{
//generate code begin
		public int PId() { return PID.GMapLinkInfoNotify; }
		public GMapLinkInfoNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            switch (proto.linkInfo)
            {
                case GMapLinkInfoNotify.LinkInfo.LinkBroken:
                    //script.SetDesc("player linked broken : " + proto.playerId);                    
                    break;
                case GMapLinkInfoNotify.LinkInfo.Reconnect:
                    //script.SetDesc("player reconnect : " + proto.playerId);
                    break;
                case GMapLinkInfoNotify.LinkInfo.Replace:
                    LSharpScript.NetworkWatcher.Inst().Replaced();
                    break;
            }
        }
	}
}
