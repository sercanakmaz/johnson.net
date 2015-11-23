using JohnsonNet;
using JohnsonNet.Queue;
using NServiceKit.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Test.Console
{
    public class JohnsonNetQueueRedisDataProvider : JohnsonNet.Queue.IJohnsonQueueDataProvider
    {
        JohnsonNet.Serialization.JsonSerializer jsonSerializer = new JohnsonNet.Serialization.JsonSerializer { ToLowerCase = false };

        const string RedisQueueListKey = "test:queuelist";

        public QueueEntity Get()
        {
            byte[] data = UseRedisClient(redisClient => redisClient.LPop(RedisQueueListKey));
            if (data == null) return null;

            string json = System.Text.Encoding.UTF8.GetString(data);
            QueueEntity queue = jsonSerializer.Deserialize<QueueEntity>(json);

            queue.Input = Activator.CreateInstance(queue.ParameterAssemblyName, queue.ParameterTypeName).Unwrap() as IQueueParameter;

            foreach (var item in JohnsonManager.Reflection.GetPropertiesWithoutHidings(queue.Input.GetType()))
            {
                object val = JohnsonManager.Convert.To(item.PropertyType, queue.PropertyValues[item.Name]);
                item.SetValue(queue.Input, val, null);
            }

            return queue;
        }

        public void Add(JohnsonNet.Queue.QueueEntity input)
        {
            string json = jsonSerializer.Serialize(input);

            UseRedisClient(redisClient =>
                           redisClient.PushItemToList(RedisQueueListKey, json));
        }

        internal static T UseRedisClient<T>(Func<RedisClient, T> action)
        {
            string host = JohnsonNet.JohnsonManager.Config.Current.GetSetting("redisHost");
            int port = JohnsonNet.JohnsonManager.Config.Current.GetSetting<int>("redisPort");

            using (var c = new RedisClient(host, port))
            {
                return action(c);
            }
        }

        internal static void UseRedisClient(Action<RedisClient> action)
        {
            UseRedisClient(c =>
            {
                action(c);

                return true;
            });
        }
    }
}
