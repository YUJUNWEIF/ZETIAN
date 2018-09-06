using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class WordQueryResponseImpl : IProtoImpl<WordQueryResponse>
	{
//generate code begin
		public int PId() { return PID.WordQueryResponse; }
		public WordQueryResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            EnglishDictModule.Inst().Sychronize(proto);
        }
	}
}
