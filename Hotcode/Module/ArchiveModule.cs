using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using geniusbaby;
using geniusbaby.archive;

namespace geniusbaby
{
    public class ArchiveModule : Singleton<ArchiveModule>, IModule
    {
        public byte[] buffer = new byte[1024 * 16];
        struct ArchiveMetaInfo
        {
            public long serverUtcMs;
            public long clientUtcMs;
            public bool needKeepAtClient;
            public bool needUpdate;
            public byte[] tempZipData;
        }
        List<Util.ISerializable> m_events = new List<Util.ISerializable>();
        ArchiveMetaInfo m_archiveMetaInfo;
        public ArchiveModule()
        {
            m_events.Add(AchieveModule.Instance);
        }
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter()
        {
            Framework.onSave.Add(CommitArchiveManual);
            var ar = SQLiteTableManager.archive.Get(PlayerModule.MyId()); ;
            if (ar != null) { m_archiveMetaInfo.tempZipData = ar.zipData; }
        }
        public void OnMainExit()
        {
            Framework.onSave.Rmv(CommitArchiveManual);
        }
        public void Sync(byte[] serverZipData)
        {
            byte[] serverUpzipData = null;
            byte[] clientUpzipData = null;
            byte[] clientZipData = null;
            m_archiveMetaInfo.needUpdate = false;
            m_archiveMetaInfo.needKeepAtClient = false;

            if (serverZipData != null && serverZipData.Length > 0)
            {
                try
                {
                    serverUpzipData = Util.DependencyUtil.zlibDecompress(serverZipData, 0, serverZipData.Length);
                    var ms = new MemoryStream(serverUpzipData);
                    Util.InStream os = new Util.InStream(ms);
                    m_archiveMetaInfo.serverUtcMs = os.ReadInt64();
                }
                catch { }
            }
            clientZipData = m_archiveMetaInfo.tempZipData;
            if (clientZipData!= null && clientZipData.Length > 0)
            {
                try
                {
                    clientUpzipData = Util.DependencyUtil.zlibDecompress(clientZipData, 0, clientZipData.Length);
                    var ms = new MemoryStream(clientUpzipData);
                    var os = new Util.InStream(ms);
                    m_archiveMetaInfo.clientUtcMs = os.ReadInt64();
                }
                catch { }
            }
            bool useSrv = (m_archiveMetaInfo.serverUtcMs > m_archiveMetaInfo.clientUtcMs);
            UnzipLoad(useSrv ? serverUpzipData : clientUpzipData, useSrv ? serverZipData : clientZipData);
        }
        void UnzipLoad(byte[] upzipData, byte[] zipData)
        {
            if (zipData == null || zipData.Length <= 0) { return; }
            try
            {
                var ms = new MemoryStream(upzipData);
                var os = new Util.InStream(ms);
                m_archiveMetaInfo.clientUtcMs = os.ReadInt64();
                m_archiveMetaInfo.tempZipData = zipData;
                for (int index = 0; index < m_events.Count; ++index)
                {
                    m_events[index].Unmarsh(os);
                }
            }
            catch (Exception ex)
            {
                Util.Logger.Instance.Error(ex.Message, ex);
            }
        }
        public void DeleteSave()
        {
            try
            {
                SQLiteTableManager.archive.Remove(PlayerModule.MyId());
            }
            catch (Exception ex)
            {
                Util.Logger.Instance.Error(ex.Message, ex);
            }
        }
        public void ForceKeepAtClient()
        {
            ZipSave();
            if (m_archiveMetaInfo.needKeepAtClient)
            {
                if (m_archiveMetaInfo.tempZipData != null)
                {
                    var ar = new Archive() { playerId = PlayerModule.MyId(), zipData = m_archiveMetaInfo.tempZipData };
                    SQLiteTableManager.archive.Update(ar.playerId, ar);
                    m_archiveMetaInfo.needKeepAtClient = false;
                }
            }
        }
        void ZipSave()
        {
            if (!m_archiveMetaInfo.needUpdate) return;
            
            try
            {
                m_archiveMetaInfo.needKeepAtClient = true;
                m_archiveMetaInfo.clientUtcMs = Util.TimerManager.Inst().RealTimeMS();

                var ms = new MemoryStream(buffer);
                var os = new Util.OutStream(ms);
                os.WriteInt64(m_archiveMetaInfo.clientUtcMs);
                for (int index = 0; index < m_events.Count; ++index)
                {
                    m_events[index].Marsh(os);
                }
                ms.Flush();

                m_archiveMetaInfo.tempZipData = Util.DependencyUtil.zlibCompress(buffer, 0, (int)ms.Position);
                m_archiveMetaInfo.needUpdate = false;
            }
            catch (Exception ex)
            {
                Util.Logger.Instance.Error(ex.Message, ex);
            }
        }
        public void SetNeedUpdateFlag()
        {
            m_archiveMetaInfo.needUpdate = true;
        }
        public void CommitArchiveManual()
        {
            ForceKeepAtClient();
            if (m_archiveMetaInfo.tempZipData == null) { return; }
            if (m_archiveMetaInfo.clientUtcMs > m_archiveMetaInfo.serverUtcMs)
            {
                m_archiveMetaInfo.serverUtcMs = m_archiveMetaInfo.clientUtcMs;
                HttpNetwork.Inst().Communicate(new geniusbaby.pps.ArchiveRequest() { utcMs = m_archiveMetaInfo.serverUtcMs, data = m_archiveMetaInfo.tempZipData });
            }
        }
    }
}