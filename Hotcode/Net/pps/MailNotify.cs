using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class MailNotifyImpl : IProtoImpl<MailNotify>
	{
//generate code begin
		public int PId() { return PID.MailNotify; }
		public MailNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var mails = new List<Mail>();
            for (int index = 0; index < proto.mails.Count; ++index)
            {
                var mail = new Mail()
                {
                    id = proto.mails[index].id,
                    senderId = proto.mails[index].senderId,
                    titleOrName = proto.mails[index].titleOrName,
                    content = proto.mails[index].content,
                    createUtc = proto.mails[index].createAt,
                    endUtc = proto.mails[index].createAt,
                    hasRead = proto.mails[index].hasRead,
                    dropItems = new List<Item>(),
                };
                proto.mails[index].drops.ForEach(it => mail.dropItems.Add(new Item(it.id) { count = it.count }));
                mails.Add(mail);
            }
            switch (proto.act)
            {
                case NotifyAction.Sy: MailModule.Inst().Sync(mails); break;
                case NotifyAction.Up: MailModule.Inst().Update(mails); break;
            }
        }
	}
}
