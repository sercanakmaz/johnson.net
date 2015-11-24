using JohnsonNet.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Console
{
    public class OrderQueue : IQueueParameter
    {
        public int OrderID { get; set; }

        public bool DeQueue()
        {
            System.Console.WriteLine("OrderID: {0}", OrderID);

            return true;
        }
    }
}
