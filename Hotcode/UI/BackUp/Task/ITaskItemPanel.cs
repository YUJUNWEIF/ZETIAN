//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using System;
//using System.Collections;
//using System.Collections.Generic;

//namespace geniusbaby.ui
//{
//    public class ITaskItemPanel : BehaviorWrapper
//    {
//        public Text nameText;
//        public Text awardText;
//        public Text progressText;
//        public Transform completeTagRoot;
//        //public Button trackButton;
//        public Button awardButton;

//        Task m_task;
//        static PointerEventData m_temp = new PointerEventData(null);
//        public void Display(Task value, string name, int[][] rewards, int conditionId)
//        {
//            m_task = value;
//            nameText.text = name;
//            progressText.text = (m_task.progress.max > 0) ? m_task.progress.ToStyle1() : string.Empty;

//            var sb = new System.Text.StringBuilder();
//            for (int index = 0; index < rewards.Length; ++index)
//            {
//                if (index > 0)
//                {
//                    sb.Append(@"，");
//                }
//                var it = rewards[index];
//                var itemCfg = tab.item.Instance.Find(it[0]);
//                sb.Append(itemCfg.name);
//                sb.Append("*");
//                sb.Append(it[1]);
//            }
//            awardText.text = sb.ToString();

//            Util.UnityHelper.ShowHide(completeTagRoot, m_task.status == Task.Status.AlreadyGet);
//            Util.UnityHelper.ShowHide(awardButton, m_task.status == Task.Status.CanGet);
//        }
//        //protected void Track(int condition_id)
//        //{
//        //    var condCfg = tab.task_condition.Instance.Get(condition_id);
//        //    if (string.IsNullOrEmpty(condCfg.scene))
//        //    {
//        //        if (string.IsNullOrEmpty(condCfg.ui)) { return; }
//        //        GotoTrack(condCfg.ui.Split(','));
//        //    }
//        //    else
//        //    {
//        //        MainState.Instance.ChangeStateByName(condCfg.scene);
//        //        if (string.IsNullOrEmpty(condCfg.ui)) { return; }
//        //        coroutineHelper.StartCoroutine(WaitMainSubStateSwitchComplete(condCfg.ui.Split(',')));
//        //    }
//        //}
//        static IEnumerator WaitMainSubStateSwitchComplete(IList<string> splits)
//        {
//            yield return new HttpNetwork.WaitHttpComplete();
//            yield return new WaitMainSubStateSwitchComplete();
//            GotoTrack(splits);
//        }
//        public static void FuncOpenGuideGotoTrack(cfg.functionTip link, Util.CoroutineHelper corHelper)
//        {
//            switch (link.type)
//            {
//                case cfg.functionTip.UI: GotoTrack(new string[] { link.target }); break;
//                case cfg.functionTip.Scene3D:
//                    {
//                        var urls = link.target.Split(',');
//                        MainState.Instance.ChangeStateByName(urls[0]);
//                        if (urls.Length > 1)
//                        {
//                            var uiUrl = new string[urls.Length - 1];
//                            Array.Copy(urls, 1, uiUrl, 0, uiUrl.Length);
//                            corHelper.StartCoroutine(WaitMainSubStateSwitchComplete(uiUrl));
//                        }
//                    }
//                    break;
//            }
//        }
//        static void GotoTrack(IList<string> splits)
//        {
//            if (splits.Count <= 0) { return; }

//            if (splits[0] != typeof(HomePageFrame).Name)
//            {
//                var script = GuiManager.Inst().ShowFrameByName(splits[0]);
//                for (int index = 1; index < splits.Count; ++index)
//                {
//                    var clicker = script.transform.Find(splits[index]);
//                    var components = new List<Component>();
//                    clicker.GetComponents(components);
//                    for (var i = 0; i < components.Count; i++)
//                    {
//                        var component = components[i];
//                        var handler = component as IPointerClickHandler;
//                        if (handler == null) continue;

//                        var behaviour = component as Behaviour;
//                        if (!behaviour || behaviour.enabled)//&& behaviour.isActiveAndEnabled)
//                        {
//                            handler.OnPointerClick(m_temp);
//                        }
//                    }
//                }
//            }
//            else
//            {
//                GuiManager.Inst().ReplaceAll<HomePageFrame>();
//                CameraControl.Inst().Reset();
//            }
//        }
//    }
//}