using System;
using System.Collections.Generic;

namespace geniusbaby
{
    class Dialog
    {

        //public void OnResult()
        //{
        //    coroutineHelper.StartCoroutine(_Result());
        //}
        //void beforePVEDialog()
        //{
        //    var thisIsPVE = FightSceneManager.Instance.thisIsPVE;
        //    if (thisIsPVE.combatId > 0 && thisIsPVE.star <= 0)
        //    {
        //        cfg.helper.challenge_conversation.Conversation conversation = null;
        //        if (cfg.helper.challenge_conversation.conversations.TryGetValue(thisIsPVE.combatId, out conversation))
        //        {
        //            if (conversation.before.Count > 0)
        //            {
        //                FightSceneManager.Instance.pause = true;
        //                DialogFrame.NotifyWhenFinished(conversation.before, () =>
        //                {
        //                    FightSceneManager.Instance.pause = false;
        //                });
        //            }
        //        }
        //    }
        //}
        //void afterPVEDialog(System.Action after)
        //{
        //    var thisIsPVE = FightSceneManager.Instance.thisIsPVE;
        //    if (thisIsPVE.combatId > 0 && thisIsPVE.star <= 0 && FEModule.Instance.CaculateResult() > 0)
        //    {
        //        cfg.helper.challenge_conversation.Conversation conversation = null;
        //        if (cfg.helper.challenge_conversation.conversations.TryGetValue(thisIsPVE.combatId, out conversation))
        //        {
        //            if (conversation.after.Count > 0)
        //            {
        //                DialogFrame.NotifyWhenFinished(conversation.after, () => after());
        //            }
        //            else { after(); }
        //        }
        //        else { after(); }
        //    }
        //    else { after(); }
        //}
        //IEnumerator _Result()
        //{
        //    if (FEModule.Inst().CaculateResult() > 0) { Util.UnityHelper.Show(m_victor); }
        //    else
        //    {
        //        Util.UnityHelper.Show(m_defeat);
        //    }
        //    yield return new WaitForSeconds(2f);
        //    afterPVEDialog(() =>
        //    {
        //        var resultReporter = FightSceneManager.Instance.resultReporter;
        //        if (resultReporter != null)
        //        {
        //            resultReporter();
        //        }
        //        else
        //        {
        //            GuiManager.Instance.HideFrame(this);
        //        }
        //    });
        //}
    }
}
