using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace JohnsonNet.Operation
{
    public class ReflectionOperation
    {
        #region ConvertClass
        public TOutput CloneClass<TInput, TOutput>(TInput entity)
            where TOutput : class
            where TInput : class
        {
            return CloneClass<TInput, TOutput>(entity, null);
        }
        public TOutput CloneClass<TInput, TOutput>(TInput entity, IEnumerable<string> exceptPropertyNames)
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
        public object CloneClass<TInput>(TInput entity, Type outputType)
          where TInput : class
        {
            return CloneClass(entity, outputType, null);
        }

        public object CloneClass<TInput>(TInput entity, Type outputType, IEnumerable<string> exceptPropertyNames)
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
        public bool IsPropertyExist(object obj, string name)
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
        public T GetPropertyValue<T>(object obj, string name)
        {
            if (obj == null) return default(T);
            Type t = obj.GetType();

            var p = obj.GetType().GetProperty(name);
            if (p == null) return default(T);
            else
            {
                object value = p.GetValue(obj, null);
                return JohnsonManager.Convert.To<T>(value);
            }
        }
        public object GetPropertyValue(object obj, string name)
        {
            return GetPropertyValue<object>(obj, name);
        }
        #endregion

        public bool IsAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}
