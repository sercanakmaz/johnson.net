using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;

namespace JohnsonNet.WebAPI
{
    public interface IHttpHandlerBase : IHttpHandler, IReadOnlySessionState, IRequiresSessionState
    {
        RequestContext RequestContext { get; set; }
    }
}