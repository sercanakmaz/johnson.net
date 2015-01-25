using JohnsonNet.Operation;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace JohnsonNet.Data.Linq
{
    public static class Extensions
    {
        #region Save
        public static T Save<T>(this Table<T> obj, T entity) where T : class
        {
            return Save(obj, entity, null, null);
        }
        public static T Save<T>(this Table<T> obj, ParamDictionary parameters) where T : class
        {
            return Save(obj, null, parameters, null);
        }
        private static T Save<T>(this Table<T> obj, T entity, ParamDictionary parameters, string procName) where T : class
        {
            if (procName == null) procName = obj.GetProcName();
            if (parameters == null) parameters = obj.GetParams(entity);

            SqlConnection cnn = (SqlConnection)obj.Context.Connection;

            bool cnnClosed = cnn.State == System.Data.ConnectionState.Closed;
            if (cnnClosed) cnn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandTimeout = obj.Context.CommandTimeout
            })
            {
                if (parameters != null) foreach (var item in parameters) cmd.Parameters.AddWithValue(item.Key, item.Value);
                object result = cmd.ExecuteScalar();
                if (entity != null)
                {
                    PropertyInfo pi = entity.GetType().GetProperties().FirstOrDefault(
                        p => p.GetCustomAttributes(true).FirstOrDefault((a) =>
                        {
                            System.Data.Linq.Mapping.ColumnAttribute c = a as System.Data.Linq.Mapping.ColumnAttribute;
                            if (c == null) return false;
                            else return c.IsPrimaryKey;
                        }) != null);
                    if (result != null && result != DBNull.Value && pi != null)
                        pi.SetValue(entity, result, null);
                }
            }

            if (cnnClosed) cnn.Close();

            return entity;
        }
        public static IEnumerable<T> Save<T>(this Table<T> obj, IEnumerable<T> entities) where T : class
        {
            using (SqlConnection cnn = new SqlConnection(obj.Context.Connection.ConnectionString))
            {
                bool cnnClosed = cnn.State == System.Data.ConnectionState.Closed;
                if (cnnClosed) cnn.Open();
                foreach (var item in entities) obj.Save(item);
                if (cnnClosed) cnn.Close();
            }
            return entities;
        }
        public static IEnumerable<T> Save<T>(this Table<T> obj, IEnumerable<ParamDictionary> entities) where T : class
        {
            List<T> result = new List<T>();
            if (entities.Count() == 0) return result;
            using (SqlConnection cnn = new SqlConnection(obj.Context.Connection.ConnectionString))
            {
                bool cnnClosed = cnn.State == System.Data.ConnectionState.Closed;
                if (cnnClosed) cnn.Open();
                foreach (var item in entities) result.Add(obj.Save(item));
                if (cnnClosed) cnn.Close();
            }
            return result;
        }
        public static IEnumerable<T> SaveBulkXml<T>(this Table<T> obj, IEnumerable<T> entities, ParamDictionary AdditionalParameters = null) where T : class
        {
            return SaveBulkXml(obj, entities: entities.Select(p => obj.GetParams(p)), AdditionalParameters: AdditionalParameters);
        }
        public static List<T> SaveBulkXml<T>(this Table<T> obj, IEnumerable<ParamDictionary> entities, string procName = null, ParamDictionary AdditionalParameters = null) where T : class
        {
            List<T> results = new List<T>();
            //if (entities.Count() == 0) return results;
            XElement Root = new XElement("Root", entities.Select(p => p.ToXElement()));
            if (procName == null) procName = obj.GetProcName("SaveXml");

            SqlConnection cnn = (SqlConnection)obj.Context.Connection;

            bool cnnClosed = cnn.State == System.Data.ConnectionState.Closed;
            if (cnnClosed) cnn.Open();

            using (SqlCommand cmd = new SqlCommand(procName, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandTimeout = obj.Context.CommandTimeout
            })
            {
                if (AdditionalParameters != null) foreach (var item in AdditionalParameters) cmd.Parameters.AddWithValue(item.Key, item.Value);
                cmd.Parameters.AddWithValue("BulkXml", Root.ToString(SaveOptions.DisableFormatting));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        try { results = obj.Context.Translate<T>(reader).ToList(); }
                        catch { }
                    }
                }
            }

            if (cnnClosed) cnn.Close();
            return results;
        }
        public static string CreateSaveProcedure<T>(this Table<T> obj) where T : class
        {
            string result = null
                 , parameters = null
                 , updateLines = null
                 , insertLines = null
                 , whereClause = null;

            Type t = typeof(T);
            TableAttribute tableAttribute = (TableAttribute)t.GetCustomAttributes(true).First(a => a.GetType() == typeof(System.Data.Linq.Mapping.TableAttribute));
            Dictionary<string, string> columns = new Dictionary<string, string>()
                , primaryKeyColumns = new Dictionary<string, string>();

            foreach (var p in t.GetProperties())
            {
                ColumnAttribute c = p.GetCustomAttributes(true)
                    .FirstOrDefault(a => a.GetType() == typeof(ColumnAttribute)) as ColumnAttribute;

                if (c == null) continue;

                string columnName = JohnsonManager.Convert.Default(c.Name, p.Name).Trim()
                    , dbType = ReplaceDbType(c.DbType).Trim();

                if (c.IsPrimaryKey) primaryKeyColumns.Add(columnName, dbType);
                if (c != null ? c.IsDbGenerated : true) continue;

                columns.Add(columnName, dbType);
            }

            int maxLength = columns.Select(p => p.Key.Length).Max();
            whereClause = string.Join(" AND ", primaryKeyColumns.Select(p => string.Format("{0} = @{0}", p.Key)));
            parameters = string.Join("," + Environment.NewLine, primaryKeyColumns.Union(columns).Select(p => string.Format("\t@{0}{1} = NULL", p.Key.PadRight(maxLength + 1, ' '), p.Value)));
            updateLines = string.Format("\t\tUPDATE\t{0}\r\n\t\tSET\t\t{1}\r\n\t\tWHERE\t{2}"
                , tableAttribute.Name
                , string.Join("," + Environment.NewLine + "\t\t\t\t", columns.Select(p => string.Format("{0} = ISNULL(@{0}, {0})", p.Key)))
                , whereClause);
            insertLines = string.Format("\t\tINSERT INTO {0} ({1})\r\n\t\tVALUES ({2})", tableAttribute.Name, string.Join(",", columns.Select(p => p.Key)), string.Join(",", columns.Select(p => "@" + p.Key)));
            result = string.Format("CREATE PROC {0}\r\n(\r\n{1}\r\n)\r\nAS\r\nBEGIN\r\n\tIF EXISTS(SELECT {2} FROM {3} WHERE {4})\r\n\tBEGIN\r\n{5}\r\n\tEND\r\n\tELSE\r\n\tBEGIN\r\n{6}\r\n\tEND\r\nEND"
                , obj.GetProcName(), parameters, string.Join(",", primaryKeyColumns.Select(p => p.Key)), tableAttribute.Name, whereClause, updateLines, insertLines);
            return result;
        }
        private static string ReplaceDbType(string storage)
        {
            return storage.Replace("NOT NULL", string.Empty).Replace("IDENTITY", string.Empty);
        }

        #endregion

        public static List<T> ExecuteProcedure<T>(this DataContext obj, string procName, ParamDictionary parameters = null)
        {
            var o = new DataOperation(obj.Connection.ToConnectionStringSettings());
            return o.Execute<T>(procName, parameters);
        }

        public static void ReAttach<T>(this Table<T> obj, T entity, bool asModified = false) where T : class
        {
            obj.Attach(JohnsonManager.Reflection.CloneClass<T, T>(entity), asModified);
        }

        public static ParamDictionary GetParams<T>(this Table<T> obj, T entity) where T : class
        {
            ParamDictionary result = new ParamDictionary();
            Type t = typeof(T);

            foreach (var p in t.GetProperties())
            {
                ColumnAttribute c = p.GetCustomAttributes(true).FirstOrDefault(a =>
                    a.GetType() == typeof(ColumnAttribute)) as ColumnAttribute;

                if (c != null ? !c.IsPrimaryKey && c.IsDbGenerated : true) continue;

                Object ParamValue = p.GetValue(entity, null);
                result.Add(p.Name, ParamValue);
            }

            return result;
        }
        public static string GetProcName<T>(this Table<T> obj, string name = "Save") where T : class
        {
            string[] result = null;
            Type t = typeof(T);
            CustomAttributeData ta = t.GetCustomAttributesData().FirstOrDefault();
            result = ta.NamedArguments.First().TypedValue.Value.ToString().Split('.');

            return string.Format("{0}.{1}", result[0], result[1].Insert(result[1].StartsWith("[") ? 1 : 0, name));
        }
    }
}
