using JohnsonNet;
using JohnsonNet.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public class JohnsonQueueManager
    {
        private static IJohnsonQueueDataProvider p_DataProvider = null;
        internal static IJohnsonQueueDataProvider DataProvider
        {
            get
            {
                if (p_DataProvider == null)
                {
                    var config = ConfigurationManager.GetSection("johnsonNetQueueConfig") as JohnsonNetQueueConfig;
                    var providerType = Type.GetType(config.Provider, true);

                    p_DataProvider = Activator.CreateInstance(providerType) as IJohnsonQueueDataProvider;
                }

                return p_DataProvider;
            }
        }

        public static void Add(IQueueParameter input)
        {
            DataProvider.Add(new QueueEntity(input));
        }
        public static QueueEntity Get()
        {
            return DataProvider.Get();
        }
        public static void DeQueue(QueueEntity input)
        {
            try
            {
                if (!input.Input.DeQueue())
                {
                    Add(input.Input);
                }
            }
            catch (Exception ex)
            {
                Add(input.Input);
                JohnsonManager.Logger.Log(ex);
            }
        }
        public static void DeQueuePeriodic()
        {
            JohnsonManager.MultiThread.ExecuteAsync(() =>
            {
                while (true)
                {
                    try
                    {
                        QueueEntity queue = Get();

                        while (queue != null)
                        {
                            DeQueue(queue);

                            queue = Get();
                        }
                    }
                    catch (Exception ex)
                    {
                        JohnsonManager.Logger.Log(ex, "DeQueuePeriodic");
                    }

                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                }
            }, "DeQueuePeriodic");
        }
    }

}
