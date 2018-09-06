using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace geniusbaby
{
    public struct PairValue
    {
        public int orign;
        public int total;
    }
    public struct StatisticData
    {
        public string playerId;
        public bool win;
        public PairValue fish;
        public PairValue mission;
        public PairValue rightAward;
        public PairValue wrongPunish;
        public PairValue timeAward;
        public int total;
        public List<int> missionIds;
        public IList<Statistic.FishKill> fishes;
        public IList<Statistic.WordReply> words;
    }
    public class Statistic
    {
        public class FishKill
        {
            public int moduleId;
            public int count;
        }
        public class WordReply : cfg.word
        {
            public int rightCount;
            public int wrongCount;
        }
        public Util.CVector<FishKill> fishes = new Util.CVector<FishKill>();
        public Util.CVector<WordReply> words = new Util.CVector<WordReply>();
        public void Reset()
        {
            fishes.Clear();
            words.Clear();
        }
        public void ReplyWord(int sessionId, string word, string translation)
        {
            if (sessionId != FightSceneManager.MySId()) { return; }
            ++GetReply(word, translation).rightCount;
        }
        public void WrongAlphabet(int sessionId, string word, string translation)
        {
            if (sessionId != FightSceneManager.MySId()) { return; }
            ++GetReply(word, translation).wrongCount;
        }
        WordReply GetReply(string word, string translation)
        {
            WordReply reply = words.Find(it => it.english == word);
            if (reply == null)
            {
                var me = FightSceneManager.Me();
                words.Add(reply = new WordReply() { english = word, rightCount = 0, wrongCount = 0 });
            }
            return reply;
        }
        public void KillNormalFish(int fishId)
        {
            var fish = fishes.Find(it => it.moduleId == fishId);
            if (fish == null) { fishes.Add(fish = new FishKill() { moduleId = fishId, count = 0 }); }
            ++fish.count;            
        }
    }
}
