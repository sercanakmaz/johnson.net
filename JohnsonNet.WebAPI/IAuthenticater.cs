using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JohnsonNet.WebAPI
{
    public interface IAuthenticater
    {
        object Authenticate(HttpContext context);
    }
}
