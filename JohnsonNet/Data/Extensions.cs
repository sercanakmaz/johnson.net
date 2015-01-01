using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JohnsonNet.Data
{
    public static class Extensions
    {
        public static List<T> ToList<T>(this IDataReader reader)
        {
            var result = new List<T>();
            var itemType = typeof(T);
            var properties = itemType.GetProperties();

            while (reader.Read())
            {
                T item;

                if (itemType.IsPrimitive
                    || itemType == typeof(System.String))
                {
                    item = Core.ConvertObject<T>(reader.GetValue(0));
                }
                else
                {
                    item = Activator.CreateInstance<T>();
                    foreach (var prp in properties)
                    {
                        string fieldName = null;
                        fieldName = prp.GetCustomAttributes(true).Where(p => p is FieldMapAttribute).Select(p => (p as FieldMapAttribute).FieldName).FirstOrDefault();

                        if (string.IsNullOrEmpty(fieldName)) fieldName = prp.Name;

                        int fieldOrdinal = reader.GetFieldOrdinal(fieldName);

                        if (fieldOrdinal >= 0)
                        {
                            prp.SetValue(item, Core.ConvertObject(prp.PropertyType, reader.GetValue(fieldOrdinal)), null);
                        }
                    }
                }

                result.Add(item);
            }

            return result;
        }
        public static int GetFieldOrdinal(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }
            return -1;
        }
        public static ConnectionStringSettings ToConnectionStringSettings(this IDbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            var type = connection.GetType();
            string provider = null;

            switch (type.FullName)
            {
                case "System.Data.SqlClient.SqlConnection":
                    provider = "System.Data.SqlClient";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return new ConnectionStringSettings { ConnectionString = connection.ConnectionString, ProviderName = provider };
        }
        public static IDbConnection ToIDbConnection(this ConnectionStringSettings setting)
        {
            var factory = DbProviderFactories.GetFactory(setting.ProviderName);
            var dbConnection = factory.CreateConnection();

            dbConnection.ConnectionString = setting.ConnectionString;
            return dbConnection;
        }
    }
}
