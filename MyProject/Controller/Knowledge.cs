using System;
using System.Collections.Generic;

namespace geniusbaby
{
	public class __Math
	{
        private static List<int> number;
        private static List<int> add;
        private static List<int> multiply;        
        private static List<char> alphabet;
        private static List<int> searchMath;
        //private static List<cfg.englishRecord> searchEnglish;
        public static List<int> Number//0-10
        {
            get
            {
                if (number == null)
                {
                    number = new List<int>(11);
                    for (int index = 0; index <= 10; ++index) { number.Add(index); }
                }
                return number;
            }
        }
        public static List<int> Add
        {
            get
            {
                if (add == null)
                {
                    add = new List<int>(20);
                    for (int index = 0; index <= 9 + 9; ++index) { add.Add(index); }
                }
                return add;
            }
        }
        public static List<int> Multiply
        {
            get
            {
                if (multiply == null)
                {
                    multiply = new List<int>(50);
                    multiply.Add(0);
                    for (int p1 = 1; p1 <= 9; ++p1)
                    {
                        for (int p2 = 1; p2 <= p1; ++p2) { multiply.Add(p1 * p2); }
                    }
                }
                return multiply;
            }
        }
        public static List<char> Alphabet
        {
            get
            {
                if (alphabet == null)
                {
                    alphabet = new List<char>('z' - 'a' + 1);
                    for (char index = 'a'; index <= 'z'; ++index) { alphabet.Add(index); }
                }
                return alphabet;
            }
        }
        public static List<int> SearchMath
        {
            get
            {
                if (searchMath == null)
                {
                    searchMath = new List<int>(100);
                    for (int index = 0; index < 100; ++index) { searchMath.Add(index); }
                }
                return searchMath;
            }
        }
        //public static List<cfg.englishRecord> SearchEnglish
        //{
        //    get
        //    {
        //        if (searchEnglish == null)
        //        {
        //            searchEnglish = new List<cfg.englishRecord>(500);
        //            foreach (var word in cfg.englishTable.Instance.Records)
        //            {
        //                searchEnglish.Add(word.Value);
        //            }
        //        }
        //        return searchEnglish;
        //    }
        //}
	}
}
