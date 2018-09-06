using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class Task
    {
        public enum Type
        {
            Unknown = 0,
            Daily = 1,
            Achieve = 2,
        }
        public enum Status
        {
            CanGet,
            Doing,
            AlreadyGet,
        }
        public int moduleId;
        public RangeValue progress;
        public Status status;
    }
    public class TaskModule : Singleton<TaskModule>, IModule
    {
        public List<Task> tasks = new List<Task>();
        public Util.ParamActions onTaskSync = new Util.ParamActions();
        public Util.Param1Actions<Task> onTaskUpdate = new Util.Param1Actions<Task>();

        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }

        public void Sync(List<Task> dailys)
        {
            this.tasks = dailys;
            onTaskSync.Fire();
        }
        public void Update(List<Task> dailys)
        {
            for (int index = 0; index < dailys.Count; ++index)
            {
                var daily = dailys[index];
                int exist = this.tasks.FindIndex(info => info.moduleId == daily.moduleId);
                if (exist >= 0)
                {
                    this.tasks[exist] = daily;
                    onTaskUpdate.Fire(daily);
                }
                else
                {
                    this.tasks.Add(daily);
                    onTaskSync.Fire();
                }
            }
        }
    }
}