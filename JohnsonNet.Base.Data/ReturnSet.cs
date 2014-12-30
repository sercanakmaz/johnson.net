using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.Base.Data
{
    public class ReturnSet<T1, T2>
    {
        public T1 Property1 { get; set; }
        public T2 Property2 { get; set; }
    }
    public class ReturnSet<T1, T2, T3>
    {
        public T1 Property1 { get; set; }
        public T2 Property2 { get; set; }
        public T3 Property3 { get; set; }
    }
    public class ReturnSet<T1, T2, T3, T4>
    {
        public T1 Property1 { get; set; }
        public T2 Property2 { get; set; }
        public T3 Property3 { get; set; }
        public T4 Property4 { get; set; }
    }
    public class ReturnSet<T1, T2, T3, T4, T5>
    {
        public T1 Property1 { get; set; }
        public T2 Property2 { get; set; }
        public T3 Property3 { get; set; }
        public T4 Property4 { get; set; }
        public T5 Property5 { get; set; }
    }
}
