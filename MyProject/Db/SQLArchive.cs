using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

namespace geniusbaby.archive
{
    public class Account
    {
        [Util.PrimaryKey]
        public int key;
        public int channel;
        public string account;
        public string password;
    }
    public class Knowledge
    {
        public class Wrong
        {
            public string english;
            public string chinese;
            public int wrongs;
        }
        [Util.PrimaryKey]
        public string playerId;
        public List<Wrong> wrongs = new List<Wrong>();
    }
    public class Archive
    {
        [Util.PrimaryKey]
        public string playerId;
        public byte[] zipData;
    }
    public class SQLiteTable<K, V>
        where V : class, new()
    {
        public SQLite.SQLiteConnection db { get; private set; }
        public SQLiteTable(SQLite.SQLiteConnection db)
        {
            this.db = db;
            this.db.CreateTable<V>();
        }
        public bool Insert(K key, V value)
        {
            return db.Insert(value) > 0;
        }
        public int InsertPrimaryKeyAutoIncr(V value)
        {
            return 0;
        }
        public V Remove(K key)
        {
            V value = Get(key);
            if (key != null)
            {
                int count = db.Delete<V>(key);
                //return count > 0;
                return value;
            }
            return value;
        }
        public V Get(K key)
        {
            try
            {
                return db.Get<V>(key);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Update(K key, V value)
        {
            db.InsertOrReplace(value);
            return true;
        }
        public List<V> GetAll()
        {
            return db.GetAll<V>();
        }
        public void DeleteAll()
        {
            db.DeleteAll<V>();
        }
    }
}
