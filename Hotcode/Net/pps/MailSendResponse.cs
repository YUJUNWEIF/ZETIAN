using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class MailSendResponseImpl : IProtoImpl<MailSendResponse>
	{
//generate code begin
		public int PId() { return PID.MailSendResponse; }
		public MailSendResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
