using System;
using System.Collections.Generic;

namespace geniusbaby
{
    public class RMCommandModule //: Singleton<RMCommandManager>, IGameEvent
    {
        public struct AddItem
        {
            public IList<Item> items;
            public AddItem(int moduleId, int count) : this()
            {
                items = new List<Item>();
                if (count <= 0) { count = 1; }
                items.Add(new Item(moduleId) { count = count });
            }
            public AddItem(IList<Item> items)
            {
                this.items = items;
            }
        }
        public struct SubItem
        {
            public IList<Item> items;
            public SubItem(Item item) : this()
            {
                this.items = new List<Item>() { item };
            }
            public SubItem(IList<Item> item) : this()
            {
                this.items = item;
            }
            public SubItem(int moduleId, int count)
            {
                var item = new Item(moduleId) { count = count };
                this.items = new List<Item>() { item };
            }
        }
        //public struct CommitTask
        //{
        //    public RMTask task;
        //    public CommitTask(RMTask task) : this()
        //    {
        //        this.task = task;
        //    }
        //}
        public struct AddSubHp
        {
            public int value;
            public bool fixValue;
            public AddSubHp(int value, bool fixValue = true) : this()
            {
                this.value = value;
                this.fixValue = fixValue;
            }
        }
        public struct AddExp
        {
            public int exp;
            public AddExp(int exp) : this()
            {
                this.exp = exp;
            }
        }
        public struct AddTower
        {
            public Grid node;
            public int moduleId;
            public AddTower(Grid node, int moduleId) : this()
            {
                this.node = node;
                this.moduleId = moduleId;
            }
        }
    }
}
