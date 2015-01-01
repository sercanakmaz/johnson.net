using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace JohnsonNet.Data
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [ComVisible(false)]
    public class ParamDictionary : Dictionary<string, object>
    {
        public ParamDictionary() { }
        public ParamDictionary(IDictionary<string, object> dictionary) : base(dictionary) { }
        public ParamDictionary(IEqualityComparer<string> comparer) : base(comparer) { }
        public ParamDictionary(int capacity) : base(capacity) { }
        public ParamDictionary(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer) : base(dictionary, comparer) { }
        public ParamDictionary(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer) { }
        protected ParamDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public virtual XElement ToXElement()
        {
            return new XElement("Entity", this.Select(kv => new XElement(kv.Key, kv.Value)));
        }
        public static ParamDictionary Convert(object input)
        {
            if (input == null) throw new ArgumentNullException("input");

            var type = input.GetType();
            var result = new ParamDictionary();

            foreach (var item in type.GetProperties().Where(p => p.PropertyType.IsPrimitive))
            {
                result.Add(item.Name, item.GetValue(input, null));
            }

            return result;
        }
    }
}
