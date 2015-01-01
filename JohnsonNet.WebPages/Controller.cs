using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.WebPages;

namespace JohnsonNet.WebPages
{
    public class Controller
    {
        protected WebPage CurrentWebPage = null;

        public Dictionary<string, object> ClientData
        {
            get
            {
                return CurrentWebPage.PageData["ClientDataObject"] as Dictionary<string, object>;
            }
        }

        internal void Init(WebPage page)
        {
            this.CurrentWebPage = page;

            if (ClientData == null) page.PageData["ClientDataObject"] = new Dictionary<string, object>();

            this.ProcessRequest();
        }

        public virtual void ProcessRequest()
        {
        }

        public T Action<T>(WebPage page)
        {
            string actionName = page.RequestValue("action");
            var controllerType = this.GetType();
            var method = controllerType.GetMethods().FirstOrDefault(p => p.Name.Equals(actionName, StringComparison.CurrentCultureIgnoreCase) && p.IsPublic);
            if (method == null) throw new NotImplementedException();

            return (T)method.Invoke(this, new object[] { page });
        }
    }
}
