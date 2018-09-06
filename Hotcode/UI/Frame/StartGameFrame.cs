using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class StartGameFrame : ILSharpScript
    {
//generate code begin
        public RectTransform hasAccount;
        public Button hasAccount_enter;
        public Button hasAccount_forget;
        public Text hasAccount_forget_name;
        public RectTransform noAccount;
        public Button noAccount_login;
        public Button noAccount_guest;
        void __LoadComponet(Transform transform)
        {
            hasAccount = transform.Find("@hasAccount").GetComponent<RectTransform>();
            hasAccount_enter = transform.Find("@hasAccount/@enter").GetComponent<Button>();
            hasAccount_forget = transform.Find("@hasAccount/@forget").GetComponent<Button>();
            hasAccount_forget_name = transform.Find("@hasAccount/@forget/@name").GetComponent<Text>();
            noAccount = transform.Find("@noAccount").GetComponent<RectTransform>();
            noAccount_login = transform.Find("@noAccount/@login").GetComponent<Button>();
            noAccount_guest = transform.Find("@noAccount/@guest").GetComponent<Button>();
        }
        void __DoInit()
        {
        }
        void __DoUninit()
        {
        }
        void __DoShow()
        {
        }
        void __DoHide()
        {
        }
//generate code end
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);

            hasAccount_enter.onClick.AddListener(() =>
            {
                var account = geniusbaby.archive.SQLiteTableManager.account.Get(0);
                var login = new AccountLoginRequest();
                login.account = new ProtoAccount()
                {
                    channel = account.channel,
                    accountId = account.account,
                    password = account.password,
                };
                HttpNetwork.Instance.Communicate(login);
            });
            hasAccount_forget.onClick.AddListener(() =>
            {
                LoginModule.Inst().Forget();
                geniusbaby.archive.SQLiteTableManager.account.Remove(0);
            });
            noAccount_guest.onClick.AddListener(() =>
            {
                var login = new AccountLoginRequest();
                login.account = new ProtoAccount()
                {
                    channel = 0,
                    accountId = SystemInfo.deviceUniqueIdentifier,
                    password = string.Empty,
                };
                HttpNetwork.Instance.Communicate(login);
            });
            noAccount_login.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                GuiManager.Instance.ShowFrame((string)typeof(ManualLoginFrame).Name);
            }));
        }
        public override void OnShow()
        {
            base.OnShow();
            LoginModule.Inst().onAccount.Add(OnAccount);
            OnAccount();
            //CameraControl.Inst().SetAsFirstCamera(false);
        }
        public override void OnHide()
        {
            //CameraControl.Inst().SetAsFirstCamera(true);
            LoginModule.Inst().onAccount.Add(OnAccount);
            base.OnHide();
        }
        void OnAccount()
        {
            var account = LoginModule.Inst().account;
            if (account != null)
            {
                if (account.channel == 0)
                {
                    hasAccount_forget_name.text = "ÓÎ¿ÍµÇÂ¼";
                }
                else
                {
                    hasAccount_forget_name.text = account.account;
                }
            }
            hasAccount.gameObject.SetActive(account != null);
            noAccount.gameObject.SetActive(account == null);
        }
    }
}
