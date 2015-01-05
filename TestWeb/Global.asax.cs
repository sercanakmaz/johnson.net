using JohnsonNet.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using JohnsonNet.WebPages;

namespace TestWeb
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.Add(new Route("api/{controller}", new GenericRouteHandler<HttpHandler>()));
            RouteTable.Routes.Add(new Route("api/{controller}/{action}", new GenericRouteHandler<HttpHandler>()));
            RouteTable.Routes.Add(new Route("api/{controller}/{action}/{param}", new GenericRouteHandler<HttpHandler>()));
            RouteTable.Routes.Add(new Route("api/{controller}/{action}/{param}/{param1}", new GenericRouteHandler<HttpHandler>()));
            RouteTable.Routes.Add(new Route("api/{controller}/{action}/{param}/{param1}/{param2}", new GenericRouteHandler<HttpHandler>()));

            // Permalink1 must be empty!!
            RouteTable.Routes.MapWebPageRoute("{Permalink1}", "~/Route.cshtml", new { Permalink1 = string.Empty });
            RouteTable.Routes.MapWebPageRoute("{Permalink1}/{Permalink2}", "~/Route.cshtml");
            RouteTable.Routes.MapWebPageRoute("{Permalink1}/{Permalink2}/{Permalink3}", "~/Route.cshtml");
            RouteTable.Routes.MapWebPageRoute("{Permalink1}/{Permalink2}/{Permalink3}/{Permalink4}", "~/Route.cshtml");
            RouteTable.Routes.MapWebPageRoute("{Permalink1}/{Permalink2}/{Permalink3}/{Permalink4}/{Permalink5}", "~/Route.cshtml");

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}