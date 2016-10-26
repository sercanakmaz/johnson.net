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
        [ConfigurationProperty("dataProvider", IsRequired = true)]
        public string DataProvider
        {
            get
            {
                return this["dataProvider"] as string;
            }
        }
        [ConfigurationProperty("errorProvider", IsRequired = true)]
        public string ErrorProvider
        {
            get
            {
                return this["errorProvider"] as string;
            }
        }
        [ConfigurationProperty("tryCount", IsRequired = true)]
        public int TryCount
        {
            get
            {
                return JohnsonManager.Convert.To<int>(this["tryCount"]);
            }
        }
    }
}
