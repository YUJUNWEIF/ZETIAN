using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class PetEggHatchResponseImpl : IProtoImpl<PetEggHatchResponse>
	{
//generate code begin
		public int PId() { return PID.PetEggHatchResponse; }
		public PetEggHatchResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                //var script = GuiManager.Inst().ShowFrame<ui.PetDetailFrame>();
                //script.Display(Pet.NewPet(proto.pet.));
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
