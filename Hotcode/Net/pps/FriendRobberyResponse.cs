using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FriendRobberyResponseImpl : IProtoImpl<FriendRobberyResponse>
	{
//generate code begin
		public int PId() { return PID.FriendRobberyResponse; }
		public FriendRobberyResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                var frame = GuiManager.Instance.ShowFrame(typeof(LSharpScript.DropFrame).Name);
                var script = LSharpScript.T.As<LSharpScript.DropFrame>(frame);
                script.Display(proto.items.ConvertAll(it => new Item(it.id) { count = it.count }));
                var rpc = HttpNetwork.Inst().Rpc as FriendRobberyRequest;
                if (rpc != null)
                {
                    FriendModule.Inst().VisitRobbery(rpc.id, rpc.slotId);
                }
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
