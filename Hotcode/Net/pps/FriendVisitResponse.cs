using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FriendVisitResponseImpl : IProtoImpl<FriendVisitResponse>
	{
//generate code begin
		public int PId() { return PID.FriendVisitResponse; }
		public FriendVisitResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                var rpc = HttpNetwork.Inst().Rpc as FriendVisitRequest;
                FriendModule.Instance.VisitSync(rpc.id, proto.infos.ConvertAll(it => FriendPetExplorer.FromProto(it)));
                MainState.Instance.PushState<SubFriendPetState>();
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
