using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class PetMergeResponseImpl : IProtoImpl<PetMergeResponse>
	{
//generate code begin
		public int PId() { return PID.PetMergeResponse; }
		public PetMergeResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                //var script = GuiManager.Inst().GetCachedFrame<ui.PetMergeFrame>();
                //script.DisplayResult(Pet.NewPet(proto.pet.uniqueId, proto.pet.mId, proto.pet.randSeed, proto.pet.lv));
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
