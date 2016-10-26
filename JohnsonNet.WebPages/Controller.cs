using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.WebPages;

namespace JohnsonNet.WebPages
{
    public class Controller
    {
        protected WebPage Page = null;

        public Dictionary<string, object> ClientData
        {
            get
            {
                return PageData["ClientDataObject"] as Dictionary<string, object>;
            }
        }

        public IDictionary<object, dynamic> PageData
        {
            get
            {
                return Page.PageData;
            }
        }

        public System.Web.HttpRequestBase Request
        {
            get
            {
                return Page.Request;
            }
        }

        public System.Web.HttpResponseBase Response
        {
            get
            {
                return Page.Response;
            }
        }

        internal void Init(WebPage page)
        {
            this.Page = page;

            if (ClientData == null) this.PageData["ClientDataObject"] = new Dictionary<string, object>();
        }
    }
}
