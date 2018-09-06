using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace geniusbaby.LSharpScript
{
    public class VersionCheckFrame : IDownloader
    {        
        //class SystemNoticeLoaded : Kogarasi.WebView.IWebViewCallback
        //{
        //    public void onLoadStart(string url)
        //    {
        //        Debug.Log("call onLoadStart : " + url);
        //    }
        //    public void onLoadFinish(string url)
        //    {
        //        Debug.Log("call onLoadFinish : " + url);
        //    }
        //    public void onLoadFail(string url)
        //    {
        //        Debug.Log("call onLoadFail : " + url);
        //    }
        //}
//generate code begin
        public Transform Notice_leftTop;
        public Transform Notice_rightBottom;
        public RectTransform down;
        public Text down_version;
        public GuiBar down_progressBar;
        public Text down_progressBar_value;
        public Text down_wholeSize;
        public Button close;
        void __LoadComponet(Transform transform)
        {
            Notice_leftTop = transform.Find("Notice/@leftTop").GetComponent<Transform>();
            Notice_rightBottom = transform.Find("Notice/@rightBottom").GetComponent<Transform>();
            down = transform.Find("@down").GetComponent<RectTransform>();
            down_version = transform.Find("@down/@version").GetComponent<Text>();
            down_progressBar = transform.Find("@down/@progressBar").GetComponent<GuiBar>();
            down_progressBar_value = transform.Find("@down/@progressBar/@value").GetComponent<Text>();
            down_wholeSize = transform.Find("@down/@wholeSize").GetComponent<Text>();
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
        //SystemNoticeLoaded m_callback;
        Util.Coroutine m_coroutine;
        bool m_canHide = false;
        AssetMetaManifest cfgManifest;
        AssetMetaManifest artManifest;
        public override GuiBar progressBar { get { return down_progressBar; } }
        public override Text progressValue { get { return down_progressBar_value; } }
        public override Text wholeSize { get { return down_wholeSize; } }

        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);

            //m_callback = new SystemNoticeLoaded();
            //WebViewBehavior.Instance.setCallback(m_callback);
            //Vector3 tl = Camera.main.WorldToScreenPoint(leftTop.position);
            //Vector3 br = Camera.main.WorldToScreenPoint(rightBottom.position);
            //var min = Vector3.Min(tl, br);
            //var max = Vector3.Max(tl, br);
            //WebViewBehavior.Instance.SetMargins((int)min.x, Screen.height - (int)max.y, Screen.width - (int)max.x, (int)min.y);

            close.onClick.AddListener(() =>
            {
                //显示我们定义的预制体到用户窗体  
                //按钮点击关闭 GameObjectFrame
                // GuiManager.Inst().ShowFrame(typeof(geniusbaby.LSharpScript.GameObjectFrame).Name);
                //按钮点击 VIP9Frame
                //GuiManager.Inst().ShowFrame(typeof(geniusbaby.LSharpScript.VIP9Frame).Name);
                //图片旋转 Vip12Frame
                // GuiManager.Inst().ShowFrame(typeof(geniusbaby.LSharpScript.Vip12Frame).Name);
                //   GuiManager.Inst().ShowFrame(typeof(geniusbaby.LSharpScript.ExperienceFrame).Name);

               // GuiManager.Inst().ShowFrame(typeof(geniusbaby.LSharpScript.TestFrame).Name);

                GuiManager.Instance.HideFrame(api.name);
                HotcodeEntry.EnterGame();
                StateManager.Instance.ChangeState<LoginState>(); 
            });

            cfgManifest = new AssetMetaManifest(GamePath.manifest.cfg);
            artManifest = new AssetMetaManifest(GamePath.manifest.art);
            cfgManifest.UnMarsh(tab.CGReader.LoadFile(cfgManifest.meteInfoFile), 0);
            artManifest.UnMarsh(tab.CGReader.LoadFile(artManifest.meteInfoFile), 0);


            //HttpNetwork.Inst().Initialize();
        }
        public override void OnShow()
        {
            base.OnShow();
            DownloadManager.Instance.onDownload.Add(OnDownloadProgress);
            DownloadManager.Instance.onCode.Add(OnCode);
            //WebViewBehavior.Instance.SetVisibility(true);
            OnRetry();
            //coroutineHelper.StartCoroutine(GuiManager.Instance.Preload(), () => m_canHide = true);
        }
        public override void OnHide()
        {
            //WebViewBehavior.Instance.SetVisibility(false);
            DownloadManager.Instance.onCode.Rmv(OnCode);
            DownloadManager.Instance.onDownload.Rmv(OnDownloadProgress);
            base.OnHide();
        }
        protected override void OnRetry()
        {
            if (m_coroutine != null) { m_coroutine.Kill(); }

            //WebViewBehavior.Instance.SetVisibility(true);
            //WebViewBehavior.Instance.LoadURL(GamePath.url.noticeURL);

            Util.UnityHelper.Show(down);
            Util.UnityHelper.Hide(close);
            m_coroutine = api.coroutineHelper.StartCoroutine(
                DownloadManager.Instance.StartUp(cfgManifest, artManifest),
                () =>
                {
                    if (m_coroutine.killed || DownloadManager.Instance.code == DownloadManager.Code.Normal)
                    {
                        Framework.Instance.AfterInit();
                        Util.UnityHelper.Hide(down);
                        Util.UnityHelper.Show(close);
                    }
                    else
                    {
                        OnCode();
                    }
                    m_coroutine = null;
                });
            //m_coroutine.Kill();
        }
        //void Update()
        //{
        //    if (DownloadManager.Instance.state == DownloadManager.State.Downloading)
        //    {
        //        var progresss = DownloadManager.Instance.progress;
        //        wholeSizeLabel.text = FileSizeToString(progresss.max);
        //        progressBar.value = progresss.current * 1f / progresss.max;
        //        progressValue.text = ((int)(progressBar.value * 100)).ToString() + @"%";
        //    }
        //}
    }
}
