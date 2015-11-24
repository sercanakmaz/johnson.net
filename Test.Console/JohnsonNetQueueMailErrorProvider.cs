using JohnsonNet.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Console
{
    public class JohnsonNetQueueMailErrorProvider : IJohnsonQueueErrorProvider
    {
        public void Process(QueueEntity input)
        {
            // TODO: Send Mail
            System.Console.WriteLine("Send Mail For: {0}", input.ParameterTypeName);
        }
    }
}
