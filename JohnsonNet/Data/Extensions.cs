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
        /// <summary>
        /// This method allows you to prepare a dictionary list with your entity. You can use this dictionary to send data to your database. And of cours its considers Ignore and FieldMap attributes. If a property has a Ignore attribute, it will not show up in the dictionary. Or if a property has a FieldMap attribute it will show up with mapped name.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ParamDictionary ToParamDictionary(this object obj)
        {
            if (obj == null) return null;

            var itemType = obj.GetType();
            var properties = JohnsonManager.Reflection.GetPropertiesWithoutHidings(itemType);
            var parameters = new ParamDictionary();

            foreach (var property in properties)
            {
                if (property.GetAttribute<IgnoreAttribute>() != null)
                    continue;

                string fieldName = null;
                fieldName = (property.GetAttribute<FieldMapAttribute>() ?? new FieldMapAttribute()).FieldName;

                if (string.IsNullOrEmpty(fieldName)) fieldName = property.Name;

                parameters.Add(fieldName, property.GetValue(obj, null));
            }

            return parameters;
        }
        /// <summary>
        /// It can convert your IDataReader to a entity list with considering FieldMap attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IDataReader reader)
        {
            var result = new List<T>();
            var itemType = typeof(T);
            var properties = JohnsonManager.Reflection.GetPropertiesWithoutHidings(itemType);

            while (reader.Read())
            {
                T item;

                if (itemType.IsPrimitive
                    || itemType == typeof(System.String))
                {
                    item = JohnsonManager.Convert.To<T>(reader.GetValue(0));
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
                            prp.SetValue(item, JohnsonManager.Convert.To(prp.PropertyType, reader.GetValue(fieldOrdinal)), null);
                        }
                    }
                }

                result.Add(item);
            }

            return result;
        }
        /// <summary>
        /// You can get columns ordinal in a IDataRecord.
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetFieldOrdinal(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// You can get ConnectionStringSettings object from a IDbConnection
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
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
        /// <summary>
        /// You can get IDbConnection object from a ConnectionStringSettings
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IDbConnection ToIDbConnection(this ConnectionStringSettings setting)
        {
            var factory = DbProviderFactories.GetFactory(setting.ProviderName);
            var dbConnection = factory.CreateConnection();

            dbConnection.ConnectionString = setting.ConnectionString;
            return dbConnection;
        }
    }
}
