using System;
using System.Collections.Generic;
using Net;
using geniusbaby.ui;

namespace geniusbaby.pps
{
	public partial class AccountLoginResponseImpl : IProtoImpl<AccountLoginResponse>
	{
//generate code begin
		public int PId() { return PID.AccountLoginResponse; }
		public AccountLoginResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                var rpc = HttpNetwork.Instance.Rpc as AccountLoginRequest;
                LoginModule.Instance.Login(new archive.Account() { channel = rpc.account.channel, account = rpc.account.accountId, password = rpc.account.password });
                if (proto.needPlayer)
                {
                    GuiManager.Instance.HideAll();
                    GuiManager.Instance.ShowFrame(typeof(LSharpScript.CreatePlayerFrame).Name);
                }
                else
                {
                    StateManager.Instance.ChangeState<MainState>();
                }
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
