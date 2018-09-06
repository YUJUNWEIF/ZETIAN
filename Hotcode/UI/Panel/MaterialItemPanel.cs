using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class Mat
    {
        public enum MType
        {
            Item = 1,
            Token = 2,
            Hero = 3,
        }
        public MType type;
        public int mId;
        public int needCount;
    }
    public class MaterialItemPanel : ILSharpScript
    {
//generate code begin
        public Image icon;
        public Text count;
        void __LoadComponet(Transform transform)
        {
            icon = transform.Find("@icon").GetComponent<Image>();
            count = transform.Find("@count").GetComponent<Text>();
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
        Mat m_mat;
        RectTransform m_cachedRc;
        public bool satisfy { get; private set; }
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            m_cachedRc = (RectTransform)api.transform;
        }
        //RectTransform m_cachedRc;
        public Mat GetValue() { return m_mat; }
        public void SetValue(Mat value)
        {
            m_mat = value;
            if (m_mat.mId > 0)
            {
                var itemCfg = geniusbaby.tab.item.Inst().Find(m_mat.mId);
                icon.sprite = SpritesManager.Inst().Find(itemCfg.icon);
                count.text = m_mat.needCount.ToString();
                OnSync();
            }
            else
            {
                icon.sprite = null;
                count.text = string.Empty;
            }
        }
        public override void OnShow()
        {
            base.OnShow();
            PackageModule.Instance.onSync.Add(OnSync);
            PackageModule.Instance.onUpdate.Add(OnUpdate);
            PlayerModule.Instance.onSync.Add(OnSync);
        }
        public override void OnHide()
        {
            PackageModule.Instance.onSync.Rmv(OnSync);
            PackageModule.Instance.onUpdate.Rmv(OnUpdate);
            PlayerModule.Instance.onSync.Rmv(OnSync);
            base.OnHide();
        }
        void OnUpdate(PackageItem mat)
        {
            OnSync();
        }
        void OnSync()
        {
            if (m_mat.mId <= 0) { return; }
            switch (m_mat.type)
            {
                case Mat.MType.Item:
                    {
                        Item mat = PackageModule.Inst().FindAllWithModuleId(m_mat.mId);
                        satisfy = (mat.count >= m_mat.needCount);
                        var counter = new RangeValue(mat.count, m_mat.needCount);
                        count.text = counter.ToStyle1();
                        count.color = (counter.current >= counter.max) ? Color.green : Color.red;
                    }
                    break;
                case Mat.MType.Token:
                    {
                        int own = TokenModule.Inst().Get(m_mat.mId);
                        satisfy = (own >= m_mat.needCount);
                        var sb = new StringBuilder();
                        sb.Append(Util.UnityHelper.TenThousand(own));
                        sb.Append('/');
                        sb.Append(Util.UnityHelper.TenThousand(m_mat.needCount));
                        count.text = sb.ToString();
                        count.color = satisfy ? Color.green : Color.red;
                    }
                    break;
                case Mat.MType.Hero:
                    {
                        int own = PetModule.Inst().FindAllWithModuleId(m_mat.mId);
                        satisfy = (own >= m_mat.needCount);
                        var counter = new RangeValue(own, m_mat.needCount);
                        count.text = counter.ToStyle1();
                        count.color = satisfy ? Color.green : Color.red;
                    }
                    break;
            }
        }
        public void OnPointerEnter()
        {
            if (m_mat.mId > 0)
            {
                var frame = GuiManager.Inst().ShowFrame(typeof(TipFrame).Name);
                var script = T.As<TipFrame>(frame);
                script.DisplayItem(m_cachedRc, m_mat.mId);
            }
        }
        public void OnPointerExit()
        {
            GuiManager.Inst().HideFrame(typeof(TipFrame).Name);
        }
    }
}
