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
    }
}
