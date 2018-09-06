using System;
using System.Collections.Generic;
using System.Text;

namespace geniusbaby
{
    public class QuizPazzle
    {
        public int Key;
        public char Value;
        public QuizPazzle() { }
        public QuizPazzle(int k, char v)
        {
            this.Key = k;
            this.Value = v;
        }
    }
    public class EnglishSwap
    {
        List<int> tmpBuffer = new List<int>();
        QuizPazzle[] m_save;
        Util.FastRandom rand;

        public const int LENGTH = 4;
        public string original { get; private set; }
        public string translation { get; private set; }
        public List<QuizPazzle> pazzles = new List<QuizPazzle>();
        public Util.ParamActions onSync = new Util.ParamActions();

        public bool Swap(int a, int b)
        {
            var ai = pazzles.FindIndex(it => it.Key == a);
            var bi = pazzles.FindIndex(it => it.Key == b);
            if (ai < 0 || bi < 0) { return false; }

            var ap = pazzles[ai];
            var bp = pazzles[bi];
            pazzles[ai] = new QuizPazzle(ap.Key, bp.Value);
            pazzles[bi] = new QuizPazzle(bp.Key, ap.Value);
            onSync.Fire();
            return true;
        }
        public string tempAnswer
        {
            get
            {
                var sb = new StringBuilder();
                for (int index = 0; index < original.Length; ++index)
                {
                    var exist = pazzles.FindIndex(it => it.Key == index);
                    sb.Append((exist >= 0) ? pazzles[exist].Value : original[index]);
                }
                return sb.ToString();
            }
        }
        public bool isRightAnswer()
        {
            for (int index = 0; index < pazzles.Count; ++index)
            {
                if (original[pazzles[index].Key] != pazzles[index].Value) { return false; }
            }
            return true;
        }
        public void Active(string word, string translation, int pazzleCount = LENGTH)
        {
            this.original = word;
            this.translation = translation;
            this.pazzles.Clear();

            rand = new Util.FastRandom();
            pazzleCount = FEMath.Min(word.Length, LENGTH, pazzleCount);

            while (pazzleCount >= 2)
            {
                var endAt = original.Length - pazzleCount + 1;
                var mm = rand.RandomBetween(0, endAt, endAt);
                for (int index = 0; index < mm.Length; ++index)
                {
                    if (SelectP(index, pazzleCount))
                    {
                        m_save = pazzles.ToArray();
                        onSync.Fire();
                        return;
                    }
                }
                --pazzleCount;
            }
            Util.Logger.Instance.Error(word.ToString());
            Active("invalid", "非法的，不合法", LENGTH);
        }
        public void Restore()
        {
            pazzles.Clear();
            if (m_save != null)
            {
                for (int index = 0; index < m_save.Length; ++index) { pazzles.Add(m_save[index]); }
            }
            onSync.Fire();
        }
        bool SelectP(int startAt, int pazzleCount)
        {
            pazzles.Clear();
            for (int index = startAt; index < startAt + pazzleCount; ++index)
            {
                pazzles.Add(new QuizPazzle(index, original[index]));
            }
            for (int index = startAt; index < startAt + pazzleCount; ++index)
            {
                if (GetPazzleCount(original[index]) > pazzleCount / 2)
                {
                    return false;
                }
            }

            for (int index = 0; index < pazzles.Count; ++index)
            {
                var pazzle = pazzles[index];
                if (original[pazzle.Key] != pazzle.Value) { continue; }

                tmpBuffer.Clear();
                for (int j = 0; j < pazzles.Count; ++j)
                {
                    if (pazzle.Value == pazzles[j].Value //相同不对调
                        || pazzle.Value == original[pazzles[j].Key])//对调成为答案
                    { continue; }
                    tmpBuffer.Add(j);
                }
                var swapIndex = tmpBuffer[rand.Next(tmpBuffer.Count)];
                var swap = pazzles[swapIndex];
                var temp = pazzle;
                pazzles[index] = new QuizPazzle(pazzle.Key, swap.Value);
                pazzles[swapIndex] = new QuizPazzle(swap.Key, temp.Value);
            }

            for (int index = 0; index < pazzles.Count; ++index)
            {
                var pazzle = pazzles[index];
                if (original[pazzle.Key] == pazzle.Value) { return false; }
            }
            return true;
        }
        int GetPazzleCount(char ch)
        {
            int count = 0;
            pazzles.ForEach(it =>
            {
                if (it.Value == ch)
                    ++count;
            });
            return count;
        }
        public bool Valid
        {
            get
            {
                return !string.IsNullOrEmpty(original) && !string.IsNullOrEmpty(translation) && m_save != null;
            }
        }
        bool _Valid(int index, int conj, int pazzle)
        {
            int valid = 0;
            for (int i = index; i < index + conj; ++i)
            {
                bool dup = false;
                for (int j = index; j < i; ++j)
                {
                    if (original[i] == original[j])
                    {
                        dup = true;
                        break;
                    }
                }
                if (!dup)
                {
                    ++valid;
                }
            }
            return valid >= pazzle;
        }
    }
}