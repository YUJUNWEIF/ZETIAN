using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ManualLoginFrame : ILSharpScript
    {
//generate code begin
        public RectTransform manualLogin;
        public InputField manualLogin_account;
        public InputField manualLogin_pwd;
        public Button manualLogin_newAccount;
        public Button manualLogin_login;
        public Image newAccount;
        public InputField newAccount_account;
        public InputField newAccount_pwd;
        public InputField newAccount_pwdComfirm;
        public Button newAccount_regist;
        public Button close;
        void __LoadComponet(Transform transform)
        {
            manualLogin = transform.Find("@manualLogin").GetComponent<RectTransform>();
            manualLogin_account = transform.Find("@manualLogin/@account").GetComponent<InputField>();
            manualLogin_pwd = transform.Find("@manualLogin/@pwd").GetComponent<InputField>();
            manualLogin_newAccount = transform.Find("@manualLogin/@newAccount").GetComponent<Button>();
            manualLogin_login = transform.Find("@manualLogin/@login").GetComponent<Button>();
            newAccount = transform.Find("@newAccount").GetComponent<Image>();
            newAccount_account = transform.Find("@newAccount/@account").GetComponent<InputField>();
            newAccount_pwd = transform.Find("@newAccount/@pwd").GetComponent<InputField>();
            newAccount_pwdComfirm = transform.Find("@newAccount/@pwdComfirm").GetComponent<InputField>();
            newAccount_regist = transform.Find("@newAccount/@regist").GetComponent<Button>();
            close = transform.Find("@close").GetComponent<Button>();
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
        public static string decryptPwd;
        int m_channel = 1;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            manualLogin_newAccount.onClick.AddListener(() =>
            {
                Util.UnityHelper.Hide(manualLogin);
                Util.UnityHelper.Show(newAccount);
            });
            manualLogin_login.onClick.AddListener(() =>
            {
                GuiManager.Instance.HideFrame(api.name);

                if (!string.IsNullOrEmpty(manualLogin_account.text) &&
                    !string.IsNullOrEmpty(manualLogin_pwd.text))
                {
                    var login = new AccountLoginRequest();
                    login.account = new ProtoAccount()
                    {
                        accountId = manualLogin_account.text,
                        password = Util.Crypt.GetMd5_16(manualLogin_pwd.text),
                        channel = 1,
                    };
                    decryptPwd = manualLogin_pwd.text;
                    HttpNetwork.Instance.Communicate(login);
                }
            });
            newAccount_regist.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(newAccount_account.text) &&
                !string.IsNullOrEmpty(newAccount_pwd.text) &&
                !string.IsNullOrEmpty(newAccount_pwdComfirm.text) &&
                newAccount_pwd.text == newAccount_pwdComfirm.text)
                {
                    var reg = new AccountRegisterRequest();
                    reg.account = new ProtoAccount()
                    {
                        channel = m_channel,
                        accountId = newAccount_account.text,
                        password = Util.Crypt.GetMd5_16(newAccount_pwd.text),
                    };
                    HttpNetwork.Instance.Communicate(reg);
                }
            });
            close.onClick.AddListener(() => GuiManager.Instance.HideFrame(api.name));
        }
        public override void OnShow()
        {
            base.OnShow();
            Util.UnityHelper.Show(manualLogin);
            Util.UnityHelper.Hide(newAccount);

            manualLogin_account.text = string.Empty;
            manualLogin_pwd.text = string.Empty;
            newAccount_account.text = string.Empty;
            newAccount_pwd.text = string.Empty;
            newAccount_pwdComfirm.text = string.Empty;
        }
        public void Display(int channel)
        {
            m_channel = channel;
        }
    }
}
