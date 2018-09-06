using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class AccountRegisterResponseImpl : IProtoImpl<AccountRegisterResponse>
	{
//generate code begin
		public int PId() { return PID.AccountRegisterResponse; }
		public AccountRegisterResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                var rpc = HttpNetwork.Inst().Rpc as AccountRegisterRequest;
                LoginModule.Instance.Login(new archive.Account() { channel = rpc.account.channel, account = rpc.account.accountId, password = rpc.account.password });
                GuiManager.Instance.HideAll();
                GuiManager.Instance.ShowFrame(typeof(LSharpScript.CreatePlayerFrame).Name);
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
