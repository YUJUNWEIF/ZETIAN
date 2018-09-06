//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.IO;
//using System.Reflection;
//using Mono.Data.Sqlite;

//namespace geniusbaby.tab
//{
//    public class SQLiteInterface : IDisposable
//    {
//        private SqliteConnection m_connection;
//        private SqliteTransaction m_transact;
//        public void Start(string url)
//        {
//            try
//            {
//                m_connection = new SqliteConnection(url);
//                m_connection.Open();
//                m_transact = m_connection.BeginTransaction();
//            }
//            catch (System.Exception ex)
//            {
//                Util.Logger.Instance.Error(ex.Message);
//                Stop();
//            }
//        }
//        public void Stop()
//        {
//            if (m_transact != null)
//            {
//                try { m_transact.Commit(); }
//                catch (System.Exception) { m_transact.Rollback(); }
//            }
//            if (m_connection != null)
//            {
//                m_connection.Close();
//                m_connection = null;
//            }
//        }
//        public void Dispose()
//        {
//            Stop();
//        }
//        void TraversalTypeFields(Type classType, System.Action<FieldInfo> act, params string[] ignores)
//        {
//            var dumpClass = Attribute.IsDefined(classType, typeof(DumpClassAttribute));
//            var fields = classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//            for (int index = 0; index < fields.Length; ++index)
//            {
//                var field = fields[index];

//                if (Array.Exists(ignores, it => it == field.Name)) { continue; }
//                if (dumpClass && !Attribute.IsDefined(field, typeof(NonDumpFieldAttribute)) ||
//                    !dumpClass && Attribute.IsDefined(field, typeof(DumpFieldAttribute)))
//                {
//                    act(field);
//                }
//                //if (field.FieldType.IsPrimitive ||
//                //    field.FieldType.IsEnum ||
//                //    field.FieldType == typeof(string))
//                //{
  
//                //}
//                //else
//                //{
//                //    TraversalTypeFields(field.FieldType, act, ignores);
//                //}
//            }
//        }
//        void TraversalObjFields(Object obj, System.Action<FieldInfo, Object> act, params string[] ignores)
//        {
//            var dumpClass = Attribute.IsDefined(obj.GetType(), typeof(DumpClassAttribute));
//            var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//            for (int index = 0; index < fields.Length; ++index)
//            {
//                var field = fields[index];
//                if (Array.Exists(ignores, it => it == field.Name)) { continue; }

