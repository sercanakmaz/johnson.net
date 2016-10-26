using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnsonNet.Queue
{
    public interface IJohnsonQueueDataProvider
    {
        QueueEntity Get();
        void Add(QueueEntity input);
    }
}
