using JohnsonNet;
using JohnsonNet.WebPages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace System.Web.WebPages
{
    public static class Extensions
    {
        public static T PageData<T>(this WebPage obj, string key, T def = default(T))
        {
            if (typeof(T).IsValueType)
            {
                return JohnsonManager.Convert.To<T>(obj.PageData[key], def);
            }
            return JohnsonManager.Convert.Default((T)obj.PageData[key], def);
        }
        public static string PageData(this WebPage obj, string key, string def = null)
        {
            return JohnsonManager.Convert.To<string>(obj.PageData[key], def);
        }

        public static T Session<T>(this WebPage obj, string key, T def = default(T))
        {
            if (typeof(T).IsValueType)
            {
                return JohnsonManager.Convert.To<T>(obj.Session[key], def);
            }
            return JohnsonManager.Convert.Default((T)obj.Session[key], def);
        }
        public static string Session(this WebPage obj, string key, string def = null)
        {
            return JohnsonManager.Convert.To<string>(obj.Session[key], def);
        }

        public static T RequestValue<T>(this WebPage obj, string key, T def = default(T))
        {
            return JohnsonManager.Convert.To<T>(obj.Context.Request[key], def);
        }
        public static T RequestValue<T>(this HttpContextBase context, string key, T def = default(T))
        {
            return JohnsonManager.Convert.To<T>(context.Request[key], def);
        }
        public static T RequestValue<T>(this HttpContext context, string key, T def = default(T))
        {
            return JohnsonManager.Convert.To<T>(context.Request[key], def);
        }
        public static string RequestValue(this System.Web.WebPages.WebPage obj, string key, string def = null)
        {
            return JohnsonManager.Convert.To<string>(obj.Context.Request[key], def);
        }

        public static T GetRouteValue<T>(this System.Web.WebPages.WebPage obj, string key, T def = default(T))
        {
            RouteValueDictionary routes = obj.Context.Items["__Route"] as RouteValueDictionary;
            if (routes == null) return default(T);
            return JohnsonManager.Convert.To<T>(routes[key], def);
        }
        public static T GetRouteValue<T>(this HttpContextBase context, string key, T def = default(T))
        {
            RouteValueDictionary routes = context.Items["__Route"] as RouteValueDictionary;
            if (routes == null) return default(T);
            return JohnsonManager.Convert.To<T>(routes[key], def);
        }
        public static T GetRouteValue<T>(this HttpContext context, string key, T def = default(T))
        {
            RouteValueDictionary routes = context.Items["__Route"] as RouteValueDictionary;
            if (routes == null) return default(T);
            return JohnsonManager.Convert.To<T>(routes[key], def);
        }
        public static string GetRouteValue(this HttpContextBase obj, string key, string def = null)
        {
            return obj.GetRouteValue<string>(key, def);
        }
        public static string GetRouteValue(this HttpContext obj, string key, string def = null)
        {
            return obj.GetRouteValue<string>(key, def);
        }
        public static string GetRouteValue(this System.Web.WebPages.WebPage obj, string key, string def = null)
        {
            return obj.GetRouteValue<string>(key, def);
        }

        public static T GetLastRouteValue<T>(this HttpContextBase context, T def = default(T))
        {
            return context.GetRouteValue<T>("Permalink5", context.GetRouteValue<T>("Permalink4", context.GetRouteValue<T>("Permalink3", context.GetRouteValue<T>("Permalink2", context.GetRouteValue<T>("Permalink1")))));
        }
        public static T GetLastRouteValue<T>(this HttpContext context, T def = default(T))
        {
            return context.GetRouteValue<T>("Permalink5", context.GetRouteValue<T>("Permalink4", context.GetRouteValue<T>("Permalink3", context.GetRouteValue<T>("Permalink2", context.GetRouteValue<T>("Permalink1")))));
        }
        public static T GetLastRouteValue<T>(this System.Web.WebPages.WebPage obj, T def = default(T))
        {
            return obj.Context.GetRouteValue<T>("Permalink5", obj.Context.GetRouteValue<T>("Permalink4", obj.Context.GetRouteValue<T>("Permalink3", obj.Context.GetRouteValue<T>("Permalink2", obj.Context.GetRouteValue<T>("Permalink1")))));
        }
        public static string GetLastRouteValue(this System.Web.WebPages.WebPage obj, string def = null)
        {
            return obj.GetLastRouteValue<string>(def);
        }

        public static T CookieValue<T>(this System.Web.WebPages.WebPage obj, string key, T def = default(T))
        {
            var cookie = obj.Request.Cookies[key];
            if (cookie == null) return def;

            return JohnsonManager.Convert.To<T>(cookie.Value, def);
        }
        public static string CookieValue(this System.Web.WebPages.WebPage obj, string key, string def = null)
        {
            return obj.CookieValue<string>(def);
        }

        public static string Permalinks(this System.Web.WebPages.WebPage obj)
        {
            RouteValueDictionary routes = obj.Context.Items["__Route"] as RouteValueDictionary;
            return "/" + string.Join("/", routes.Keys.Select(p => routes[p]));
        }

        public static void BindController(this System.Web.WebPages.WebPage obj, Type type)
        {
            if (type.BaseType != typeof(Controller))
                throw new Exception("Controller must be inherited from Controller");
            var controller = Activator.CreateInstance(type) as Controller;
            controller.Init(obj);
        }
    }
}
