using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class DropdownPanel : BehaviorWrapper, IPointerClickHandler
{
    public class Data
    {
        public string display { get; private set; }
        public object value { get; private set; }
        public T As<T>() { return (T)value; }
        public Data(string text, object v)
        {
            this.display = text;
            this.value = v;
        }
    }
    [RequireComponent(typeof(Button))]
    class ItemPanel : IItemLogic<ItemPanel>, IListItemBase<Data>
    {
        private Text m_text;
        private Data m_data;
        public Data AttachValue
        {
            get { return m_data; }
            set
            {
                m_data = value;
                m_text.text = m_data.display;
            }
        }
        public int index { get; set; }
        public IListBase ListComponent { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            m_text = GetComponentInChildren<Text>();
            GetComponent<Button>().onClick.AddListener(() => ListComponent.SelectIndex(index));
        }
    }
    class ListPanel : GuiListSuper<ItemPanel, ItemPanel, Data> { };

    public int m_select { get; private set; }
    public readonly Util.ParamActions selectListener = new Util.ParamActions();
    public IList<Data> values { get; private set; }

    public Button clickBtn;
    public Text text;
    public Transform clipRoot;
    ListPanel m_downPanel;
    RectTransform m_blocker;
    void Awake()
    {
        var templGo = clipRoot.Find("Templ").gameObject;
        var templ = templGo.AddComponent<ItemPanel>();
        templGo.SetActive(false);

        var listGo = clipRoot.Find("Lister").gameObject;
        m_downPanel = listGo.AddComponent<ListPanel>();
        m_downPanel.listItem = templ;

        m_blocker = (RectTransform)transform.Find("Blocker");
    }
    public override void OnInitialize()
    {
        base.OnInitialize();
        m_downPanel.OnInitialize();
        clickBtn.onClick.AddListener(() =>
        {
            ClipShow();
        });
        m_downPanel.selectListener.Add(() =>
        {
            var no = m_downPanel.itemSelected.First;
            if (no != null)
            {
                select = no.Value;
            }
            else
            {
                select = -1;
            }
        });
    }
    public override void OnUnInitialize()
    {
        m_downPanel.OnUnInitialize();
        base.OnUnInitialize();
    }
    public override void OnShow()
    {
        base.OnShow();
        m_downPanel.OnShow();

        m_select = -1;
        m_blocker.position = new Vector3(Desktop.root.position.x, Desktop.root.position.y);
        var loc = m_blocker.localPosition;
        m_blocker.localPosition = new Vector3(loc.x, loc.y, 0f);
        m_blocker.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Desktop.realWidth);
        m_blocker.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Desktop.realHeight);

        m_blocker.gameObject.SetActive(false);
    }
    public override void OnHide()
    {
        m_downPanel.OnHide();
        base.OnHide();
    }
    public void Display(IList<Data> data, int sel)
    {
        values = data;
        select = sel;
        if (clipRoot.gameObject.activeSelf)
        {
            m_downPanel.SetValues(values);
            m_downPanel.SelectIndex(sel);
        }
    }
    public int select
    {
        get { return m_select; }
        set
        {
            m_select = value;
            if (value >= 0 && value < values.Count)
            {
                m_select = value;
                text.text = values[m_select].display;
                ClipHide();
            }
            else
            {
                m_select = -1;
            }
            selectListener.Fire();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ClipHide();
    }
    void ClipShow()
    {
        clipRoot.gameObject.SetActive(true);
        m_blocker.gameObject.SetActive(true);
        m_downPanel.SetValues(values);
        m_downPanel.SelectIndex(m_select);
    }
    void ClipHide()
    {
        clipRoot.gameObject.SetActive(false);
        m_blocker.gameObject.SetActive(false);
    }
}