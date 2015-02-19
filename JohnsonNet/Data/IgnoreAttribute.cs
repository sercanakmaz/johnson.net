using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.Data
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IgnoreAttribute : Attribute
    {
        public IgnoreAttribute()
        {
        }
    }
}
