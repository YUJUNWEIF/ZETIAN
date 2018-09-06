using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public struct Guide
    {
        public geniusbaby.cfg.guide guideCfg;
        public int stepId;
    }
    public class GuideModule : Singleton<GuideModule>, IModule
    {
        public enum Type
        {
            Strong,
            Auto,
        }
        Guide m_guide;
        public int finishGuideId { get; private set; }
        public Guide guide { get { return m_guide; } }
        public bool tutoring { get { return m_guide.guideCfg != null; } }
        public geniusbaby.pps.GSrvPvpNotify pvp { get; set; }
        public readonly Util.ParamActions onSync = new Util.ParamActions();
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter()
        {
            if (GamePath.debug.guideUseDebug)
            {
                Sync(GamePath.debug.guideDebugStep);
            }
        }
        public void OnMainExit() { }
        public void Sync(int finishGuideId)
        {
            this.finishGuideId = finishGuideId;
            AutoAccept();
        }
        public void AutoAccept()
        {
            m_guide = new Guide();

            var guideCfgs = geniusbaby.tab.guide.Inst().RecordArray;
            if (finishGuideId <= 0)
            {
                m_guide = new Guide() { guideCfg = guideCfgs[0], stepId = 0 };
            }
            else
            {
                var guideCfg = guideCfgs.Find(it => it.prevId == finishGuideId);
                if (guideCfg != null && guideCfg.acceptType == geniusbaby.cfg.guide.AutoAccept)
                {
                    m_guide = new Guide() { guideCfg = guideCfg, stepId = 0 };
                }
            }
        }
        public void ScenarioAccept(int guideId)
        {
            m_guide = new Guide();

            var guideCfg = geniusbaby.tab.guide.Inst().Find(guideId);
            if (guideCfg.acceptType == geniusbaby.cfg.guide.ScenarioAccept)
            {
                m_guide = new Guide() { guideCfg = guideCfg, stepId = 0 };
            }
        }
        public void Finish()
        {
            finishGuideId = guide.guideCfg.id;
        }
        public void Update(Guide guide)
        {
            m_guide = guide;
        }
    }
}