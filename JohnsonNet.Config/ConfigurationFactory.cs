using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace JohnsonNet.Config
{
    public class ConfigurationFactory
    {
        static EnvironmentConfig p_CurrentConfig = null;
        static EnvironmentConfig CurrentConfig
        {
            get
            {
                if (p_CurrentConfig == null)
                {
                    p_CurrentConfig = ConfigurationManager.GetSection("environmentConfig") as EnvironmentConfig;
                }
                return p_CurrentConfig;
            }
        }

        private static IProvider p_Current = null;
        public static IProvider Current
        {
            get
            {
                if (p_Current == null)
                {
                    if (CurrentConfig == null)
                    {
                        p_Current = new ConfigurationFileProvider(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                    }
                    else
                    {
                        p_Current = GetProvider(CurrentEnvironment);
                    }
                }
                return p_Current;
            }
            set
            {
                p_Current = value;
            }
        }
        public static EnvironmentType CurrentEnvironment
        {
            get
            {
                if (CurrentConfig == null)
                    return EnvironmentType.Unknown;

                switch (CurrentConfig.Rules.Type)
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
                                foreach (Rule rule in CurrentConfig.Rules)
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

                            foreach (Rule rule in CurrentConfig.Rules)
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

        public static IProvider GetProvider(EnvironmentType type)
        {
            IProvider provider = null;
            string configParam = null;

            switch (type)
            {
                case EnvironmentType.Live:
                    configParam = CurrentConfig.Live;
                    break;
                case EnvironmentType.PreProduction:
                    configParam = CurrentConfig.PreProduction;
                    break;
                case EnvironmentType.Test:
                    configParam = CurrentConfig.Test;
                    break;
                case EnvironmentType.Local:
                    configParam = CurrentConfig.Local;
                    break;
                default:
                    throw new NotImplementedException();
            }

            switch (CurrentConfig.Provider)
            {
                case "ConfigurationFile":
                    if (CurrentConfig.Rules.Type == RuleType.ComputerName)
                        configParam = configParam.TrimStart('~').TrimStart('/').Replace('/', '\\');

                    provider = new ConfigurationFileProvider(configParam);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return provider;
        }
    }
}
