using JohnsonNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JohnsonNet.WebAPI
{
    public class ApiController
    {
        public JohnsonNet.Serialization.ISerializer Serializer { get; set; }
        public IAuthenticater Authenticater { get; set; }
        public HttpContext Context { get { return HttpContext.Current; } }

        public virtual IOutput OnError(Exception ex, string action, string requestBody) { return null; }
    }
}
