using JohnsonNet.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Web
{
    public class DataController : Test.Web.Base.DataController
    {
        public override EmptyOutput Get()
        {
            return new EmptyOutput { Status = OutputStatus.Failed };
        }
    }
}
