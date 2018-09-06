using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class MapPvpFrame : ILSharpScript
    {
//generate code begin
        public Transform BG_Lc_root;
        public Text BG_Lc_name;
        public RectTransform BG_Lc_star;
        public Image BG_Rc_title_icon;
        public Text BG_Rc_title_lv;
        public RectTransform BG_Rc_title_bar_exp;
        public LSharpListSuper BG_Rc_Clip_map;
        public Button BG_fight;
        void __LoadComponet(Transform transform)
        {
            BG_Lc_root = transform.Find("BG/Lc/@root").GetComponent<Transform>();
            BG_Lc_name = transform.Find("BG/Lc/@name").GetComponent<Text>();
            BG_Lc_star = transform.Find("BG/Lc/@star").GetComponent<RectTransform>();
            BG_Rc_title_icon = transform.Find("BG/Rc/title/@icon").GetComponent<Image>();
            BG_Rc_title_lv = transform.Find("BG/Rc/title/@lv").GetComponent<Text>();
            BG_Rc_title_bar_exp = transform.Find("BG/Rc/title/bar/@exp").GetComponent<RectTransform>();
            BG_Rc_Clip_map = transform.Find("BG/Rc/Clip/@map").GetComponent<LSharpListSuper>();
            BG_fight = transform.Find("BG/@fight").GetComponent<Button>();
        }
        void __DoInit()
        {
            BG_Rc_Clip_map.OnInitialize();
        }
        void __DoUninit()
        {
            BG_Rc_Clip_map.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Rc_Clip_map.OnShow();
        }
        void __DoHide()
        {
            BG_Rc_Clip_map.OnHide();
        }
//generate code end
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_Rc_Clip_map.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(MapPvpItemPanel).Name);
            __DoInit();

            BG_fight.onClick.AddListener(() =>
            {
                var no = BG_Rc_Clip_map.itemSelected.First;
                if (no == null) { return; }
                
                var cls = KnowledgeModule.Inst().classId;
                if (cls >= 1)
                {
                    var pvp = new GSrvPvpRequest()
                    {
                        combatType = GlobalDefine.GTypeFightMulti_1S1,
                        mapId = (int)BG_Rc_Clip_map.values[no.Value],
                    };
                    var classId = KnowledgeModule.Instance.classId;
                    var classCfgs = KnowledgeModule.Inst().FindLGAP();
                    var list = new List<geniusbaby.cfg.word>();
                    int tryCount = 0;
                    while (list.Count < 10 && tryCount < 30)
                    {
                        var classCfg = classCfgs[Framework.rand.Next(cls)];
                        var select = Framework.rand.Next(classCfg.words.Count);

                        var wordCfg = classCfg.words[select];
                        if (!list.Contains(wordCfg))
                        {
                            list.Add(wordCfg);
                        }
                        ++tryCount;
                    }
                    list.ForEach(it => pvp.knows.Add(new ProtoKnowledge() { english = it.english, chinese = it.chinese }));
                    HttpNetwork.Inst().Communicate(pvp);
                }
                else
                {

                }
            });

            //petButton.onClick.AddListener(() =>
            //{
            //    if (PetModule.Inst().equipId > 0)
            //    {
            //        GuiManager.Inst().ShowFrame<PetFrame>();
            //    }
            //});
        }
        //public Image petImage;
        //public Button petButton;
        public override void OnUnInitialize()
        {
            __DoUninit();
            base.OnUnInitialize();
        }
        public override void OnShow()
        {
            base.OnShow();
            __DoShow();
            
            PetModule.Inst().onEquipedSync.Add(OnPetSync);
            OnPetSync();

            var mapIds = new List<object>();
            for (int index = 0; index < geniusbaby.tab.mapBind.Inst().RecordArray.Count; ++index)
            {
                var bc = geniusbaby.tab.mapBind.Inst().RecordArray[index];
                if (bc.type == 2)
                {
                    Array.ForEach(bc.mapIds, it => mapIds.Add(it));
                    break;
                }
            }
            BG_Rc_Clip_map.SetValues(mapIds);
        }
        public override void OnHide()
        {
            PetModule.Inst().onEquipedSync.Rmv(OnPetSync);

            __DoHide();
            base.OnHide();
        }
        void OnPetSync()
        {
            //var pet = PetModule.Inst().GetEquiped();
            //if (pet != null)
            //{
            //    var petCfg = tab.hero.Inst().Find(pet.mId);
            //    var resCfg = tab.objRes.Inst().Find(petCfg.star.resId);
            //    petImage.enabled = true;
            //    petImage.sprite = SpritesManager.Inst().Find(resCfg.icon);
            //}
            //else
            //{
            //    petImage.enabled = false;
            //}
        }
    }
}
