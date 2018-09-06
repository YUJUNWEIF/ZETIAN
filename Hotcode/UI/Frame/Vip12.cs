using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class Vip12Frame : ILSharpScript
    {
//generate code begin
        public Image image;
        void __LoadComponet(Transform transform)
        {
            image = transform.Find("@image").GetComponent<Image>();
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
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
           // image.transform.RotateAround(Vector3.zero,0.5f*Time.deltaTime);
             
            Util.TimerManager.Inst().onFrameUpdate.Add(OnUpdate);
           
        }
        public override void OnUnInitialize()
        {
            Util.TimerManager.Inst().onFrameUpdate.Rmv(OnUpdate);
            base.OnUnInitialize();
        }
        void OnUpdate()
        {
            image.transform.Rotate(0, Time.deltaTime* 360,0);

        }
    }
}
