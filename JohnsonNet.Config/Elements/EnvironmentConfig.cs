using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace JohnsonNet.Config
{
    public class EnvironmentConfig : ConfigurationSection
    {
        [ConfigurationProperty("live", IsRequired = false)]
        public string Live
        {
            get
            {
                return this["live"] as string;
            }
        }
        [ConfigurationProperty("preProduction", IsRequired = false)]
        public string PreProduction
        {
            get
            {
                return this["preProduction"] as string;
            }
        }
        [ConfigurationProperty("test", IsRequired = false)]
        public string Test
        {
            get
            {
                return this["test"] as string;
            }
        }
        [ConfigurationProperty("local", IsRequired = false)]
        public string Local
        {
            get
            {
                return this["local"] as string;
            }
        }

        [ConfigurationProperty("provider", IsRequired = true)]
        public string Provider
        {
            get
            {
                return this["provider"] as string;
            }
        }

        [System.Configuration.ConfigurationProperty("rules", IsRequired = true)]
        [ConfigurationCollection(typeof(Rules))]
        public Rules Rules
        {
            get
            {
                object o = this["rules"];
                return o as Rules;
            }
        }
    }
}
