using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.Data
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class FieldMapAttribute : Attribute
    {
        internal FieldMapAttribute()
        {

        }
        public FieldMapAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }

        public string FieldName { get; set; }
    }
}
