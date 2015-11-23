using JohnsonNet;
using JohnsonNet.Data;
using JohnsonNet.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            JohnsonQueueManager.Add(new CouponQueue
            {
                CouponCode = "KOD-01"
            });
            JohnsonQueueManager.Add(new CouponQueue
            {
                CouponCode = "KOD-02"
            });
            JohnsonQueueManager.Add(new CouponQueue
            {
                CouponCode = "KOD-03"
            });

            JohnsonQueueManager.Add(new OrderQueue
            {
                OrderID = 1
            });
            JohnsonQueueManager.Add(new OrderQueue
            {
                OrderID = 5
            });

            JohnsonQueueManager.DeQueuePeriodic();

            Console.Read();
        }
    }
}
