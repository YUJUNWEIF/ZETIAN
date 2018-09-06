using System;
using System.Collections.Generic;
using Net;
using System.IO;
using System.Text;
using UnityEngine;

namespace geniusbaby.pps
{
	public partial class KnowledgeSwitchResponseImpl : IProtoImpl<KnowledgeSwitchResponse>
	{
//generate code begin
		public int PId() { return PID.KnowledgeSwitchResponse; }
		public KnowledgeSwitchResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                //var rpc = HttpNetwork.Inst().Rpc as KnowledgeSwitchRequest;
                //if (rpc != null)
                //{
                //    var lgf = LGCP.Decode(rpc.lgcpId);
                //    var xx = LGCPManager.Inst().Find(lgf.gradeId);
                //    var jc = xx.cs.Find(it => it.categoryId == lgf.categoryId);
                //    var sb = new StringBuilder().Append(lgf.langId).Append('_').Append(lgf.gradeId).Append('_').Append(lgf.categoryId).Append(".zip");
                //    if (!File.Exists(sb.ToString()))
                //    {
                //        var script = GuiManager.Inst().ShowFrame<ui.DownloadFile>();
                //        script.Download(new ProtoAssetMetaInfo() { file = "knowledge/english/" + jc.file, md5 = jc.md5, size = jc.size }, sb.ToString());
                //    }
                //}
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
