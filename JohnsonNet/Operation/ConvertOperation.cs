using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace JohnsonNet.Operation
{
    public class ConvertOperation
    {
        #region ConvertObject
        public object To(Type type, object val, string cultureInfo = null)
        {
            if (val == null || val == DBNull.Value) return Default(type);
            CultureInfo info = string.IsNullOrEmpty(cultureInfo) ? System.Threading.Thread.CurrentThread.CurrentCulture : new CultureInfo(cultureInfo);

            Func<Type, object> ConvertAction = (currentType) =>
            {
                try
                {
                    if (currentType == typeof(Guid))
                    {
                        return Convert.ChangeType(Guid.Parse(val.ToString()), currentType, info);
                    }
                    else if (currentType.IsEnum)
                    {
                        return Enum.Parse(currentType, val.ToString());
                    }
                    return Convert.ChangeType(val, currentType, info);
                }
                catch { return Default(currentType); }
            };

            object result;
            if (type.FullName.Contains(typeof(Nullable).FullName))
                result = ConvertAction(Nullable.GetUnderlyingType(type));
            else result = ConvertAction(type);

            return result;
        }
        public object Default(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public T To<T>(object val, T def = default(T), string cultureInfo = null)
        {
            if (val == null) return Default(default(T), def);
            CultureInfo info = string.IsNullOrEmpty(cultureInfo) ? System.Threading.Thread.CurrentThread.CurrentCulture : new CultureInfo(cultureInfo);

            Func<Type, T> ConvertAction = (t) =>
            {
                try
                {
                    if (t == typeof(Guid))
                    {
                        return (T)Convert.ChangeType(Guid.Parse(val.ToString()), t, info);
                    }
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
        public T Default<T>(T val, T def)
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
        public string Default<T>(string format, T val, string def)
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
        #endregion
    }
}
