using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace JohnsonNet.Config
{
    public class ConfigurationFileProvider : IProvider
    {
        internal Configuration configuration = null;
        internal ConfigurationFileProvider(string configSource)
        {
            if (!Path.IsPathRooted(configSource))
            {
                if (HttpContext.Current != null)
                {
                    configSource = HttpContext.Current.Server.MapPath(configSource);
                }
                else
                {
                    var directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    configSource = Path.Combine(directoryPath, configSource);
                }
            }

            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap { ExeConfigFilename = configSource };
            configuration = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
        }

        public T GetSetting<T>(string key, T def = default(T))
        {
            var s = configuration.AppSettings.Settings[key];
            if (s == null) return def;
            return Helpers.ConvertObject<T>(s.Value, def);
        }
        public string GetSetting(string key, string def = null)
        {
            var s = configuration.AppSettings.Settings[key];
            if (s == null) return def;
            return Helpers.ConvertObject(s.Value, def);
        }
        public T GetCommunicationObject<T>()
        {
            CustomClientChannel<T> channel = new CustomClientChannel<T>(configuration.FilePath);
            return channel.CreateChannel();
        }
        public ConnectionStringSettings GetConnectionString(string key)
        {
            return configuration.ConnectionStrings.ConnectionStrings[key];
        }
        public System.Data.IDbConnection GetConnection(string key)
        {
            var conn = GetConnectionString(key);
            return Helpers.GetConnection(conn);
        }
    }
}
