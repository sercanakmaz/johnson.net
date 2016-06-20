using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.log4net.Appender
{
    public class JohnsonDataCustomTypeParameter
    {
        public JohnsonDataCustomTypeParameter()
        {
        }

        public Type LogType { get; set; }
        public Type HandlerType { get; set; }

        public string LogTypeName { get; set; }
        public string HandlerTypeName { get; set; }
    }
}
