using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace JohnsonNet.Base
{
    public static class Core
    {
        #region ConvertObject
        public static object ConvertObject(Type type, object val, string cultureInfo = null)
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
        public static object Default(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
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
        #endregion
        #region Reflection
        #region ConvertClass
        public static TOutput ConvertClass<TInput, TOutput>(TInput entity)
            where TOutput : class
            where TInput : class
        {
            return ConvertClass<TInput, TOutput>(entity, null);
        }
        public static TOutput ConvertClass<TInput, TOutput>(TInput entity, IEnumerable<string> exceptPropertyNames)
            where TOutput : class
            where TInput : class
        {
            if (entity == null) return null;

            if (exceptPropertyNames == null) exceptPropertyNames = new List<string>();
            Type outputType = typeof(TOutput);
            Type inputType = typeof(TInput);
            TOutput result = (TOutput)System.Reflection.Assembly.GetAssembly(outputType).CreateInstance(outputType.FullName);
            foreach (var p in inputType.GetProperties())
            {
                if (exceptPropertyNames.Contains(p.Name)) continue;
                System.Reflection.PropertyInfo outP = outputType.GetProperty(p.Name);
                if (outP != null) outP.SetValue(result, p.GetValue(entity, null), null);
                else Debug.WriteLine("Çevrilme Sırasında Bulunamayan Özellik: " + p.Name);
            }
            return result;
        }
        public static object ConvertClass<TInput>(TInput entity, Type outputType)
          where TInput : class
        {
            return ConvertClass(entity, outputType, null);
        }

        public static object ConvertClass<TInput>(TInput entity, Type outputType, IEnumerable<string> exceptPropertyNames)
            where TInput : class
        {
            if (entity == null) return null;
            if (exceptPropertyNames == null) exceptPropertyNames = new List<string>();
            Type inputType = entity.GetType();
            object result = System.Reflection.Assembly.GetAssembly(outputType).CreateInstance(outputType.FullName);
            foreach (var p in inputType.GetProperties())
            {
                if (exceptPropertyNames.Contains(p.Name)) continue;
                System.Reflection.PropertyInfo outP = outputType.GetProperty(p.Name);
                if (outP != null) outP.SetValue(result, p.GetValue(entity, null), null);
                else Debug.WriteLine("Çevrilme Sırasında Bulunamayan Özellik: " + p.Name);
            }
            return result;
        }
        #endregion 
        #region Property
        public static bool IsPropertyExist(object obj, string name)
        {
            if (obj == null) return false;
            Type t = obj.GetType();

            if (t.Name.Contains("JsonObject"))
            {
                dynamic DynamicObj = obj;
                object value = null;

                if (DynamicObj.TryGetValue(name, out value))
                    return value != null;

                return false;
            }
            var p = obj.GetType().GetProperty(name);
            if (p == null) return false;
            else
            {
                object value = p.GetValue(obj, null);
                return value != null;
            }
        }
        public static T GetPropertyValue<T>(object obj, string name)
        {
            if (obj == null) return default(T);
            Type t = obj.GetType();

            var p = obj.GetType().GetProperty(name);
            if (p == null) return default(T);
            else
            {
                object value = p.GetValue(obj, null);
                return ConvertObject<T>(value);
            }
        }
        public static object GetPropertyValue(object obj, string name)
        {
            return GetPropertyValue<object>(obj, name);
        }
        #endregion
        #region ResourceStream
        public static string GetResourceStream(Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                 return reader.ReadToEnd();
            }
        }
        #endregion

        public static bool IsAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
        #endregion
        #region Threading
        public static Thread ExecuteAsync(Action action, string name = "Worker")
        {
            Thread thread = new Thread(() =>
            {
                try { action(); }
                catch { }
            })
            {
                CurrentCulture = Thread.CurrentThread.CurrentCulture,
                CurrentUICulture = Thread.CurrentThread.CurrentUICulture,
                Name = name
            };
            thread.Start();
            return thread;
        }
        public static Thread ExecuteAsync<T>(Action<T> action, T parameter, string name = "Worker")
        {
            Thread thread = new Thread((p) =>
            {
                try { action((T)p); }
                catch { }
            })
            {
                CurrentCulture = Thread.CurrentThread.CurrentCulture,
                CurrentUICulture = Thread.CurrentThread.CurrentUICulture,
                Name = name
            };
            thread.Start(parameter);
            return thread;
        }
        #endregion
    }
}
