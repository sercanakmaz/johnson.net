using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using log4net.Core;

namespace JohnsonNet.log4net.Appender
{
    public class JohnsonDataAppender : global::log4net.Appender.AdoNetAppender
    {
        public JohnsonDataAppender()
        {
            CustomTypeList = new List<JohnsonDataCustomTypeParameter>();
        }

        public List<JohnsonDataCustomTypeParameter> CustomTypeList { get; private set; }

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            var connectionStringName = JohnsonManager.Convert.Default(ConnectionStringName, "LocalSqlServer");
            var connectionString = JohnsonManager.Config.Current.GetConnectionString(connectionStringName);

            if (connectionString == null)
            {
                throw new LogException("ConectionString cannot be found");
            }
            this.ConnectionString = connectionString.ConnectionString;
        }

        protected override Type ResolveConnectionType()
        {
            var connectionStringName = JohnsonManager.Convert.Default(ConnectionStringName, "LocalSqlServer");
            var connectionString = JohnsonManager.Config.Current.GetConnectionString(connectionStringName);

            if (connectionString == null)
            {
                throw new LogException("ConectionString cannot be found");
            }

            var factory = DbProviderFactories.GetFactory(connectionString.ProviderName);
            var dbConnection = factory.CreateConnection();

            return dbConnection.GetType();
        }

        public void AddCustomType(JohnsonDataCustomTypeParameter item)
        {
            CustomTypeList.Add(item);
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            foreach (var loggingEvent in loggingEvents)
            {
                this.Append(loggingEvent);
            }
        }
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (HandleCustomTypes(loggingEvent)) return;

            base.Append(loggingEvent);
        }
        protected virtual bool HandleCustomTypes(LoggingEvent loggingEvent)
        {
            var customType = CustomTypeList.FirstOrDefault(p => p.HandlerType.Equals(loggingEvent.MessageObject.GetType()));

            if (customType != null)
            {
                var handler = Activator.CreateInstance(customType.HandlerType) as IJohnsonDataCustomTypeHandler;

                handler.Handle(loggingEvent.MessageObject);

                return true;
            }

            return false;
        }
    }
}
