using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FriendNotifyImpl : IProtoImpl<FriendNotify>
	{
//generate code begin
		public int PId() { return PID.FriendNotify; }
		public FriendNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var friends = new List<Friend>();
            for (int index = 0; index < proto.friends.Count; ++index)
            {
                var fri = proto.friends[index];
                friends.Add(new Friend() { type = Friend.Type.Ok, id = fri.id, name = fri.name, giveClock = fri.giveUtc });    
            }
            for (int index = 0; index < proto.appliers.Count; ++index)
            {
                var fri = proto.appliers[index];
                friends.Add(new Friend() { type = Friend.Type.NeedSelfConfirm, id = fri.id, name = fri.name });
            }
            switch (proto.act)
            {
                case NotifyAction.Sy: FriendModule.Inst().SychronizeFriends(friends); break;
                case NotifyAction.Up: FriendModule.Inst().Update(friends); break;
            }
        }
	}
}
