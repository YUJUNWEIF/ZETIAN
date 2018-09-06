//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using System;
//using System.Collections;
//using System.Collections.Generic;

//namespace geniusbaby.LSharpScript
//{
//    public class ItemUsePanel
//    {
//        int m_targetId;
//        Func<int, bool> m_canUse;
//        public void Display(List<PackageItem> items, int targetId, Func<int, bool> canUse)
//        {
//            m_targetId = targetId;
//            m_canUse = canUse;
//            SetValues(items);
//        }
//        void OnFireStackItem(int stackId, int count)
//        {
//            var items = new List<PackageItem>(values);
//            var index = items.FindIndex(it => it.stackId == stackId);
//            if (index >= 0)
//            {
//                var stackItem = values[index];
//                stackItem.count -= count;
//                if (stackItem.count <= 0)
//                {
//                    items.RemoveAt(index);
//                }
//                else
//                {
//                    items[index] = stackItem;
//                }
//                SetValues(items);
//            }
//        }
//        public void OnUse(PackageItem item, int count)
//        {
//            if (m_canUse(m_targetId) && count > 0)
//            {
//                HttpNetwork.Inst().Communicate(new pps.ItemUseRequest() { stackId = item.stackId, count = count, targetId = m_targetId });
//            }
//        }
//        //void Update()
//        //{
//        //    if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
//        //    {
//        //        var touch = Input.GetTouch(0);
//        //        cursor = new Cursor(touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled, touch.position);
//        //    }
//        //    else
//        //    {
//        //        cursor = new Cursor(Input.GetMouseButton(0), Input.mousePosition);
//        //    }
//        //    if (cursor.down)
//        //    {
//        //        var results = GuiManager.FindGraphicAtPointer(cursor.position);
//        //        if (results.Count > 0)
//        //        {
//        //            var script = results[0].gameObject.GetComponent<ItemUseItemPanel>();
//        //            if (script && script!= m_lastHover)
//        //            {
//        //                if (m_lastHover) { m_lastHover.OnPointerExit(); }
//        //            }
//        //        }
//        //    }
//        //}
//    }