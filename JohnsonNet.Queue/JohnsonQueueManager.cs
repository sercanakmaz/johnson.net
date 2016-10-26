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
        private static JohnsonNetQueueConfig p_CurrentJohnsonNetQueueConfig = null;
        internal static JohnsonNetQueueConfig CurrentJohnsonNetQueueConfig
        {
            get
            {
                if (p_CurrentJohnsonNetQueueConfig == null)
                {
                    p_CurrentJohnsonNetQueueConfig = ConfigurationManager.GetSection("johnsonNetQueueConfig") as JohnsonNetQueueConfig;
                }
                return p_CurrentJohnsonNetQueueConfig;
            }
        }

        private static IJohnsonQueueDataProvider p_DataProvider = null;
        internal static IJohnsonQueueDataProvider DataProvider
        {
            get
            {
                if (p_DataProvider == null)
                {
                    var providerType = Type.GetType(CurrentJohnsonNetQueueConfig.DataProvider, true);

                    p_DataProvider = Activator.CreateInstance(providerType) as IJohnsonQueueDataProvider;
                }
                return p_DataProvider;
            }
        }

        private static IJohnsonQueueErrorProvider p_ErrorProvider = null;
        internal static IJohnsonQueueErrorProvider ErrorProvider
        {
            get
            {
                if (p_ErrorProvider == null)
                {
                    var providerType = Type.GetType(CurrentJohnsonNetQueueConfig.ErrorProvider, true);

                    p_ErrorProvider = Activator.CreateInstance(providerType) as IJohnsonQueueErrorProvider;
                }
                return p_ErrorProvider;
            }
        }

        public static void Add(IQueueParameter input)
        {
            Add(new QueueEntity(input));
        }
        public static void Add(QueueEntity input)
        {
            DataProvider.Add(input);
        }
        public static QueueEntity Get()
        {
            return DataProvider.Get();
        }
        public static void DeQueue(QueueEntity input)
        {
            try
            {
                bool dequeueResult = false;

                try
                {
                    dequeueResult = input.Input.DeQueue();
                }
                catch (Exception ex)
                {
                    dequeueResult = false;
                    JohnsonManager.Logger.Log(ex);
                }

                if (!dequeueResult)
                {
                    bool isTryLimitReached = input.TryCount > CurrentJohnsonNetQueueConfig.TryCount;

                    if (isTryLimitReached)
                    {
                        ErrorProvider.Process(input);
                        return;
                    }

                    input.TryCount += 1;

                    Add(input);
                }
            }
            catch (Exception ex)
            {
                Add(input);
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
