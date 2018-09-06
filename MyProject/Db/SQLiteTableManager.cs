using System;
using System.Collections.Generic;
using System.IO;

namespace geniusbaby.archive
{
    public class SQLiteTableManager: Singleton<SQLiteTableManager>, IGameEvent
    {
        public const string save = "/gbconfig.db";
        class _Account : SQLiteTable<int, archive.Account>
        {
            public _Account(SQLite.SQLiteConnection storage) : base(storage) { }
        }
        class _KnowledgeFreq : SQLiteTable<string, archive.Knowledge>
        {
            public _KnowledgeFreq(SQLite.SQLiteConnection storage) : base(storage) { }
        }
        class _Archive : SQLiteTable<string, archive.Archive>
        {
            public _Archive(SQLite.SQLiteConnection storage) : base(storage) { }
        }
        public static SQLiteTable<int, archive.Account> account;
        public static SQLiteTable<string, archive.Archive> archive;
        public static SQLiteTable<string, archive.Knowledge> knowledgeFreq;

        SQLite.SQLiteConnection m_connection;
        public void OnStartGame()
        {
            string path = UnityEngine.Application.persistentDataPath + save;
            var flags = File.Exists(path) ? SQLite.SQLiteOpenFlags.ReadWrite : SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create;
            m_connection = new SQLite.SQLiteConnection(path, flags);

            account = new _Account(m_connection);
            archive = new _Archive(m_connection);
            knowledgeFreq = new _KnowledgeFreq(m_connection);
        }
        public void OnStopGame()
        {
            if (m_connection != null)
            {
                m_connection.Close();
                m_connection = null;
            }
        }
        public void Begin()
        {
            m_connection.BeginTransaction();
        }
        public void Commit()
        {
            m_connection.Commit();
        }
        public void Rollback()
        {
            m_connection.Rollback();
        }
    }
}