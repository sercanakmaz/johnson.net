using JohnsonNet.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Web.Base
{
    public class DataController : ApiController
    {
        public virtual EmptyOutput Get()
        {
            return new EmptyOutput { Status = OutputStatus.Succeed };
        }
    }
}
