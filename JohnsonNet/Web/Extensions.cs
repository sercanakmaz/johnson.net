using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using JohnsonNet;

namespace System.Web
{
    public static class Extensions
    { 
        # region | Request Values |
        private static T ReqValue<T>(System.Web.UI.TemplateControl P, String ParamName, T DefaultValue)
        {
            String ParamValue = P.Page.Request[ParamName];

            return Core.ConvertObject<T>(ParamValue, DefaultValue);
        }
        public static T RequestValue<T>(this System.Web.UI.TemplateControl P, String ParamName)
        {
            return ReqValue<T>(P, ParamName, default(T));
        }
        public static T RequestValue<T>(this System.Web.UI.TemplateControl P, String ParamName, T DefaultValue)
        {
            return ReqValue<T>(P, ParamName, DefaultValue);
        }
        public static T RequestValue<T>(this System.Web.UI.TemplateControl P, Expression<Func<T>> Param)
        {
            String ParamName = ((MemberExpression)Param.Body).Member.Name;

            return ReqValue<T>(P, ParamName, default(T));
        }
        public static T RequestValue<T>(this System.Web.UI.TemplateControl P, Expression<Func<T>> Param, T DefaultValue)
        {
            String ParamName = ((MemberExpression)Param.Body).Member.Name;

            return ReqValue<T>(P, ParamName, DefaultValue);
        }
        # endregion

        #region | Query String Values |
        private static T QSValue<T>(System.Web.UI.TemplateControl P, String ParamName, T DefaultValue)
        {
            String ParamValue = P.Page.Request.QueryString[ParamName];

            return Core.ConvertObject<T>(ParamValue, DefaultValue);
        }
        public static T QueryStringValue<T>(this System.Web.UI.TemplateControl P, String ParamName)
        {
            return QSValue<T>(P, ParamName, default(T));
        }
        public static T QueryStringValue<T>(this System.Web.UI.TemplateControl P, String ParamName, T DefaultValue)
        {
            return QSValue<T>(P, ParamName, DefaultValue);
        }
        public static T QueryStringValue<T>(this System.Web.UI.TemplateControl P, Expression<Func<T>> Param)
        {
            String ParamName = ((MemberExpression)Param.Body).Member.Name;

            return QSValue<T>(P, ParamName, default(T));
        }
        public static T QueryStringValue<T>(this System.Web.UI.TemplateControl P, Expression<Func<T>> Param, T DefaultValue)
        {
            String ParamName = ((MemberExpression)Param.Body).Member.Name;

            return QSValue<T>(P, ParamName, DefaultValue);
        }
        #endregion

        #region | Form Values |
        private static T FValue<T>(System.Web.UI.TemplateControl P, String ParamName, T DefaultValue)
        {
            String ParamValue = P.Page.Request.Form[ParamName] as String;

            return Core.ConvertObject<T>(ParamValue, DefaultValue);
        }
        public static T FormValue<T>(this System.Web.UI.TemplateControl P, String ParamName)
        {
            return FValue<T>(P, ParamName, default(T));
        }
        public static T FormValue<T>(this System.Web.UI.TemplateControl P, String ParamName, T DefaultValue)
        {
            return FValue<T>(P, ParamName, DefaultValue);
        }
        public static T FormValue<T>(this System.Web.UI.TemplateControl P, Expression<Func<T>> Param)
        {
            String ParamName = ((MemberExpression)Param.Body).Member.Name;

            return FValue<T>(P, ParamName, default(T));
        }
        public static T FormValue<T>(this System.Web.UI.TemplateControl P, Expression<Func<T>> Param, T DefaultValue)
        {
            String ParamName = ((MemberExpression)Param.Body).Member.Name;

            return FValue<T>(P, ParamName, DefaultValue);
        }
        #endregion

        #region | RouteData Values |
        private static T RDValue<T>(System.Web.UI.TemplateControl P, String ParamName, T DefaultValue)
        {
            String ParamValue = P.Page.RouteData.Values[ParamName] as String;

            return Core.ConvertObject<T>(ParamValue, DefaultValue);
        }
        public static T RouteDataValue<T>(this System.Web.UI.TemplateControl P, String ParamName)
        {
            return RDValue<T>(P, ParamName, default(T));
        }
        public static T RouteDataValue<T>(this System.Web.UI.TemplateControl P, String ParamName, T DefaultValue)
        {
            return RDValue<T>(P, ParamName, DefaultValue);
        }
        public static T RouteDataValue<T>(this System.Web.UI.TemplateControl P, Expression<Func<T>> Param)
        {
            String ParamName = ((MemberExpression)Param.Body).Member.Name;

            return RDValue<T>(P, ParamName, default(T));
        }
        public static T RouteDataValue<T>(this System.Web.UI.TemplateControl P, Expression<Func<T>> Param, T DefaultValue)
        {
            String ParamName = ((MemberExpression)Param.Body).Member.Name;

            return RDValue<T>(P, ParamName, DefaultValue);
        }
        #endregion

        # region | Cookies |

        public static T CookieValue<T>(this HttpContext obj, string key, T def = default(T))
        {
            var cookie = obj.Request.Cookies[key];
            if (cookie == null) return def;

            return Core.ConvertObject<T>(cookie.Value, def);
        }
        public static string CookieValue(this HttpContext obj, string key, string def = null)
        {
            return obj.CookieValue<string>(def);
        }

        # endregion

    }
}
