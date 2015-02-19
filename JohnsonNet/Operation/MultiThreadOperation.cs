using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JohnsonNet.Operation
{
    public class MultiThreadOperation
    {
        #region Threading
        public  Thread ExecuteAsync(Action action, string name = "Worker")
        {
            Thread thread = new Thread(() =>
            {
                try { action(); }
                catch { }
            })
            {
                CurrentCulture = Thread.CurrentThread.CurrentCulture,
                CurrentUICulture = Thread.CurrentThread.CurrentUICulture,
                Name = name
            };
            thread.Start();
            return thread;
        }
        public  Thread ExecuteAsync<T>(Action<T> action, T parameter, string name = "Worker")
        {
            Thread thread = new Thread((p) =>
            {
                try { action((T)p); }
                catch { }
            })
            {
                CurrentCulture = Thread.CurrentThread.CurrentCulture,
                CurrentUICulture = Thread.CurrentThread.CurrentUICulture,
                Name = name
            };
            thread.Start(parameter);
            return thread;
        }
        #endregion
    }
}
