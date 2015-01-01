using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace JohnsonNet.WebPages
{
    public static class RouteCollectionExtension
    {
        public static void MapWebPageRoute(this RouteCollection routeCollection, string routeUrl, string virtualPath, object defaultValues = null, object constraints = null, string routeName = null)
        {
            routeName = routeName ?? routeUrl;

            Route item = new Route(routeUrl, new RouteValueDictionary(defaultValues), new RouteValueDictionary(constraints), new WebPagesRouteHandler(virtualPath));
            routeCollection.Add(routeName, item);
        }
    }
}
