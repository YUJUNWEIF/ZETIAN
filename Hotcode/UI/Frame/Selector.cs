using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby.LSharpScript
{
    public class SelectValue<T>
    {
        public T value;
        public bool canSelect;
    }
    public class ListSelector<TValue>
    {
        public Action<IList<int>> confirm;
        public bool singleSelect;
        public int maxSelectCount;
        public IList<int> alreadySelect;
        public IList<TValue> optionsSelect;
    }
    public abstract class Selector : ILSharpScript
    {
        public interface ISelector
        {
            void OnConfirmSelect();
            void OnCancelSelect();
        }
        class ThingsSelector<TListItem, TItemDef, TValue> : ISelector
            where TListItem : IItemLogic<TItemDef>, IListItemBase<TValue>
            where TItemDef : Component
        {
            IGuiList<TListItem, TItemDef, TValue> listPanel;
            ListSelector<TValue> selector;
            public ThingsSelector(ListSelector<TValue> selector, IGuiList<TListItem, TItemDef, TValue> listPanel)
            {
                this.selector = selector;
                this.listPanel = listPanel;

                listPanel.SetValues(selector.optionsSelect);
                listPanel.singleSelect = selector.singleSelect;
                listPanel.maxSelectCount = selector.maxSelectCount;
                if (selector.alreadySelect != null)
                {
                    for (int index = 0; index < selector.alreadySelect.Count; ++index)
                    {
                        listPanel.SelectIndex(selector.alreadySelect[index]);
                    }
                }
            }
            public void OnConfirmSelect()
            {
                var selected = new List<int>();
                var no = listPanel.itemSelected.First;
                while (no != null)
                {
                    selected.Add(no.Value);
                    no = no.Next;
                }
                selector.confirm(selected);
            }
            public void OnCancelSelect() { selector.confirm(new List<int>()); }
        }
        public abstract Button GetConfirm();
        public abstract Button GetCancel();

        public ISelector selector { get; private set; }
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            GetConfirm().onClick.AddListener(() =>
            {
                GuiManager.Instance.HideFrame(api.name);
                if (selector != null) { selector.OnConfirmSelect(); }
            });
            GetCancel().onClick.AddListener(() => GuiManager.Instance.HideFrame(api.name));
        }
        public void SetSelector<TListItem, TItemDef, TValue>(ListSelector<TValue> selector, IGuiList<TListItem, TItemDef, TValue> listPanel)
            where TListItem : IItemLogic<TItemDef>, IListItemBase<TValue>
            where TItemDef : Component
        {
            this.selector = new ThingsSelector<TListItem, TItemDef, TValue>(selector, listPanel);
        }
        public void SetSelector<TListItem, TItemDef>(ListSelector<object> selector, IGuiList<TListItem, TItemDef, object> listPanel)
            where TListItem : IItemLogic<TItemDef>, IListItemBase<object>
            where TItemDef : Component
        {
            this.selector = new ThingsSelector<TListItem, TItemDef, object>(selector, listPanel);
        }
    }
}