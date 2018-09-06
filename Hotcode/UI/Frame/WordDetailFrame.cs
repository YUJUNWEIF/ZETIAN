using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class WordDetailFrame : WordDictFrame
    {
        public Text Basic_translation;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            Basic_translation = api.transform.Find("Basic/@translation").GetComponent<Text>();
        }
        public void Display(int subPackageId, string english, string chinese)
        {
            //base.Display(word.english);
            //var subCfg = tab.wordPackageSub.Inst().Find(subPackageId);
            string packageName = string.Empty;
            Basic_translation.text = GlobalString.Format(geniusbaby.cfg.CodeDefine.GString_ClsTrans, packageName, chinese);
        }
    }
}
