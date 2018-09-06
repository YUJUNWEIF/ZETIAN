using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace geniusbaby
{
    public class PetBuilding : IBaseObj
    {
        public Transform tipRoot;
        public PetBuildingFunc func;
        bool me;
        void Awake()
        {
            Util.UnityHelper.Register(this, EventTriggerType.PointerClick, ev =>
            {
                SoundManager.Instance.PlaySound(GamePath.music.soundClickBuilding);
                switch (func)
                {
                    case PetBuildingFunc.Slot1:
                    case PetBuildingFunc.Slot2:
                    case PetBuildingFunc.Slot3:
                        if (me)
                        {
                            var slot = PetModule.Inst().exploreres.Find(it => it.slotId == entityId.uniqueId);
                            tipRoot.gameObject.SetActive(slot.unlock);
                            //if (slot.unlock)
                            //{
                            //    var script = GuiManager.Inst().ShowFrame<ui.PetExplorerFrame>();
                            //    script.Display(slot);
                            //}
                            //else
                            //{
                            //    var script = GuiManager.Inst().ShowFrame<ui.PetExplorerLockFrame>();
                            //    script.Display(slot);
                            //}
                        }
                        else
                        {
                            //var slot = FriendModule.Inst().friendSlots.Find(it => it.slotId == entityId.uniqueId);
                            //tipRoot.gameObject.SetActive(slot.unlock);
                            //if (slot.unlock)
                            //{
                            //    var script = GuiManager.Inst().ShowFrame<ui.FriendPetExplorerFrame>();
                            //    script.Display(slot);
                            //}
                        }
                        break;
                    case PetBuildingFunc.Merge:
                        //if (me) GuiManager.Inst().ShowFrame<ui.PetMergeFrame>();
                        break;
                }
            });
        }
        public void Initialize(bool me)
        {
            this.me = me;
            this.entityId = new EntityId(Obj3DType.Unknown, (int)func);
            FuncTipModule.Instance.onHighLight.Add(OnUpdate);
        }
        public override void UnInitialize()
        {
            FuncTipModule.Instance.onHighLight.Rmv(OnUpdate);
            base.UnInitialize();
        }
        void OnUpdate(int[] funcIds)
        {
            var needTip = FunctionEntry.IsFuncOpen(moduleId) && FuncTipModule.Instance.GetFuncTip(moduleId);
            if (tipRoot) Util.UnityHelper.ShowHide(tipRoot, needTip);
            //    if (helper.functions.Count >= 1 && m_renderer)
            //    {
            //        var open = helper.functions.Exists(it => FunctionEntry.IsFuncOpen(it.pk));
            //        m_renderer.material.SetColor("_TintColor", Util.Helper.ColorFromString(GlobalString.Get(open ? GStringType.Building_Open : GStringType.Building_Lock)));
            //        if (lockRoot) { Util.UnityHelper.ShowHide(lockRoot, !open); }
            //    }
            //}
        }
    }
}