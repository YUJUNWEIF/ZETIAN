using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class GMapChatItemPanel : ILSharpScript
    {
//generate code begin
        public Text talker;
        public TextImage2D textImage;
        public Text content;
        public Image newly;
        void __LoadComponet(Transform transform)
        {
            talker = transform.Find("@talker").GetComponent<Text>();
            textImage = transform.Find("@textImage").GetComponent<TextImage2D>();
            content = transform.Find("@content").GetComponent<Text>();
            newly = transform.Find("@newly").GetComponent<Image>();
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
