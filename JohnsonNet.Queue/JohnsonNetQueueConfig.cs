using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnsonNet.Queue
{
    public class JohnsonNetQueueConfig : ConfigurationSection
    {
        [ConfigurationProperty("provider", IsRequired = true)]
        public string Provider
        {
            get
            {
                return this["provider"] as string;
            }
        }
    }
}
