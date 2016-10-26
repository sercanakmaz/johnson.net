using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JohnsonNet.WebAPI
{
    public interface IApiAuthenticater
    {
        object Authenticate(HttpContext context);
    }
}
