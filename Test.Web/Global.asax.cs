using JohnsonNet.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Test.Web
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