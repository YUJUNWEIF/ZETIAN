using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class TestFrame : ILSharpScript
    {
//generate code begin
        public Button button;
        void __LoadComponet(Transform transform)
        {
            button = transform.Find("@button").GetComponent<Button>();
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
            button.transform.localPosition = new Vector3(-277, 101, 0);
        }
    }
}
