using JohnsonNet;
using JohnsonNet.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Web
{
    public class DataController : ApiController
    {
        public static int D = 0; 
        public  GenericOutput<int> Get()
        {
            D++;
            MyClass i = JohnsonManager.Cache.Get<MyClass>("deneme", getAction: () => { return new MyClass { MyProperty = D }; });

            return new GenericOutput<int> { Status = OutputStatus.Failed, Data = i.MyProperty };
        }
    }
    class MyClass
    {
        public int MyProperty { get; set; }
    }
}
