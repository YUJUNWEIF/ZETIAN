using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class PetNotifyImpl : IProtoImpl<PetNotify>
	{
//generate code begin
		public int PId() { return PID.PetNotify; }
		public PetNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var pets = proto.pets.ConvertAll(it => new PetBase() { uniqueId = it.uniqueId, mId = it.mId, randSeed = it.randSeed, lv = it.lv });
            switch (proto.act)
            {
                case NotifyAction.Sy: PetModule.Inst().PetSync(proto.equipId, pets); break;
                case NotifyAction.Up: PetModule.Inst().PetUpdate(proto.equipId, pets); break;
            }
        }
	}
}
