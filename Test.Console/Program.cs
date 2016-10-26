using JohnsonNet;
using JohnsonNet.Data;
using JohnsonNet.log4net.Appender;
using JohnsonNet.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var logger = log4net.LogManager.GetLogger("DefaultLogger");

            logger.Info(new SaveInput
            {
                 ID = 0,
                 Name = "qdwwqd"
            });

            System.Console.ReadKey();
        }
    }
    class SaveInput
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    class SaveInputHandler : IJohnsonDataCustomTypeHandler
    {
        public bool Handle(object input)
        {
            SaveInput typedInput = input as SaveInput;

            return true;
        }
    }
}
