using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.WebAPI
{
    public class EmptyOutput: IOutput
    {
        public OutputStatus Status { get; set; }
    }
}
