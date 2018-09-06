using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ItemUseResponseImpl : IProtoImpl<ItemUseResponse>
	{
//generate code begin
		public int PId() { return PID.ItemUseResponse; }
		public ItemUseResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                if (proto.drops.Count > 0)
                {
                    var frame = GuiManager.Instance.ShowFrame(typeof(LSharpScript.DropFrame).Name);
                    var script = LSharpScript.T.As<LSharpScript.DropFrame>(frame);
                    script.Display(proto.drops.ConvertAll(it => new Item(it.id) { count = it.count }));
                }
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
