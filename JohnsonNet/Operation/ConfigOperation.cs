using JohnsonNet.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace JohnsonNet.Operation
{
    public class ConfigOperation
    {
        public EnvironmentConfig p_EnvironmentConfig = null;
        public EnvironmentConfig EnvironmentConfig
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

        private IProvider p_Current = null;
        public IProvider Current
        {
            get
            {
                if (p_Current == null)
                {
                    if (EnvironmentConfig == null)
                    {
                        p_Current = new ConfigurationFileProvider(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                    }
                    else
                    {
                        p_Current = GetConfigProvider(CurrentEnvironment);
                    }
                }
                return p_Current;
            }
            set
            {
                p_Current = value;
            }
        }

        public EnvironmentType CurrentEnvironment
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
                           
                            foreach (Rule rule in EnvironmentConfig.Rules)
                            {
                                if (req.Url.Host.Equals(rule.Param, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    return rule.Environment;
                                }
                            }

                            if (req.IsLocal)
                            {
                                return EnvironmentType.Local;
                            }

                            return EnvironmentType.Unknown;
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

        public Rule GetEnvironmentRule()
        {
            return GetEnvironmentRule(CurrentEnvironment);
        }

        public Rule GetEnvironmentRule(EnvironmentType type)
        {
            if (EnvironmentConfig == null) return null;
            return EnvironmentConfig.Rules.OfType<Rule>().FirstOrDefault(p => p.Environment == type);
        }

        public IProvider GetConfigProvider(EnvironmentType type)
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
                default:
                    var providerType = Type.GetType(EnvironmentConfig.Provider, true);
                    var provider = Activator.CreateInstance(providerType, configParam) as IProvider;

                    if (provider == null) throw new NotImplementedException();

                    return provider;
            }
        }

    }
}
