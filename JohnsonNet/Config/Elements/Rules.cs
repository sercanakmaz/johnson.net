using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace JohnsonNet.Config
{
    public class Rules : ConfigurationElementCollection
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public RuleType Type
        {
            get
            {
                return (RuleType)this["type"];
            }
        }

        public Rule this[int index]
        {
            get
            {
                return base.BaseGet(index) as Rule;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new Rule();
        }

        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            return Guid.NewGuid();
        }
    }
}
