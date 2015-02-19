using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JohnsonNet.Operation
{
    public class ConvertOperation
    {
        private object TryTo(Type type, object value, object defaultValue, string cultureInfo)
        {
            CultureInfo info = string.IsNullOrEmpty(cultureInfo) ? System.Threading.Thread.CurrentThread.CurrentCulture : new CultureInfo(cultureInfo);

            try
            {
                if (type == typeof(Guid))
                {
                    return Guid.Parse(value.ToString());
                }
                else if (type.IsEnum)
                {
                    return Enum.Parse(type, value.ToString());
                }
                else if (type == typeof(XElement))
                {
                    return XElement.Parse(value.ToString());
                }
                else if (type == typeof(Uri))
                {
                    return new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
                }

                return Convert.ChangeType(value, type, info);
            }
            catch
            {
                return Default(type, value, defaultValue);
            }
        }

        public object To(Type type, object value, object defaultValue = null, string cultureInfo = null)
        {
            if (value == null || value == DBNull.Value)
                return Default(type, value, defaultValue);

            Type underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType != null)
                return TryTo(underlyingType, value, defaultValue, cultureInfo);

            return TryTo(type, value, defaultValue, cultureInfo);
        }
        public object Default(Type type, object value, object defaultValue = null)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            if (value == null) return null;

            if (type == typeof(string))
            {
                if (string.IsNullOrEmpty(value.ToString()))
                    return defaultValue;

                else return value;
            }

            return null;
        }

        public T Default<T>(T val, T def)
        {
            return (T)Default(typeof(T), val, def);
        }
        public T To<T>(object val, T def = default(T), string cultureInfo = null)
        {
            object result = To(typeof(T), val, def, cultureInfo);

            return (T)result;
        }
    }
}
