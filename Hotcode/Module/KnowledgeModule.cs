using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using geniusbaby;
using geniusbaby.archive;
using geniusbaby.ui;

namespace geniusbaby
{
    public struct KnowledgeFreq
    {
        public int lgapId;
        public string name;
        public long time;
    }
    public class KnowledgeModule : Singleton<KnowledgeModule>, IModule
    {
        LGAP m_view;
        public geniusbaby.archive.Knowledge knowledge { get; private set; } 
        public int lgapId { get; private set; }
        public int classId { get; private set; }
        public List<KnowledgeFreq> freqs = new List<KnowledgeFreq>();
        public int learnedWord { get; private set; }
        public LGAP preview { get { return m_view; } }
        public Util.ParamActions onKnowledgeSync = new Util.ParamActions();
        public Util.ParamActions onSync = new Util.ParamActions();
        public Util.ParamActions onFrequentSync = new Util.ParamActions();


        public Util.ParamActions onSwitchLang = new Util.ParamActions();
        public Util.ParamActions onSwitchGrade = new Util.ParamActions();
        public Util.ParamActions onSwitchArea = new Util.ParamActions();
        public Util.ParamActions onPackageSync = new Util.ParamActions();

        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { Load(); }
        public void OnMainExit() { Save(); }
        public void Load()
        {
            knowledge = SQLiteTableManager.knowledgeFreq.Get(PlayerModule.MyId());
            if (knowledge == null) { knowledge = new Knowledge() { playerId = PlayerModule.MyId() }; }
        }
        public void Save()
        {
            if (knowledge != null)
            {
                SQLiteTableManager.knowledgeFreq.Update(PlayerModule.MyId(), knowledge);
            }
        }
        public void Sync(int lgapId, int classId, List<KnowledgeFreq> freqs)
        {
            Update(lgapId, classId);
            this.freqs = freqs;
            onFrequentSync.Fire();
        }
        public void Update(int lgapId, int classId)
        {
            this.lgapId = lgapId;
            this.classId = classId;
            onKnowledgeSync.Fire();
            onSync.Fire();
        }
        public void ResetPreview()
        {
            this.m_view = LGAP.Decode(lgapId);
        }
        public void SwitchViewLang(int langId)
        {
            if (m_view.langId != langId)
            {
                m_view.langId = (byte)langId;
                LGAPManager.Inst().DownloadGAPInfo(LGAPManager.Inst().localLGAPInfo.meteInfoFile);
                onSwitchLang.Fire();
            }
        }
        public void SwitchViewGrade(int gradeId)
        {
            if (m_view.gradeId != gradeId)
            {
                m_view.gradeId = (byte)gradeId;
                onSwitchGrade.Fire();
            }
        }
        public void SwitchViewArea(int areaId)
        {
            if (m_view.areaId != areaId)
            {
                m_view.areaId = (byte)areaId;
                onSwitchArea.Fire();
            }
        }
        public void Refresh(IList<Statistic.WordReply> replies)
        {
            if (knowledge == null) { return; }

            for (int index = 0; index < replies.Count; ++index)
            {
                var it = replies[index];
                if (it.wrongCount <= 0) { continue; }
                var statistic = new Knowledge.Wrong() { english = it.english, chinese = it.chinese, wrongs = it.wrongCount };
                var exist = knowledge.wrongs.FindIndex(wrong => it.english == wrong.english);
                if (exist >= 0)
                {
                    var wrong = knowledge.wrongs[exist];
                    knowledge.wrongs.RemoveAt(exist);
                    statistic.wrongs += wrong.wrongs;
                }
                knowledge.wrongs.Add(statistic);
            }
            if (knowledge.wrongs.Count > 50)
            {
                knowledge.wrongs.RemoveRange(0, knowledge.wrongs.Count - 50);
            }
            Save();
        }

        public List<geniusbaby.cfg.wordClass> FindLGAP() { return LGAPManager.Inst().FindLGAP(lgapId); }
        public geniusbaby.cfg.wordClass FindClass(int classId) { return LGAPManager.Inst().FindClass(lgapId, classId); }
    }
}

