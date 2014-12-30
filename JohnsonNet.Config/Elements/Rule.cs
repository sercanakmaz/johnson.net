using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace JohnsonNet.Config
{
    public class Rule : ConfigurationElement
    {
        [ConfigurationProperty("environment", IsRequired = true)]
        public EnvironmentType Environment
        {
            get
            {
                return (EnvironmentType)this["environment"];
            }
        }
        [ConfigurationProperty("param", IsRequired = true)]
        public string Param
        {
            get
            {
                return this["param"] as string;
            }
        }
    }
}
