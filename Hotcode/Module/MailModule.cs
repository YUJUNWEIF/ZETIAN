using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class Mail
    {
        public int id;
        public string senderId;
        public string titleOrName;
        public string content;
        public long createUtc;
        public long endUtc;
        public bool hasRead;
        public List<Item> dropItems;
    }

    public class MailModule : Singleton<MailModule>, IModule
    {
        public List<Mail> mails = new List<Mail>();
        public Util.ParamActions onMailSync = new Util.ParamActions();
        public Util.Param1Actions<Mail> onMailUpdate = new Util.Param1Actions<Mail>();
        
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        public void Sync(List<Mail> forms)
        {
            this.mails = forms;
            onMailSync.Fire();
        }
        public void Update(List<Mail> ups)
        {
            for (int index = 0; index < ups.Count; ++index)
            {
                var el = ups[index];
                int exist = this.mails.FindIndex(info => info.id == el.id);
                if (exist >= 0)
                {
                    this.mails[exist] = el;
                    onMailUpdate.Fire(el);
                }
                else
                {
                    this.mails.Add(el);
                    onMailSync.Fire();
                }
            }
        }
        public void Remove(int mailId)
        {
            int exist = mails.FindIndex(info => info.id == mailId);
            if (exist >= 0) { mails.RemoveAt(exist);
                onMailSync.Fire();
            }
        }
    }
}

