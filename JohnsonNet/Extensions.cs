using JohnsonNet;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace System
{
    public static class Extensions
    {
        #region Collections
        public static IEnumerable<TItem> Extract<TList, TItem>(this IEnumerable<TList> obj
          , Func<TList, List<TItem>> field)
        {
            foreach (var item in obj)
            {
                foreach (var i in field(item))
                {
                    yield return i;
                }
            }
        }
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
        public static IEnumerable<IEnumerable<TSource>> Paged<TSource>(this IEnumerable<TSource> source, int pageSize)
        {
            int pageCount = (int)Math.Ceiling((double)source.Count() / (double)pageSize);
            for (int i = 1; i <= pageCount; i++)
            {
                yield return source.Page(i, pageSize);
            }
        }
        public static IEnumerable<TResult> Cursor<TSource, TResult>(this IEnumerable<TSource> obj, Func<TSource, TSource, TSource, TResult> action)
        {
            var Objects = obj.ToList();
            return obj.Select((Current, Index) =>
            {
                TSource Next = default(TSource);
                TSource Previous = default(TSource);
                try { Next = Objects[Index + 1]; }
                catch { }
                try { Previous = Objects[Index - 1]; }
                catch { }
                return action(Next, Current, Previous);
            });
        }
        #endregion
        #region Dictionary
        public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> obj, Dictionary<TKey, TValue> items)
        {
            foreach (var item in items)
            {
                obj.Add(item.Key, item.Value);
            }
            return obj;
        }
        #endregion
        #region String
        public static string CombineUri(this string uri1, string uri2)
        {
            if (string.IsNullOrEmpty(uri1)) return uri2;
            if (string.IsNullOrEmpty(uri2)) return uri1;

            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return string.Format("{0}/{1}", uri1, uri2);
        }
        public static bool EqualsWithCurrentCulture(this string text, string compareto)
        {
            return (text ?? string.Empty).Equals(compareto, StringComparison.CurrentCultureIgnoreCase);
        }
        public static bool ContainsWithCurrentCulture(this string text, string compareto)
        {
            return text.IndexOf(compareto, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
        public static string F(this string obj, params object[] args)
        {
            return string.Format(obj, args);
        }
        public static T ToNumeric<T>(this string obj)
        {
            if (string.IsNullOrEmpty(obj)) return default(T);
            return JohnsonManager.Convert.To<T>(Regex.Replace(obj, "[^.0-9]", ""));
        }
        /// <summary>
        /// string length'ten uzun ise kesilip sonuna addToLast eklenir.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="length">Kesilecek uzunluk</param>
        /// <param name="addToLast">Sona eklenecek paramatre</param>
        /// <returns></returns>
        public static string ToSummary(this string obj, int length = 300, string addToLast = null)
        {
            if (string.IsNullOrEmpty(obj)) return obj;
            return obj.Length > length ? obj.Substring(0, length) + (addToLast ?? string.Empty) : obj;
        }
        /// <summary>
        /// string'deki html elementlerini siler.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string RemoveHtml(this string obj)
        {
            return Regex.Replace(string.Format("{0}", obj ?? ""), "<.*?>", string.Empty);
        }
        #endregion
        #region Threading
        public static Dictionary<T, Thread> ExecuteAsync<T>(this IEnumerable<T> list, Action<T> action, int? threadCount = null, string name = "Worker")
        {
            Dictionary<T, Thread> result = new Dictionary<T, Thread>();
            threadCount = threadCount ?? Environment.ProcessorCount;
            int pageSize = (int)Math.Floor(list.Count() / (double)threadCount.Value);

            string nameFormat = name + "-{0}";
            int index = 0;

            foreach (var listPaged in list.Paged(pageSize))
            {
                var thread = JohnsonManager.MultiThread.ExecuteAsync((listPagedAsync) =>
                {
                    foreach (var item in listPagedAsync)
                    {
                        action(item);
                    }
                }, listPaged, string.Format(nameFormat, index));

                result.AddRange(listPaged.ToDictionary(p => p, c => thread));
                index++;
            }

            return result;
        }
        #endregion
        #region Reflection
        public static IEnumerable<Enum> GetFlags(this Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }
        public static T GetAttribute<T>(this MethodBase obj) where T : class
        {
            return obj.GetCustomAttributes(true).FirstOrDefault(p => p is T) as T;
        } 
        #endregion
    }
}