//                if (field.FieldType.IsPrimitive ||
//                    field.FieldType.IsEnum ||
//                    field.FieldType == typeof(string))
//                {
//                    if (dumpClass && !Attribute.IsDefined(field, typeof(NonDumpFieldAttribute)) ||
//                        !dumpClass && Attribute.IsDefined(field, typeof(DumpFieldAttribute)))
//                    {
//                        act(field, field.GetValue(obj));
//                    }
//                }
//                else
//                {
//                    act(field, DeJson.Serializer.Serialize(field.GetValue(obj)));
//                }
//            }
//        }
//        void ConditionConnect(StringBuilder sb, string[] fieldNames)
//        {
//            sb.Append(@" WHERE ");
//            bool first = true;
//            for (int index = 0; index < fieldNames.Length; ++index)
//            {
//                var fieldName = fieldNames[index];
//                sb.Append(first ? string.Empty : @" AND ");
//                sb.Append(fieldName);
//                sb.Append(@" = @");
//                sb.Append(fieldName);
//                first = false;
//            }
//        }
//        public void CreateDb(string db)
//        {
//            using (SqliteCommand cmd = new SqliteCommand(@"CREATE DATABASE IF NOT EXISTS @db", m_connection, m_transact))
//            {
//                cmd.Parameters.AddWithValue(@"db", db);
//                cmd.ExecuteNonQuery();
//            }
//        }
//        public bool ExistsDb(string name)
//        {
//            string exists = "SELECT * FROM SQLITE_MASTER WHERE TYPE = @type AND NAME = @name";
//            using (SqliteCommand cmd = new SqliteCommand(exists, m_connection, m_transact))
//            {
//                cmd.Parameters.AddWithValue(@"type", @"TABLE");
//                cmd.Parameters.AddWithValue(@"name", name);
//                using (SqliteDataReader sdr = cmd.ExecuteReader())
//                {
//                    return sdr != null && sdr.Read();
//                }
//            }
//        }
//        public void CreateTable<T>()
//        {
//            var sb = new StringBuilder();
//            sb.Append(@" CREATE TABLE IF NOT EXISTS ");
//            sb.Append(typeof(T).Name);
//            sb.Append(@" (");
//            bool isFirst = true;
//            TraversalTypeFields(typeof(T),
//                (field) =>
//                {
//                    sb.Append(isFirst ? ' ' : ',');
//                    sb.Append(field.Name);
//                    if (field.FieldType == typeof(int) ||
//                        field.FieldType == typeof(long) ||
//                        field.FieldType == typeof(bool))
//                    {
//                        sb.Append(@" integer DEFAULT 0");
//                    }
//                    else if (field.FieldType == typeof(float) ||
//                        field.FieldType == typeof(double))
//                    {
//                        sb.Append(@" real DEFAULT 0");
//                    }
//                    else if (field.FieldType == typeof(string))
//                    {
//                        sb.Append(@" text");
//                    }
//                    else
//                    {
//                        sb.Append(@" text");
//                    }
//                    if (isFirst) { sb.Append(@" PRIMARY KEY "); }
//                    isFirst = false;
//                });
//            sb.Append(@" )");
//            using (SqliteCommand cmd = new SqliteCommand(sb.ToString(), m_connection, m_transact))
//            {
//                cmd.ExecuteNonQuery();
//            }
//        }
//        public void DropTable<T>()
//        {
//            var sb = new StringBuilder();
//            sb.Append(@" DROP TABLE IF EXISTS ");
//            sb.Append(typeof(T).Name);
//            using (SqliteCommand cmd = new SqliteCommand(sb.ToString(), m_connection, m_transact))
//            {
//                cmd.ExecuteNonQuery();
//            }
//        }
//        public bool ExistsTable(string name)
//        {
//            string exists = "SELECT * FROM SQLITE_MASTER WHERE TYPE = @type AND NAME = @name";
//            using (SqliteCommand cmd = new SqliteCommand(exists, m_connection, m_transact))
//            {
//                cmd.Parameters.AddWithValue(@"type", @"TABLE");
//                cmd.Parameters.AddWithValue(@"name", name);
//                using (SqliteDataReader sdr = cmd.ExecuteReader())
//                {
//                    return sdr != null && sdr.Read();
//                }
//            }
//        }
//        public List<T> SelectAll<T>() where T : new()
//        {
//            return Select<T>(default(T));
//        }
//        public List<T> Select<T>(T data, params string[] fieldNames) where T : new()
//        {
//            List<T> result = new List<T>();
//            var sb = new StringBuilder();
//            sb.Append(@" SELECT ");
//            bool isFirst = true;
//            TraversalTypeFields(typeof(T),
//                (field) =>
//                {
//                    sb.Append(isFirst ? ' ' : ',');
//                    sb.Append(field.Name);
//                    isFirst = false;
//                });
//            sb.Append(@" FROM ");
//            sb.Append(typeof(T).Name);
//            if (fieldNames.Length > 0) { ConditionConnect(sb, fieldNames); }

//            try
//            {
//                using (SqliteCommand cmd = new SqliteCommand(sb.ToString(), m_connection, m_transact))
//                {
//                    if (fieldNames.Length > 0)
//                    {
//                        for (int index = 0; index < fieldNames.Length; ++index)
//                        {
//                            var field = typeof(T).GetField(fieldNames[index]);
//                            cmd.Parameters.AddWithValue(field.Name, field.GetValue(data));
//                        }
//                    }
//                    using (SqliteDataReader sdr = cmd.ExecuteReader())
//                    {
//                        while (sdr != null && sdr.Read())
//                        {
//                            var value = ParseValue(typeof(T), sdr);
//                            if (value != null) { result.Add((T)value); }
//                        }
//                    }
//                }
//            }
//            catch (System.Exception ex) { Util.Logger.Instance.Error(ex.Message); } 
//            return result;
//        }

//        private object ParseValue(Type t, SqliteDataReader sdr)
//        {
//            var obj = Activator.CreateInstance(t);
//            var fields = obj.GetType().GetFields();
//            for (int index = 0; index < fields.Length; ++index)
//            {
//                var field = fields[index];
//                if (Attribute.IsDefined(field, typeof(NonDumpFieldAttribute))) { continue; }

//                var value = sdr[field.Name];
//                if (value == null || value.GetType() == typeof(System.DBNull)) { continue; }

