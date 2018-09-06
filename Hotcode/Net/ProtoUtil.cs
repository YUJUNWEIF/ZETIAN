using System;
using System.Collections.Generic;
using UnityEngine;

namespace geniusbaby.pps
{
    public class ProtoUtil
    {
        public static void ErrRet(int ret)
        {
            var frame = GuiManager.Instance.ShowFrame(typeof(LSharpScript.MessageBoxFrame).Name);
            var script = LSharpScript.T.As<LSharpScript.MessageBoxFrame>(frame);
            var msgCfg = tab.messageTip.Inst().Find(ret);
            if (msgCfg != null)
            {
                script.SetDesc(msgCfg.des, LSharpScript.MsgBoxType.Mbt_Ok);
            }
            else
            {
                script.SetDesc("Unknown error!", LSharpScript.MsgBoxType.Mbt_Ok);
            }
        }
    }
}
