using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PetMergeFrame : ILSharpScript
    {
//generate code begin
        public LSharpListContainer BG_Material;
        public Image BG_root;
        public Button BG_confirm;
        public Image BG_confirm_costType;
        public Text BG_confirm_costValue;
        public RectTransform BG_Lc_clip_list;
        public Toggle BG_Lc_group_type1;
        public Toggle BG_Lc_group_type2;
        public Toggle BG_Lc_group_type3;
        public Toggle BG_Lc_group_type4;
        public Toggle BG_Lc_group_type5;
        void __LoadComponet(Transform transform)
        {
            BG_Material = transform.Find("BG/@Material").GetComponent<LSharpListContainer>();
            BG_root = transform.Find("BG/@root").GetComponent<Image>();
            BG_confirm = transform.Find("BG/@confirm").GetComponent<Button>();
            BG_confirm_costType = transform.Find("BG/@confirm/@costType").GetComponent<Image>();
            BG_confirm_costValue = transform.Find("BG/@confirm/@costValue").GetComponent<Text>();
            BG_Lc_clip_list = transform.Find("BG/Lc/clip/@list").GetComponent<RectTransform>();
            BG_Lc_group_type1 = transform.Find("BG/Lc/group/@type1").GetComponent<Toggle>();
            BG_Lc_group_type2 = transform.Find("BG/Lc/group/@type2").GetComponent<Toggle>();
            BG_Lc_group_type3 = transform.Find("BG/Lc/group/@type3").GetComponent<Toggle>();
            BG_Lc_group_type4 = transform.Find("BG/Lc/group/@type4").GetComponent<Toggle>();
            BG_Lc_group_type5 = transform.Find("BG/Lc/group/@type5").GetComponent<Toggle>();
        }
        void __DoInit()
        {
            BG_Material.OnInitialize();
        }
        void __DoUninit()
        {
            BG_Material.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Material.OnShow();
        }
        void __DoHide()
        {
            BG_Material.OnHide();
        }
//generate code end
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
    }
}
