using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
namespace geniusbaby.ui
{
    public class MissionItemPanel : IItemLogic<MissionItemPanel>, IListItemBase<CombatMission>
    {
        public Graphic doneRoot;
        public Graphic failedRoot;
        public Text desText;
        public Text progressText;
        public Text timeText;
        CombatMission m_mission;
        Outline m_outline;
        public CombatMission AttachValue
        {
            get { return m_mission; }
            set
            {
                OnUpdateMission(m_mission = value);
            }
        }
        public int index { get; set; }
        public IListBase ListComponent { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            var child = transform.Find("Done");
            if (child) { doneRoot = child.GetComponent<Graphic>(); }

            child = transform.Find("Failed");
            if (child) { failedRoot = child.GetComponent<Graphic>(); }

            child = transform.Find("Des");
            if (child) { desText = child.GetComponent<Text>(); }

            child = transform.Find("Progress");
            if (child) { progressText = child.GetComponent<Text>(); }
            m_outline = progressText.GetComponent<Outline>();

            child = transform.Find("Time");
            if (child) { timeText = child.GetComponent<Text>(); }
        }
        public override void OnShow()
        {
            base.OnShow();
            CombatMissionManager.onMissionUpdate.Add(OnUpdateMission);
            Util.TimerManager.Inst().Add(OnTimer, 1000);
        }
        public override void OnHide()
        {
            Util.TimerManager.Inst().Remove(OnTimer);
            CombatMissionManager.onMissionUpdate.Rmv(OnUpdateMission);
            base.OnHide();
        }
        public void SetColor(Color color)
        {
            if (m_outline) { m_outline.effectColor = color; }
        }
        void OnTimer()
        {
            if (!timeText) { return; }
            if (m_mission.timeLimit.max > 0)
            {
                var left = m_mission.timeLimit.max - m_mission.timeLimit.current;
                if (left < 0) { left = 0; }
                timeText.text = (left / 1000).ToString();
            }
            else
            {
                timeText.text = string.Empty;
            }
        }
        void OnUpdateMission(CombatMission mission)
        {
            if (m_mission == mission)
            {
                OnTimer();
                if (desText)
                {
                    var missionCfg = m_mission.mission;
                    //var missionTipCfg = tab.combatMissionTip.Inst().Find((int)missionCfg.content.missionType);
        
                    //switch (missionCfg.content.missionType)
                    //{
                    //    case CombatMissionType.Score_Get://总分达到(2,0,0,x)(类型，0，0，分数)
                    //        desText.text = string.Format(missionTipCfg.tip, null, missionCfg.content.param.ToString()); break;
                    //    case CombatMissionType.Fish_KillSpec://消灭指定鱼(3,0,x,x)(类型，0，编号，个数)
                    //        {
                    //            var fishResCfg = tab.fishRes.Inst().Find(missionCfg.content.moduleId);
                    //            desText.text = string.Format(missionTipCfg.tip, fishResCfg.name, missionCfg.content.param.ToString()); break;
                    //        }
                    //    case CombatMissionType.Fish_killAny://时间内消灭任意鱼(4,x,0,x)(类型，时间，0，个数)
                    //        desText.text = string.Format(missionTipCfg.tip, null, missionCfg.content.param.ToString()); break;
                    //    case CombatMissionType.Alphabet_Wrong://错误字母(6,0,0,x)(类型，0，0，个数)
                    //        desText.text = string.Format(missionTipCfg.tip, null, missionCfg.content.param.ToString()); break;
                    //    case CombatMissionType.Ahphabet_Right://时间内答对正确字母(7,x,0,x)(类型，时间，0，个数)
                    //        desText.text = string.Format(missionTipCfg.tip, null, missionCfg.content.param.ToString()); break;
                    //    case CombatMissionType.Ahphabet_Combo://连续答对字母combo(8,0,0,x)(类型，0，0，个数)
                    //        desText.text = string.Format(missionTipCfg.tip, null, missionCfg.content.param.ToString()); break;
                    //    case CombatMissionType.Level_FinishInTime://时间内完成3轮(关卡模式专用)(10,x,0,0)(类型，时间，0，0)
                    //        desText.text = string.Format(missionTipCfg.tip, missionCfg.content.time.ToString(), null, null); break;
                    //}
                }

                if (progressText)
                {
                    if (m_mission.status != CombatMission.Status.Done)
                    {
                        progressText.text = m_mission.progress.ToStyle1();
                    }
                    else
                    {
                        progressText.text = string.Empty;
                    }
                }

                if (doneRoot) doneRoot.enabled = m_mission.status == CombatMission.Status.Done;
                if (failedRoot) failedRoot.enabled = m_mission.status == CombatMission.Status.Failed;
            }
        }
    }
}