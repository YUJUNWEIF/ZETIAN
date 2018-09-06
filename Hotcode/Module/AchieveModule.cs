using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public struct Achieve
    {
        public const int learningWord = 1;
        public const int levelScore = 2;
        public const int levelStar = 3;

        public int id;
        public int lv;
        public int value;

        public bool NotMax()
        {
            var achieveCfg = geniusbaby.tab.achievement.Inst().Find(id);
            return lv < achieveCfg.subs.Count;
        }
    }
    public struct AchieveStatistic
    {
        public int curScore;
    }
    
    public class AchieveModule : Singleton<AchieveModule>, IModule, Util.ISerializable
    {
        public List<Achieve> achieves = new List<Achieve>();
        public Util.ParamActions onAchieveSync = new Util.ParamActions();
        public Util.Param2Actions<Achieve, Achieve> onAchieveUpdate = new Util.Param2Actions<Achieve, Achieve>();
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter()
        {
            KnowledgeModule.Inst().onKnowledgeSync.Add(OnKnowledgeSync);
        }
        public void OnMainExit()
        {
            KnowledgeModule.Inst().onKnowledgeSync.Rmv(OnKnowledgeSync);
            achieves.Clear();
        }
        public Util.OutStream Marsh(Util.OutStream os)
        {
            os.WriteInt32(achieves.Count);
            for (int index = 0; index < achieves.Count; ++index)
            {
                os.WriteInt32(achieves[index].id);
                os.WriteInt32(achieves[index].value);
            }
            return os;
        }
        public Util.InStream Unmarsh(Util.InStream os)
        {
            var count = os.ReadInt32();
            for (int index = 0; index < count; ++index)
            {
                var achieve = new Achieve();
                achieve.id = os.ReadInt32();
                achieve.value = os.ReadInt32();
                achieves.Add(achieve);
            }
            return os;
        }
        void OnKnowledgeSync()
        {
            for (int index = 0; index < achieves.Count; ++index)
            {
                var achieve = achieves[index];
                if (achieve.NotMax() && achieve.id == Achieve.learningWord)
                {
                    achieve.value = KnowledgeModule.Inst().learnedWord;
                    achieves[index] = achieve;
                    ArchiveModule.Inst().SetNeedUpdateFlag();
                }
            }
        }
        public void StatisticLevelScore(int value)
        {
            for(int index = 0; index < achieves.Count; ++index)
            {
                var achieve = achieves[index];
                if (achieve.NotMax() && achieve.id == Achieve.levelScore)
                {
                    if (value > achieve.value)
                    {
                        achieve.value = value;
                        achieves[index] = achieve;

                        ArchiveModule.Inst().SetNeedUpdateFlag();
                    }
                }
            }
        }
        public void Sync(List<Achieve> ups)
        {
            var achieveCfgs = geniusbaby.tab.achievement.Inst().RecordArray;
            for (int index = 0; index < achieveCfgs.Count; ++index)
            {
                if (!achieves.Exists(it => it.id == achieveCfgs[index].id))
                {
                    achieves.Add(new Achieve() { id = achieveCfgs[index].id });
                }
            }

            for (int index = 0; index < ups.Count; ++index)
            {
                var exist = achieves.FindIndex(it => it.id == ups[index].id);
                if (exist >= 0)
                {
                    var temp = achieves[exist];
                    temp.lv = ups[index].lv;
                    achieves[exist] = temp;
                }
            }
            onAchieveSync.Fire();
        }
        public void Update(List<Achieve> ups)
        {
            for (int index = 0; index < ups.Count; ++index)
            {
                var exist = achieves.FindIndex(it => it.id == ups[index].id);
                if (exist >= 0)
                {
                    var old = achieves[exist];

                    var up = achieves[exist];
                    up.lv = ups[index].lv;
                    onAchieveUpdate.Fire(achieves[exist] = up, old);
                }
            }
        }
        public void ClientUpdate(AchieveStatistic statistic)
        {
            var index = achieves.FindIndex(it => it.id == 2);
            if (statistic.curScore > achieves[index].value)
            {
                var achieve = achieves[index];
                achieve.value = statistic.curScore;
                achieves[index] = achieve;
            }
        }
        public Achieve Find(int achieveId)
        {
            return achieves.Find(it => it.id == achieveId);
        }
    }
}