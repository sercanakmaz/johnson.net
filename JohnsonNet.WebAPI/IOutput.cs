using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.WebAPI
{
   public interface IOutput
    {
         OutputStatus Status { get; set; }
    }
}
