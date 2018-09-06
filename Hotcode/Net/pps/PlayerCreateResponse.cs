using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class PlayerCreateResponseImpl : IProtoImpl<PlayerCreateResponse>
	{
//generate code begin
		public int PId() { return PID.PlayerCreateResponse; }
		public PlayerCreateResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                GuideModule.Instance.Sync(0);
                StateManager.Instance.ChangeState<MainState>();
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
