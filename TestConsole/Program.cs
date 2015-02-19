using JohnsonNet;
using JohnsonNet.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = JohnsonManager.Data.Execute<MyClass>("Member.Get", new JohnsonNet.Data.ParamDictionary { { "ID", 730 }, { "IsActive", null } });
            Console.WriteLine(result);
        }
    }
    class MyClass
    {

    }
}
