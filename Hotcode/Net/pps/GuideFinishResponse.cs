using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GuideFinishResponseImpl : IProtoImpl<GuideFinishResponse>
	{
//generate code begin
		public int PId() { return PID.GuideFinishResponse; }
		public GuideFinishResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                var frame = GuiManager.Instance.ShowFrame(typeof(LSharpScript.DropFrame).Name);
                var script = LSharpScript.T.As<LSharpScript.DropFrame>(frame);
                script.Display(proto.drops.ConvertAll(it => new Item(it.id) { count = it.count }), ()=>
                {
                    GuideModule.Inst().AutoAccept();
                    //ui.GuideMaskFrame.RestoreGuide(GuideModule.Inst().guide);
                });
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
