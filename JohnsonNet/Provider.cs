using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JohnsonNet.Operation;
using JohnsonNet.Config;
using System.Configuration;
using System.Web;

namespace JohnsonNet
{
    public static class Provider
    {
        private static LogOperation p_Log = null;
        public static LogOperation Log
        {
            get
            {
                if (p_Log == null)
                    p_Log = new LogOperation();

                return p_Log;
            }
            set
            {
                p_Log = value;
            }
        }

        private static MailOperation p_Mail = null;
        public static MailOperation Mail
        {
            get
            {
                if (p_Mail == null)
                    p_Mail = new MailOperation();

                return p_Mail;
            }
            set
            {
                p_Mail = value;
            }
        }

        static EnvironmentConfig p_EnvironmentConfig = null;
        static EnvironmentConfig EnvironmentConfig
        {
            get
            {
                if (p_EnvironmentConfig == null)
                {
                    p_EnvironmentConfig = ConfigurationManager.GetSection("environmentConfig") as EnvironmentConfig;
                }
                return p_EnvironmentConfig;
            }
        }

        private static IProvider p_Config = null;
        public static IProvider Config
        {
            get
            {
                if (p_Config == null)
                {
                    if (EnvironmentConfig == null)
                    {
                        p_Config = new ConfigurationFileProvider(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                    }
                    else
                    {
                        p_Config = GetConfigProvider(CurrentEnvironment);
                    }
                }
                return p_Config;
            }
            set
            {
                p_Config = value;
            }
        }

        public static EnvironmentType CurrentEnvironment
        {
            get
            {
                if (EnvironmentConfig == null)
                    return EnvironmentType.Unknown;

                switch (EnvironmentConfig.Rules.Type)
                {
                    case RuleType.Request:
                        {
                            var req = HttpContext.Current.Request;

                            if (req.IsLocal)
                            {
                                return EnvironmentType.Local;
                            }
                            else
                            {
                                foreach (Rule rule in EnvironmentConfig.Rules)
                                {
                                    if (req.Url.Host.Equals(rule.Param, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        return rule.Environment;
                                    }
                                }
                            }
                        }
                        break;
                    case RuleType.ComputerName:
                        {
                            string computerName = Environment.MachineName;

                            foreach (Rule rule in EnvironmentConfig.Rules)
                            {
                                if (rule.Param.Equals(computerName, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    return rule.Environment;
                                }
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }

                return EnvironmentType.Unknown;
            }
        }

        public static IProvider GetConfigProvider(EnvironmentType type)
        {
            string configParam = null;

            switch (type)
            {
                case EnvironmentType.Live:
                    configParam = EnvironmentConfig.Live;
                    break;
                case EnvironmentType.PreProduction:
                    configParam = EnvironmentConfig.PreProduction;
                    break;
                case EnvironmentType.Test:
                    configParam = EnvironmentConfig.Test;
                    break;
                case EnvironmentType.Local:
                    configParam = EnvironmentConfig.Local;
                    break;
                default:
                    throw new NotImplementedException("Unknown Environment");
            }

            if (EnvironmentConfig.Rules.Type == RuleType.ComputerName)
                configParam = configParam.TrimStart('~').TrimStart('/').Replace('/', '\\');

            switch (EnvironmentConfig.Provider)
            {
                case "JohnsonNet.Config.ConfigurationFileProvider":
                    return new ConfigurationFileProvider(configParam);

                default:
                    var providerType = Type.GetType(EnvironmentConfig.Provider, true);
                    var provider = Activator.CreateInstance(providerType) as IProvider;

                    if (provider == null) throw new NotImplementedException();

                    return provider;
            }
        }
    }
}
