using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class DownloadFileFrame : IDownloader
    {
//generate code begin
        public GuiBar slider;
        public Text slider_progress;
        public Text slider_wholeSize;
        void __LoadComponet(Transform transform)
        {
            slider = transform.Find("@slider").GetComponent<GuiBar>();
            slider_progress = transform.Find("@slider/@progress").GetComponent<Text>();
            slider_wholeSize = transform.Find("@slider/@wholeSize").GetComponent<Text>();
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
        pps.ProtoAssetMetaInfo metaInfo;
        string localUrl;
        string remoteUrl;
        Action whenDone;
        Util.Coroutine m_coroutine;
        public override GuiBar progressBar { get { return slider; } }
        public override Text progressValue { get { return slider_progress; } }
        public override Text wholeSize { get { return slider_wholeSize; } }
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        public void Download(pps.ProtoAssetMetaInfo metaInfo, string localFile, Action whenDone = null)
        {
            this.metaInfo = metaInfo;
            this.localUrl = Path.Combine(Application.persistentDataPath, localFile);
            this.remoteUrl = GamePath.url.updateURL + metaInfo.file;
            this.whenDone = whenDone;
            OnRetry();
        }
        protected override void OnRetry()
        {
            if (m_coroutine != null) { m_coroutine.Kill(); }
            api.coroutineHelper.StartCoroutine(
               DownloadManager.Inst().DownloadFromUrl(localUrl, remoteUrl, metaInfo.md5), () =>
               {
                   if (DownloadManager.Inst().code == DownloadManager.Code.Normal)
                   {
                       GuiManager.Inst().HideFrame(api.name);
                       whenDone();
                   }
                   else
                   {
                       OnCode();
                   }
                   m_coroutine = null;
               });
        }
    }
}
