using JohnsonNet.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Console
{
    public class CouponQueue : IQueueParameter
    {
        public string CouponCode { get; set; }

        public bool DeQueue()
        {
            if (CouponCode.EqualsWithCurrentCulture("KOD-03"))
            {
                throw new NotImplementedException();
            }
            System.Console.WriteLine(this.CouponCode);

            return true;
        }
    }
}
