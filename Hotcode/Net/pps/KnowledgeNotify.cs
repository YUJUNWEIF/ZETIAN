using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class KnowledgeNotifyImpl : IProtoImpl<KnowledgeNotify>
	{
//generate code begin
		public int PId() { return PID.KnowledgeNotify; }
		public KnowledgeNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            switch (proto.act)
            {
                case NotifyAction.Sy:
                    var freqs = proto.freqs.ConvertAll(it => new KnowledgeFreq() { lgapId = it.lgapId, name = it.name });
                    KnowledgeModule.Inst().Sync(proto.lgapId, proto.classId, freqs); break;
                case NotifyAction.Up: KnowledgeModule.Inst().Update(proto.lgapId, proto.classId); break;
            }
        }
	}
}
