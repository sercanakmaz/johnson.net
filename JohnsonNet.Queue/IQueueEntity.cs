using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnsonNet.Queue
{
    public class QueueEntity
    {
        public QueueEntity()
        {

        }

        public QueueEntity(IQueueParameter input)
        {
            var type = input.GetType();

            this.Input = input;
            this.ParameterAssemblyName = type.Assembly.GetName().Name;
            this.ParameterTypeName = type.FullName;
            this.PropertyValues = new Dictionary<string, object>();

            foreach (var item in JohnsonManager.Reflection.GetPropertiesWithoutHidings(type))
            {
                this.PropertyValues.Add(item.Name, JohnsonManager.Reflection.GetPropertyValue(input, item.Name));
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public IQueueParameter Input { get; set; }

        public string ParameterAssemblyName { get; set; }
        public string ParameterTypeName { get; set; }
        public Dictionary<string, object> PropertyValues { get; set; }
    }
}
