using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class LvlUpFrame : ILSharpScript
    {
//generate code begin
        public Text Root_Image_oldLv;
        public Text Root_Image_curLv;
        public Button Root_Image_funcDes;
        void __LoadComponet(Transform transform)
        {
            Root_Image_oldLv = transform.Find("Root/Image/@oldLv").GetComponent<Text>();
            Root_Image_curLv = transform.Find("Root/Image/@curLv").GetComponent<Text>();
            Root_Image_funcDes = transform.Find("Root/Image/@funcDes").GetComponent<Button>();
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
        }
    }
}
