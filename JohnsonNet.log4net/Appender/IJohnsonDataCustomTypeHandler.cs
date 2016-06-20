using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.log4net.Appender
{
    public interface IJohnsonDataCustomTypeHandler
    {
        bool Handle(object input);
    }
}
