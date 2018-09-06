using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class WordAreaItemPanel : ILSharpScript
    {
//generate code begin
        public Text name;
        public Button getUp;
        public Image getUp_getRoot;
        public Image getUp_upRoot;
        public Text getUp_size;
        public Text getUpRoot;
        public Image icon;
        public Image icon_usingRoot;
        void __LoadComponet(Transform transform)
        {
            name = transform.Find("@name").GetComponent<Text>();
            getUp = transform.Find("@getUp").GetComponent<Button>();
            getUp_getRoot = transform.Find("@getUp/@getRoot").GetComponent<Image>();
            getUp_upRoot = transform.Find("@getUp/@upRoot").GetComponent<Image>();
            getUp_size = transform.Find("@getUp/@size").GetComponent<Text>();
            getUpRoot = transform.Find("@getUpRoot").GetComponent<Text>();
            icon = transform.Find("@icon").GetComponent<Image>();
            icon_usingRoot = transform.Find("@icon/@usingRoot").GetComponent<Image>();
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
        enum ZipStatus
        {
            Normal = 0,
            NeedGet = 1,
            NeedUp = 2,
        }
        GAPInfo m_remoteInfos;
        GAPInfo._areaInfo m_remoteInfo;
        AssetMetaInfo m_localInfo;
        LGAP lgap;
        ZipStatus m_zipStatus;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                var com = api.GetComponent<LSharpItemPanel>();
                com.ListComponent.SelectIndex(com.index);
            });
            getUp.onClick.AddListener(() =>
            {
                LGAPManager.Inst().ParseAfterDownload(lgap);
            });
        }
        public void SetValue(GAPInfo value)
        {
            var com = api.GetComponent<LSharpItemPanel>();

            m_remoteInfos = value;
            m_remoteInfo = m_remoteInfos.areaInfos[com.index];

            lgap = new LGAP() { langId = 1, gradeId = (byte)value.gradeId, areaId = (byte)m_remoteInfo.areaId };
            var localFile = lgap.MakeFileName();
            var localLGCPInfo = LGAPManager.Inst().localLGAPInfo;
            m_localInfo = localLGCPInfo.hashs.Find(it => it.name == localFile);

            lgap = KnowledgeModule.Inst().preview;
            lgap.gradeId = (byte)m_remoteInfos.gradeId;
            lgap.areaId = (byte)m_remoteInfo.areaId;

            name.text = m_remoteInfo.name;
            if (m_remoteInfo.size < 1000 * 1000)
            {
                getUp_size.text = string.Format("{0:#.0}K", m_remoteInfo.size * 0.001f);
            }
            else
            {
                getUp_size.text = string.Format("{0:#.0}M", (m_remoteInfo.size / 1000) * 0.001f);
            }

            if (m_localInfo != null)
            {
                m_zipStatus = (m_remoteInfo.md5 != m_localInfo.md5 || m_remoteInfo.size != m_localInfo.size) ? ZipStatus.NeedUp : ZipStatus.Normal;
            }
            else
            {
                m_zipStatus = ZipStatus.NeedGet;
            }

            getUp.gameObject.SetActive(m_zipStatus == ZipStatus.NeedGet || m_zipStatus == ZipStatus.NeedUp);
            getUp_getRoot.gameObject.SetActive(m_zipStatus == ZipStatus.NeedGet);
            getUp_upRoot.gameObject.SetActive(m_zipStatus == ZipStatus.NeedUp);

            if (m_zipStatus != ZipStatus.Normal)
            {
                var localUrl = Path.Combine(Application.persistentDataPath, localFile);
                File.Delete(localUrl);
            }
        }
        public GAPInfo GetValue()
        {
            return m_remoteInfos; 
        }
    }
}
