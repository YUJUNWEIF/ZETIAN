using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class DialogFrame : ILSharpScript
    {
//generate code begin
        public RectTransform left;
        public RectTransform right;
        public Text dlg_chinese1;
        public Text dlg_chinese2;
        public Text dlg_english1;
        public Text dlg_english2;
        public Button dlg_next;
        public Text dlg_Image_name;
        public Button skip;
        void __LoadComponet(Transform transform)
        {
            left = transform.Find("@left").GetComponent<RectTransform>();
            right = transform.Find("@right").GetComponent<RectTransform>();
            dlg_chinese1 = transform.Find("dlg/@chinese1").GetComponent<Text>();
            dlg_chinese2 = transform.Find("dlg/@chinese2").GetComponent<Text>();
            dlg_english1 = transform.Find("dlg/@english1").GetComponent<Text>();
            dlg_english2 = transform.Find("dlg/@english2").GetComponent<Text>();
            dlg_next = transform.Find("dlg/@next").GetComponent<Button>();
            dlg_Image_name = transform.Find("dlg/Image/@name").GetComponent<Text>();
            skip = transform.Find("@skip").GetComponent<Button>();
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
        protected PetDisplayObj m_heroDisplay;
        static geniusbaby.cfg.dialog m_dialogCfg;
        static int m_dialogIndex;
        public static Util.ParamActions afterDialog = new Util.ParamActions();
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            dlg_next.onClick.AddListener(() =>
            {
                if (m_dialogCfg != null && m_dialogIndex < m_dialogCfg.convs.Count)
                {
                    BeginDialog(m_dialogCfg.convs[m_dialogIndex]);
                    ++m_dialogIndex;
                }
                else
                {
                    GuiManager.Instance.HideFrame(api.name);
                    afterDialog.Fire();
                }
            });
            skip.onClick.AddListener(() =>
            {
                GuiManager.Instance.HideFrame(api.name);
                afterDialog.Fire();
            });
            m_heroDisplay = new GameObject("heroDisplay").AddComponent<PetDisplayObj>();
        }
        public override void OnUnInitialize()
        {
            GameObjPool.Instance.DestroyObj(m_heroDisplay.gameObject);
            base.OnUnInitialize();
        }
        public override void OnHide()
        {
            base.OnHide();
            m_heroDisplay.UnInitialize();
        }
        public static void NotifyWhenFinished(int dlgId, Action dialogFinished = null)
        {
            if (dlgId > 0)
            {
                m_dialogCfg = geniusbaby.tab.dialog.Inst().Find(dlgId);
                m_dialogIndex = 0;
                afterDialog.Clear();
                afterDialog.Add(dialogFinished);
                var frame = GuiManager.Instance.ShowFrame(typeof(DialogFrame).Name);
                var script = T.As<DialogFrame>(frame);
                script.dlg_next.onClick.Invoke();
            }
            else
            {
                if (dialogFinished != null) dialogFinished();
            }
        }
        void BeginDialog(geniusbaby.cfg.dialog._conversation conversation)
        {
            m_heroDisplay.UnInitialize();
            //var eId = new EntityId(Obj3DType.Player, 0);

            //m_heroDisplay.Initialize(eId, dlg.objId);
            //npcNameText.text = tab.hero.Inst().Find(dlg.objId).name;
            dlg_Image_name.text = "from cfg";

            m_heroDisplay.gameObject.layer = Util.TagLayers.UI;
            Util.UnityHelper.Show(m_heroDisplay, conversation.posId == 1 ? left : right, true);

            //try
            //{
            //    var animType = (AnimationType)Enum.Parse(typeof(AnimationType), dlg.action, true);
            //    m_heroDisplay.Display(animType);
            //}
            //catch
            //{
            //    m_heroDisplay.Display(AnimationType.Idle);
            //}

            var splits = conversation.chinese.Split('|');
            dlg_chinese1.text = splits[0];
            dlg_chinese2.text = splits.Length >= 2 ? splits[1] : string.Empty;

            splits = conversation.english.Split('|');
            dlg_english1.text = splits[0];
            dlg_english2.text = splits.Length >= 2 ? splits[1] : string.Empty;
        }
    }
}
