using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.WebAPI
{
    public class OutputStatus
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public static OutputStatus Succeed = new OutputStatus { Code = 0, Message = "Succeed" };
        public static OutputStatus Failed = new OutputStatus { Code = -1, Message = "Failed" };
    }
}
