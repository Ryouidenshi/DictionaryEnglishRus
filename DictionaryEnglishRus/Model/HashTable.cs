using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryEnglishRus.Model
{

    public class HashTable
    {
        public long MaxCount { get; set; }
        public Word[] Values { get; set; }

        public HashTable(long maxCount)
        {
            MaxCount = maxCount;
            Values = new Word[maxCount];
        }

        public bool Insert(Word word)
        {
            if (word.Key == null)
            {
                throw new Exception();
            }
            long hash = GetHash(word.Key);
            for (long i = hash; i < Values.Length; i++)
            {
                if (Values[i] == null)
                {
                    Values[i] = word;
                    return true;
                }
            }
            return false;
        }

        public bool Remove(string key)
        {
            long hash = GetHash(key);
            for (long i = hash; i < Values.Length; i++)
            {
                if (Values[i] != null && key.Equals(Values[i].Key))
                {
                    Values[i] = null;
                    return true;
                }
            }
            return false;
        }

        public bool Search(string movieName, out Word found)
        {
            long hash = GetHash(movieName);
            for (long i = hash; i < Values.Length; i++)
            {
                if (Values[i] != null && movieName.Equals(Values[i].Key))
                {
                    found = Values[i];
                    return true;
                }
            }
            found = default;
            return false;
        }

        public bool Search(string movieName)
        {
            long hash = GetHash(movieName);
            for (long i = hash; i < Values.Length; i++)
            {
                if (Values[i] != null && movieName.Equals(Values[i].Key))
                {
                    return true;
                }
            }
            return false;
        }

        public long GetHash(string key)
        {
            long hash = 0;
            var koef = 4228;
            foreach (char ch in key)
            {
                hash += (long)(Convert.ToInt16(ch) * koef);
            }
            while (hash > MaxCount)
                hash /= 2;
            return hash;
        }

        public class Word : IEquatable<Word>
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public Word(string key, string value)
            {
                Value = value;
                Key = key;
            }

            public bool Equals(Word other)
            {
                return Key == other.Key
                    && Value == other.Value;
            }
        }
    }
}
