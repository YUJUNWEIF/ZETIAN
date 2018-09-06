using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Util;
using System;
using System.IO;

namespace geniusbaby.LSharpScript
{
    public abstract class IDownloader : ILSharpScript
    {
        public abstract GuiBar progressBar { get; }
        public abstract Text progressValue { get; }
        public abstract Text wholeSize { get; }

        protected abstract void OnRetry();

        public override void OnShow()
        {
            base.OnShow();
            DownloadManager.Instance.onDownload.Add(OnDownloadProgress);
            DownloadManager.Instance.onCode.Add(OnCode);
        }
        public override void OnHide()
        {
            DownloadManager.Instance.onDownload.Rmv(OnDownloadProgress);
            DownloadManager.Instance.onCode.Rmv(OnCode);
            base.OnHide();
        }
        protected void Update()
        {
            var progress = DownloadManager.Inst().progress;
            progressBar.value = progress.current * 1f / progress.max;
        }
        protected void OnDownloadProgress(AssetMetaInfo assetInfo)
        {
            progressBar.value = 0;
        }
        protected string FileSizeToString(int size)
        {
            if (size < 1000) { return size.ToString() + @"B"; }
            if (size < 1000 * 1000) { return string.Format("{0:#.0%}K", size * 0.001f); }
            return string.Format("{0:#.0%}M", (size / 1000) * 0.001f);
        }
        protected void OnCode()
        {
            switch (DownloadManager.Inst().code)
            {
                case DownloadManager.Code.Normal: break;
                case DownloadManager.Code.ReadError:
                    {
                        //var script = GuiManager.Instance.ShowLSharpFrame(@"MessageBoxFrame");
                        //script.SetDelegater(OnRetry);             
                        //script.SetDesc(@"更新失败，请确保网络畅通请下载最新客户端");
                    }
                    break;
                case DownloadManager.Code.CheckSumError:
                    {
                        //var script = GuiManager.Instance.ShowFrame<MessageBoxFrame>();
                        //script.SetDelegater(OnRetry);
                        //script.SetDesc(@"更新失败，下载数据出现错误，请重新下载");
                    }
                    break;
                case DownloadManager.Code.WriteError:
                    {
                        //var script = GuiManager.Instance.ShowFrame<MessageBoxFrame>();
                        //script.SetDelegater(() => Application.Quit());
                        //script.SetDesc(@"更新失败，请确保剩余足够的磁盘空间");
                    }
                    break;
                case DownloadManager.Code.InvalidClient:
                    {
                        //var script = GuiManager.Instance.ShowFrame<MessageBoxFrame>();
                        //script.SetDelegater(() => Application.Quit());
                        //script.SetDesc(@"更新失败，请下载最新客户端");
                    }
                    break;
                default: break;
            }
        }
    }
}
