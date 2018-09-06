using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class GuideMaskFrame : ILSharpScript
    {
        public const string pathLineFxName = "PathLine";
        //PathIndicator m_pathIndicator;
//generate code begin
        public RawImage mask;
        public RectTransform root;
        public RectTransform root_npcLeft;
        public RawImage root_npcLeft_npc;
        public RectTransform root_npcRight;
        public RectTransform root_dlgLeft;
        public RectTransform root_dlgRight;
        public Text root_dlgRight_dialog;
        public GuiSpriteLine line;
        public DragGridCell highlight;
        public Transform highlight_arrow;
        public DragGridCell dst;
        void __LoadComponet(Transform transform)
        {
            mask = transform.Find("@mask").GetComponent<RawImage>();
            root = transform.Find("@root").GetComponent<RectTransform>();
            root_npcLeft = transform.Find("@root/@npcLeft").GetComponent<RectTransform>();
            root_npcLeft_npc = transform.Find("@root/@npcLeft/@npc").GetComponent<RawImage>();
            root_npcRight = transform.Find("@root/@npcRight").GetComponent<RectTransform>();
            root_dlgLeft = transform.Find("@root/@dlgLeft").GetComponent<RectTransform>();
            root_dlgRight = transform.Find("@root/@dlgRight").GetComponent<RectTransform>();
            root_dlgRight_dialog = transform.Find("@root/@dlgRight/@dialog").GetComponent<Text>();
            line = transform.Find("@line").GetComponent<GuiSpriteLine>();
            highlight = transform.Find("@highlight").GetComponent<DragGridCell>();
            highlight_arrow = transform.Find("@highlight/@arrow").GetComponent<Transform>();
            dst = transform.Find("@dst").GetComponent<DragGridCell>();
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
        GuideStep m_currentGuide;
        Bounds m_maskBounds;
        static int m_newStepId;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);

            highlight.onClick.Add(sender => m_currentGuide.OnClickHighLight(sender));
            highlight.onSwap.Add(dst => m_currentGuide.OnSwapHighLight());
            highlight.onDrop.Add(sender => m_currentGuide.OnDropHighLight(sender));
            highlight.onDragging.Add(sender => m_currentGuide.OnDragHighLight(sender));
            highlight.onPress.Add(sender => m_currentGuide.OnPressHighLight(sender));
            m_maskBounds = GetMaskRootBounds();
            //m_pathIndicator = GetComponentInChildren<PathIndicator>();
        }
        public void GotoNext()
        {
            if (m_currentGuide.guide.stepId + 1 < m_currentGuide.guide.guideCfg.subs.Count)
            {
                m_newStepId = m_currentGuide.guide.stepId + 1;
                if (HttpNetwork.Inst().IsSending() || MainState.Instance.IsSwitching())
                {
                    api.coroutineHelper.StartCoroutine(GotoStepWaitMainSubStateSwitchComplete(), () =>
                    {
                        m_currentGuide.guide.stepId = m_newStepId; StepProc(Convert(m_currentGuide.guide));
                    });
                }
                else
                {
                    m_currentGuide.guide.stepId = m_newStepId;
                    StepProc(Convert(m_currentGuide.guide));
                }
            }
            else
            {
                GuiManager.Inst().HideFrame(api.name);
                GuideModule.Inst().Finish();
                if (GamePath.debug.guideUseDebug || m_currentGuide.guide.guideCfg.drops.Length <= 0)
                {
                    GuideModule.Inst().AutoAccept();
                    if (HttpNetwork.Inst().IsSending() || MainState.Instance.IsSwitching())
                    {
                        api.coroutineHelper.StartCoroutineImmediate(GotoStepWaitMainSubStateSwitchComplete(), () => RestoreGuide(GuideModule.Inst().guide));
                    }
                    else
                    {
                        RestoreGuide(GuideModule.Inst().guide);
                    }
                }
                else
                {
                    HttpNetwork.Inst().Communicate(new GuideFinishRequest() { guideId = m_currentGuide.guide.guideCfg.id });
                }
            }
        }
        IEnumerator GotoStepWaitMainSubStateSwitchComplete()
        {
            if (HttpNetwork.Inst().IsSending())
            {
                yield return new HttpNetwork.WaitHttpComplete();
            }
            if (MainState.Instance.IsSwitching())
            {
                yield return new WaitMainSubStateSwitchComplete();
            }
        }
        public static void RestoreGuide(Guide guide)
        {
            m_newStepId = 0;
            if (guide.guideCfg != null)
            {
                GuideStep step = Convert(guide);
                //step.Restore();
                StepProc(step);
            }
            else
            {
                GuiManager.Inst().HideFrame(typeof(GuideMaskFrame).Name);
            }
        }
        public static void FirstEntry(Guide guide)
        {
            m_newStepId = 0;
            if (guide.guideCfg.id == 1 && guide.stepId == 0)
            {
                var mapId = guide.guideCfg.taskType.mapId;
                var petMId = guide.guideCfg.taskType.gunId;
                FightSceneManager.Inst().PVEDemoEnter(mapId, -1, null, petMId);
                MainState.Instance.PushState<SubFightState>();
            }
            else
            {
                for (int stepIndex = 0; stepIndex < guide.guideCfg.restores.Length; ++stepIndex)
                {
                    var restoreCfg = guide.guideCfg.restores[stepIndex];
                    var frame = GuiManager.Instance.GetCachedFrame(restoreCfg.frame);
                    var child = frame.transform.Find(restoreCfg.button);
                    GuideMaskFrame.GetClickHandler(child.gameObject).OnPointerClick(GuideStep.tped);
                }
                RestoreGuide(guide);
            }
        }
        static void StepProc(GuideStep step)
        {
            GuiManager.Inst().HideFrame(typeof(GuideMaskFrame).Name);
            DialogFrame.NotifyWhenFinished(step.stepCfg.dialogBefore, () =>
            {
                var frame = GuiManager.Instance.ShowFrame(typeof(GuideMaskFrame).Name);
                var script = T.As<GuideMaskFrame>(frame);
                script.DoStep(step);
            });
        }
        public static void Trigger(bool msgShield)
        {
            var frame = GuiManager.Instance.GetCachedFrame(typeof(GuideMaskFrame).Name);
            var script = T.As<GuideMaskFrame>(frame);
            script.mask.enabled = msgShield;
        }
        void DoStep(GuideStep step)
        {
            //if (m_currentGuide!= null) { m_currentGuide.Leave(); }

            m_currentGuide = step;
            m_currentGuide.frame = this;
            highlight.clickable = false;
            highlight.draggle = false;
            dst.clickable = false;
            dst.draggle = false;

            Util.UnityHelper.Hide(highlight);
            Util.UnityHelper.Hide(dst);
            //Util.UnityHelper.Hide(m_pathIndicator);
            Util.UnityHelper.Hide(root);
            Util.UnityHelper.Hide(line);
            m_currentGuide.Enter();
        }
        Bounds GetMaskRootBounds()
        {
            var viewRect = mask.rectTransform;
            var viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            return viewBounds;
        }
        public void PlayArrowTip(GuideDirection dir, Transform from = null, Transform dst = null)
        {
            const float speedPress = 20f;
            const float speedDrag = 100f;
            highlight_arrow.localPosition = Vector3.zero;
            if (from && dst)
            {
                var distance = highlight_arrow.worldToLocalMatrix * (dst.position - from.position);
                var duaration = distance.magnitude / speedDrag;
                var tp = TweenPosition.Begin(highlight_arrow.gameObject, duaration, highlight_arrow.localPosition + (Vector3)distance);
                tp.style = UITweener.Style.Loop;
                tp.ignoreTimeScale = true;
            }
            else
            {
                var distance = Vector3.zero;
                switch (dir)
                {
                    case GuideDirection.Left: distance = Vector3.left * speedPress; break;
                    case GuideDirection.Right: distance = Vector3.right * speedPress; break;
                    case GuideDirection.Up: distance = Vector3.up * speedPress; break;
                    case GuideDirection.Down: distance = Vector3.down * speedPress; break;
                }
                var duaration = 1;
                TweenPosition.Begin(highlight_arrow.gameObject, duaration, highlight_arrow.localPosition + distance).style = UITweener.Style.PingPong;
            }
            for (var index = GuideDirection.Left; index <= GuideDirection.Down; ++index)
            {
                var child = highlight_arrow.Find(index.ToString());
                Util.UnityHelper.ShowHide(child, index == dir);
            }
        }

        void Update()
        {
            if (m_newStepId == m_currentGuide.guide.stepId)
            {
                m_currentGuide.Update();
            }
        }
        public void Dialog(GuideStep step)
        {
            if (string.IsNullOrEmpty(step.stepCfg.des))
            {
                Util.UnityHelper.Hide(root);
            }
            else
            {
                Util.UnityHelper.Show(root);
                root_dlgRight_dialog.text = step.stepCfg.des;
            }
        }
        static GuideStep Convert(Guide guide)
        {
            GuideStep guideStep = null;
            var stepCfg = guide.guideCfg.subs[guide.stepId];
            var contents = stepCfg.content.Split(',');
            switch (int.Parse(contents[0]))
            {
                case geniusbaby.cfg.guide.Empty: guideStep = new EmptyStep(); break;
                case geniusbaby.cfg.guide.ClickUI: guideStep = new ClickUIStep(); break;
                case geniusbaby.cfg.guide.TipUI: guideStep = new TipUIStep(); break;
                case geniusbaby.cfg.guide.ShowUI: guideStep = new UIShowHideStep(true); break;
                case geniusbaby.cfg.guide.HideUI: guideStep = new UIShowHideStep(false); break;
                case geniusbaby.cfg.guide.ClickFish: guideStep = new Obj3DStep(); break;
                case geniusbaby.cfg.guide.ClickScreen: guideStep = new ClickScreenStep(); break;
                case geniusbaby.cfg.guide.ResumePause: guideStep = new ResumePauseStep(); break;
                case geniusbaby.cfg.guide.CombatFinish: guideStep = new CombatFinishStep(); break;
                case geniusbaby.cfg.guide.KnowledgeBuy: guideStep = new KnowledgeBuyStep(); break;
            }
            guideStep.Initialize(guide);
            return guideStep;
        }
        public static IPointerClickHandler GetClickHandler(GameObject go)
        {
            if (go == null) { return null; }

            var components = new List<Component>();
            go.GetComponents(components);
            for (var i = 0; i < components.Count; i++)
            {
                var component = components[i];
                var handler = component as IPointerClickHandler;
                if (handler == null) continue;

                var behaviour = component as Behaviour;
                if (!behaviour || behaviour.enabled)//&& behaviour.isActiveAndEnabled)
                {
                    return handler;
                }
            }
            return GetClickHandler(go.transform.parent.gameObject);
            //return null;
        }
    }
}