//                if (field.FieldType.IsPrimitive ||
//                    field.FieldType.IsEnum ||
//                    field.FieldType == typeof(string))
//                {
//                    if (field.FieldType == value.GetType())
//                    {
//                        field.SetValue(obj, value);
//                    }
//                    else if (field.FieldType == typeof(int))
//                    {
//                        if (value.GetType() == typeof(long))
//                        {
//                            field.SetValue(obj, (int)(long)value);
//                        }
//                        else { field.SetValue(obj, int.Parse(value.ToString())); }
//                    }
//                    else if (field.FieldType == typeof(bool))
//                    {
//                        if (value.GetType() == typeof(long))
//                        {
//                            field.SetValue(obj, 1 == (long)value);
//                        }
//                        else { field.SetValue(obj, true); }
//                    }
//                    else if (field.FieldType == typeof(Enum) || field.FieldType.BaseType == typeof(Enum))
//                    {
//                        if (value.GetType() == typeof(long))
//                        {
//                            field.SetValue(obj, (int)(long)value);
//                        }
//                        else if (value.GetType() == typeof(int))
//                        {
//                            field.SetValue(obj, value);
//                        }
//                        else if (value.GetType() == typeof(string))
//                        {
//                            field.SetValue(obj, Enum.Parse(field.FieldType, (string)value));
//                        }
//                        else
//                        {
//                            Util.Logger.Instance.Error(@"Not support type!" + t.Name + @"." + field.Name);
//                        }
//                    }
//                    else
//                    {
//                        Util.Logger.Instance.Error(@"Not support type!" + t.Name + @"." + field.Name);
//                    }
//                }
//                else
//                {
//                    field.SetValue(obj, DeJson.Deserializer.Deserialize((string)value, field.FieldType));
//                }
//            }
//            return obj;
//        }
//        public bool Exists(object data, params string[] fieldNames)
//        {
//            var sb = new System.Text.StringBuilder();
//            sb.Append(@" SELECT ");
//            bool isFirst = true;
//            TraversalTypeFields(data.GetType(), (field) =>
//                {
//                    sb.Append(isFirst ? ' ' : ',');
//                    sb.Append(field.Name);
//                    isFirst = false;
//                });
//            sb.Append(@" FROM ");
//            sb.Append(data.GetType().Name);
//            if (fieldNames.Length > 0) { ConditionConnect(sb, fieldNames); }

//            using (SqliteCommand cmd = new SqliteCommand(sb.ToString(), m_connection, m_transact))
//            {
//                if (fieldNames.Length > 0)
//                {
//                    for (int index = 0; index < fieldNames.Length; ++index)
//                    {
//                        var field = data.GetType().GetField(fieldNames[index]);
//                        cmd.Parameters.AddWithValue(field.Name, field.GetValue(data));
//                    }
//                }
//                using (SqliteDataReader sdr = cmd.ExecuteReader())
//                {
//                    return sdr != null && sdr.Read();
//                }
//            }
//        }
//        public void ReplaceInto(object obj, params string[] ignores)
//        {
//            var sb = new StringBuilder();
//            sb.Append(@" REPLACE INTO ");
//            sb.Append(obj.GetType().Name);
//            sb.Append(@"(");
//            bool isFirst = true;
//            TraversalTypeFields(obj.GetType(), (field) =>
//                {
//                    sb.Append(isFirst ? ' ' : ',');
//                    sb.Append(field.Name);
//                    isFirst = false;
//                }, ignores);
//            sb.Append(@") VALUES (");
//            isFirst = true;
//            TraversalTypeFields(obj.GetType(), (field) =>
//                {
//                    sb.Append(isFirst ? "" : @", ");
//                    sb.Append("@");
//                    sb.Append(field.Name);
//                    isFirst = false;
//                }, ignores);
//            sb.Append(')');
//            using (SqliteCommand cmd = new SqliteCommand(sb.ToString(), m_connection, m_transact))
//            {
//                TraversalObjFields(obj, (field, value) => cmd.Parameters.AddWithValue(field.Name, value), ignores);
//                cmd.ExecuteNonQuery();
//            }
//        }
//        public int InsertInto(object obj, string primaryKey)
//        {
//            ReplaceInto(obj, primaryKey);
//            var sb = new StringBuilder();
//            sb.Append(@" select last_insert_rowid() from ");
//            sb.Append(obj.GetType().Name);
//            using (SqliteCommand cmd = new SqliteCommand(sb.ToString(), m_connection, m_transact))
//            {
//                TraversalObjFields(obj, (field, value) => cmd.Parameters.AddWithValue(field.Name, value), primaryKey);
//                using (SqliteDataReader sdr = cmd.ExecuteReader())
//                {
//                    while (sdr != null && sdr.Read())
//                    {
//                        return (int)sdr[primaryKey];
//                    }
//                }
//            }
//            return -1;
//        }
//        public void Delete(object data, params string[] fieldNames)
//        {
//            var sb = new StringBuilder();
//            sb.Append(@" DELETE FROM ");
//            sb.Append(data.GetType().Name);
//            if (fieldNames.Length > 0) { ConditionConnect(sb, fieldNames); }

//            using (SqliteCommand cmd = new SqliteCommand(sb.ToString(), m_connection, m_transact))
//            {
//                if (fieldNames.Length > 0)
//                {
//                    for (int index = 0; index < fieldNames.Length; ++index)
//                    {
//                        var field = data.GetType().GetField(fieldNames[index]);
//                        cmd.Parameters.AddWithValue(field.Name, field.GetValue(data));
//                    }
//                }
//                cmd.ExecuteNonQuery();
//            }
//        }
//    }
//}