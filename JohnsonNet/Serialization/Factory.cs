using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.Serialization
{
    public static class Factory
    {
        private static JsonSerializer p_Json = null;
        public static JsonSerializer Json
        {
            get
            {
                if (p_Json ==null) p_Json = new JsonSerializer();
                return p_Json;
            }
        }
        private static XmlSerializer p_Xml = null;
        public static XmlSerializer Xml
        {
            get
            {
                if (p_Xml == null) p_Xml = new XmlSerializer();
                return p_Xml;
            }
        }
        private static ISerializer p_Current = null;
        public static ISerializer Current
        {
            get
            {
                if (p_Current == null) p_Current = new JsonSerializer();
                return p_Current;
            }
        }
    }
}
