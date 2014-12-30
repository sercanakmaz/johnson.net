using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;

namespace JohnsonNet.Config
{
    public class Helpers
    {
        public static T Default<T>(T val, T def)
        {
            if (((object)val) == null)
                return def;
            else if (val.Equals(default(T)))
                return def;
            else if (typeof(T) == typeof(string))
            {
                if (string.IsNullOrEmpty(val.ToString()))
                    return def;
                else return val;
            }
            else return val;
        }
        public static string Default<T>(string format, T val, string def)
        {
            if (((object)val) == null)
                return def;
            else if (val.Equals(default(T)))
                return def;
            else if (typeof(T) == typeof(string))
            {
                if (string.IsNullOrEmpty(val.ToString()))
                    return def;
                else return string.Format(format, val);
            }
            else return string.Format(format, val);
        }
        public static T ConvertObject<T>(object val, T def = default(T), string cultureInfo = null)
        {
            if (val == null) return Default(default(T), def);
            CultureInfo info = string.IsNullOrEmpty(cultureInfo) ? System.Threading.Thread.CurrentThread.CurrentCulture : new CultureInfo(cultureInfo);

            Func<Type, T> ConvertAction = (t) =>
            {
                try
                {
                    if (t == typeof(Guid))
                        return (T)Convert.ChangeType(Guid.Parse((string)val), t, info);
                    return (T)Convert.ChangeType(val, t, info);
                }
                catch { return default(T); }
            };

            T result;
            if (typeof(T).FullName.Contains(typeof(Nullable).FullName))
                result = ConvertAction(Nullable.GetUnderlyingType(typeof(T)));
            else result = ConvertAction(typeof(T));


            return Default(result, def);
        }

        public static DbConnection GetConnection(ConnectionStringSettings setting)
        {
            var factory = DbProviderFactories.GetFactory(setting.ProviderName);
            var dbConnection = factory.CreateConnection();

            dbConnection.ConnectionString = setting.ConnectionString;
            return dbConnection;
        }
    }
}
