using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class TitleNotifyImpl : IProtoImpl<TitleNotify>
	{
//generate code begin
		public int PId() { return PID.TitleNotify; }
		public TitleNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            //TitleModule.Inst().Sync(proto.id, proto.lv, proto.exp);
            //var buffs = proto.buffs.ConvertAll(it => new Title() { id = it.id, lv = 0, endAtMs = it.endAtMs, isNew = false });
            //switch (proto.act)
            //{
            //    case NotifyAction.Sy: TitleModule.Inst().Sync(buffs); break;
            //    case NotifyAction.Up: TitleModule.Inst().Update(buffs); break;
            //}
        }
        //Title Convert(cfg.title titleCfg)
        //{
        //    var title = proto.titles.Find(it => it.id == titleCfg.id);
        //    if (title != null) { return Convert(title); }
        //    return new Title() { id = titleCfg.id, lv = 0, locked = true };
        //}
        //Title Convert(TitleNotify.Title title)
        //{
        //    var titleCfg = tab.title.Inst().Find(title.id);
        //    return new Title() { id = title.id, lv = title.lv, locked = false };
        //}
    }
}
