using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.WebAPI
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class AuthenticateAttribute : Attribute
    {

    }
}
