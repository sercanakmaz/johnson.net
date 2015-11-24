using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.Queue
{
    public interface IJohnsonQueueErrorProvider
    {
        void Process(QueueEntity input);
    }
}
