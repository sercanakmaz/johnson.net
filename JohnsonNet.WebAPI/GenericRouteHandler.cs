using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace JohnsonNet.WebAPI
{
    public class GenericRouteHandler<T> : IRouteHandler
      where T : IHttpHandlerBase, new()
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var retVal = new T();
            retVal.RequestContext = requestContext;
            return retVal;
        }
    }   
}