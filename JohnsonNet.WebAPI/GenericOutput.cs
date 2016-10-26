using JohnsonNet.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.WebAPI
{
    public class GenericOutput<T> : IOutput
    {
        public T Data { get; set; }
        public OutputStatus Status { get; set; }
    }
}
