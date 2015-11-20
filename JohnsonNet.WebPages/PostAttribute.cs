using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.WebPages
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class PostAttribute : Attribute
    {
    }
}
