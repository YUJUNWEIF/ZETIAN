using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class CreatePlayerFrame : ILSharpScript
    {
//generate code begin
        public Toggle Gender_boy;
        public Toggle Gender_girl;
        public InputField name;
        public Button name_rand;
        public Button confirm;
        void __LoadComponet(Transform transform)
        {
            Gender_boy = transform.Find("Gender/@boy").GetComponent<Toggle>();
            Gender_girl = transform.Find("Gender/@girl").GetComponent<Toggle>();
            name = transform.Find("@name").GetComponent<InputField>();
            name_rand = transform.Find("@name/@rand").GetComponent<Button>();
            confirm = transform.Find("@confirm").GetComponent<Button>();
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
            name_rand.onClick.AddListener(() =>
            {
                RandName();
            });
            confirm.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(name.text))
                {
                    HttpNetwork.Instance.Communicate(new PlayerCreateRequest() { name = name.text });
                }
            });
            RandName();
        }
        public override void OnShow()
        {
            base.OnShow();
            name.text = string.Empty;
        }
        private void RandName()
        {
            //var firstArray = geniusbaby.tab.name.Instance.RecordArray;
            //var lastArray = tab.lastName.Instance.RecordArray;

            //int firstIndex = Random.Range(0, firstArray.Count);
            //int lastIndex = Random.Range(0, lastArray.Count);

            //nameInput.text = firstArray[firstIndex].name + "." + lastArray[lastIndex].name;
            //nameInput.text = firstArray[firstIndex].name;
        }
    }
}
