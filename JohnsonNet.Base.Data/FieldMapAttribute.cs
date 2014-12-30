using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.Base.Data
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class FieldMapAttribute : Attribute
    {

        public FieldMapAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }

        public string FieldName { get; set; }
    }
}
