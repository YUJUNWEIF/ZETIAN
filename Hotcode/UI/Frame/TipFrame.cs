using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class TipFrame : ILSharpScript
    {
//generate code begin
        public Image BG_icon;
        public Image BG_icon_fragment;
        public Text BG_name;
        public Text BG_des;
        void __LoadComponet(Transform transform)
        {
            BG_icon = transform.Find("BG/@icon").GetComponent<Image>();
            BG_icon_fragment = transform.Find("BG/@icon/@fragment").GetComponent<Image>();
            BG_name = transform.Find("BG/@name").GetComponent<Text>();
            BG_des = transform.Find("BG/@des").GetComponent<Text>();
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
        RectTransform cachedRc;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            cachedRc = (RectTransform)api.transform;
            api.GetComponent<Button>().onClick.AddListener(() => GuiManager.Instance.HideFrame(api.name));
        }
        public void DisplayItem(RectTransform target, int itemId)
        {
            var itemCfg = tab.item.Inst().Find(itemId);
            //BG_icon_fragment.enabled = false;
            BG_icon.sprite = SpritesManager.Instance.Find(itemCfg.des);
            BG_name.text = itemCfg.name;
            BG_des.text = itemCfg.des;
            Display(target);
        }
        public void DisplaySkill(RectTransform target, int skillId)
        {
            var skillCfg = tab.skill.Inst().Find(skillId);
            BG_icon.sprite = SpritesManager.Instance.Find(skillCfg.icon);
            BG_name.text = skillCfg.name;
            BG_des.text = skillCfg.des;
            Display(target);
        }
        enum Dir
        {
            Top = 1,
            VCenter = 2,
            Bottom = 3,

            Left = 4,
            HCenter = 5,
            Right = 6,
        }
        void Display(RectTransform target)
        {
            Vector3[] worldPos = new Vector3[4];
            Vector2[] localPos = new Vector2[4];
            target.GetWorldCorners(worldPos);
            Vector2 center = Vector2.zero;
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            for (int index = 0; index < worldPos.Length; ++index)
            {
                Framework.C2DWorldPosRectangleLocal(worldPos[index], out localPos[index]);
                center += localPos[index];
                if (localPos[index].x < minX) { minX = localPos[index].x; }
                if (localPos[index].x > maxX) { maxX = localPos[index].x; }
                if (localPos[index].y < minY) { minY = localPos[index].y; }
                if (localPos[index].y > maxY) { maxY = localPos[index].y; }
            }

            Dir v = Dir.VCenter;
            Dir h = Dir.HCenter;

            float targetX = (minX + maxY) * 0.5f;
            float targetY = (maxY + minY) * 0.5f;
            float targetWidth = maxX - minX;
            float targetHeight = maxY - minY;

            float thisX = targetX;
            float thisY = targetY;
            float thisWidth = cachedRc.sizeDelta.x;
            float thisHeight = cachedRc.sizeDelta.y;

            if (maxY + thisHeight < Desktop.realHeight * 0.5f)
            {
                v = Dir.Top;
                thisY = maxY + thisHeight * 0.5f;
            }
            else if (minY - thisHeight > -Desktop.realHeight * 0.5f)
            {
                v = Dir.Bottom;
                thisY = minY - thisHeight * 0.5f;
            }
            else
            {
                v = Dir.VCenter;
                thisY = (maxY + minY) * 0.5f;
            }

            if (v != Dir.VCenter)
            {
                if (targetX - thisWidth * 0.5f < -Desktop.realWidth * 0.5f)
                {
                    thisX = thisWidth * 0.5f - Desktop.realWidth * 0.5f;
                }
                else if (targetX + thisWidth * 0.5f > Desktop.realWidth * 0.5f)
                {
                    thisX = Desktop.realWidth * 0.5f - thisWidth * 0.5f;
                }
                else
                {
                    thisX = (minX + maxY) * 0.5f;
                }
            }
            else
            {
                if (maxX + thisWidth < Desktop.realWidth)
                {
                    thisX = maxX + thisWidth * 0.5f;
                }
                else if (minX - thisWidth > 0)
                {
                    thisX = minX - thisWidth * 0.5f;
                }
            }
            cachedRc.anchoredPosition = new Vector2(thisX, thisY);
        }
    }
}
