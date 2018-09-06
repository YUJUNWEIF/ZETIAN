using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace geniusbaby
{
    public class FriendPetBuilding : IBaseObj
    {
        public Transform tipRoot;
        public PetBuildingFunc func;

        public void Initialize()
        {
            var tipRoot = transform.Find("tip");
            if (!tipRoot) { tipRoot = transform; }

            entityId = new EntityId(Obj3DType.Unknown, (int)func);
            Util.UnityHelper.Register(this, EventTriggerType.PointerClick, ev =>
            {
                SoundManager.Instance.PlaySound(GamePath.music.soundClickBuilding);
                var slot = FriendModule.Inst().friendSlots.Find(it => it.slotId == entityId.uniqueId);
                tipRoot.gameObject.SetActive(slot.unlock);
                if (slot.unlock && slot.bookId > 0 && Util.TimerManager.Inst().RealTimeMS() >= slot.endAt)
                {
                    HttpNetwork.Inst().Communicate(new pps.FriendRobberyRequest() { id = FriendModule.Inst().friUId, slotId= slot.slotId });
                }
            });
        }
    }
}