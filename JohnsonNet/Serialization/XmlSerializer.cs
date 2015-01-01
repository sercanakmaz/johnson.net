using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JohnsonNet.Serialization
{
    public class XmlSerializer : ISerializer
    {
        public XElement ToXElement(object input)
        {
            if (input == null) return null;

            Type type = input.GetType();

            return new XElement("Class",
                           new XElement((Core.IsAnonymousType(type) ? "AnonymousType" : type.Name),
                               from pi in type.GetProperties()
                               where !pi.GetIndexParameters().Any()
                               let value = pi.GetValue(input, null)
                               select pi.PropertyType.IsValueType || pi.PropertyType == typeof(string)
                                    ? new XElement(pi.Name, value)
                                    : ToXElement(value)
                               )
                           );
        }

        public string Serialize(object input)
        {
            var element = ToXElement(input);
            if (element == null) return null;

            return element.ToString(SaveOptions.None);
        }

        public object Deserialize(string input)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(string input, Type type)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(string input)
        {
            throw new NotImplementedException();
        }
    }
}
