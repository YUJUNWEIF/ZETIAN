using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class TokenNotifyImpl : IProtoImpl<TokenNotify>
	{
//generate code begin
		public int PId() { return PID.TokenNotify; }
		public TokenNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var tokens = proto.tokens.ConvertAll(it => new TokenModule.Token() { id = it.id, value = it.count });
            switch (proto.act)
            {
                case NotifyAction.Sy: TokenModule.Inst().Sync(tokens); break;
                case NotifyAction.Up: TokenModule.Inst().Update(tokens); break;
            }
        }
	}
}
