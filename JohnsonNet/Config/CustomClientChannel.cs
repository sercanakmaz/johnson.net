using System;
using System.Configuration;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace JohnsonNet.Config
{
    public class CustomClientChannel<T> : ChannelFactory<T>
    {
        string configurationPath;
        string endpointConfigurationName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configurationPath"></param>
        public CustomClientChannel(string configurationPath)
            : base(typeof(T))
        {
            this.configurationPath = configurationPath;
            base.InitializeEndpoint((string)null, null);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="configurationPath"></param>
        public CustomClientChannel(Binding binding, string configurationPath)
            : this(binding, (EndpointAddress)null, configurationPath)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceEndpoint"></param>
        /// <param name="configurationPath"></param>
        public CustomClientChannel(ServiceEndpoint serviceEndpoint, string configurationPath)
            : base(typeof(T))
        {
            this.configurationPath = configurationPath;
            base.InitializeEndpoint(serviceEndpoint);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="configurationPath"></param>
        public CustomClientChannel(string endpointConfigurationName, string configurationPath)
            : this(endpointConfigurationName, null, configurationPath)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="endpointAddress"></param>
        /// <param name="configurationPath"></param>
        public CustomClientChannel(Binding binding, EndpointAddress endpointAddress, string configurationPath)
            : base(typeof(T))
        {
            this.configurationPath = configurationPath;
            base.InitializeEndpoint(binding, endpointAddress);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        /// <param name="configurationPath"></param>
        public CustomClientChannel(Binding binding, string remoteAddress, string configurationPath)
            : this(binding, new EndpointAddress(remoteAddress), configurationPath)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="endpointAddress"></param>
        /// <param name="configurationPath"></param>
        public CustomClientChannel(string endpointConfigurationName, EndpointAddress endpointAddress, string configurationPath)
            : base(typeof(T))
        {
            this.configurationPath = configurationPath;
            this.endpointConfigurationName = endpointConfigurationName;
            base.InitializeEndpoint(endpointConfigurationName, endpointAddress);
        }

        /// <summary>
        /// Loads the serviceEndpoint description from the specified configuration file
        /// </summary>
        /// <returns></returns>
        protected override ServiceEndpoint CreateDescription()
        {
            ServiceEndpoint serviceEndpoint = base.CreateDescription();

            if (endpointConfigurationName != null)
                serviceEndpoint.Name = endpointConfigurationName;

            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = this.configurationPath;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            ServiceModelSectionGroup group = ServiceModelSectionGroup.GetSectionGroup(config);

            ChannelEndpointElement selectedEndpoint = null;

            foreach (ChannelEndpointElement endpoint in group.Client.Endpoints)
            {
                if (endpoint.Contract == serviceEndpoint.Contract.ConfigurationName &&
                    (this.endpointConfigurationName == null || this.endpointConfigurationName == endpoint.Name))
                {
                    selectedEndpoint = endpoint;
                    break;
                }
            }

            if (selectedEndpoint != null)
            {
                if (serviceEndpoint.Binding == null)
                {
                    serviceEndpoint.Binding = CreateBinding(selectedEndpoint.Binding, group);
                }

                if (serviceEndpoint.Address == null)
                {
                    serviceEndpoint.Address = new EndpointAddress(selectedEndpoint.Address, GetIdentity(selectedEndpoint.Identity), selectedEndpoint.Headers.Headers);
                }

                if (serviceEndpoint.Behaviors.Count == 0 && !string.IsNullOrEmpty(selectedEndpoint.BehaviorConfiguration))
                {
                    AddBehaviors(selectedEndpoint.BehaviorConfiguration, serviceEndpoint, group);
                }

                serviceEndpoint.Name = selectedEndpoint.Contract;
            }

            return serviceEndpoint;

        }

        /// <summary>
        /// Configures the binding for the selected endpoint
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        private Binding CreateBinding(string bindingName, ServiceModelSectionGroup group)
        {
            BindingCollectionElement bindingElementCollection = group.Bindings[bindingName];
            if (bindingElementCollection.ConfiguredBindings.Count > 0)
            {
                IBindingConfigurationElement be = bindingElementCollection.ConfiguredBindings[0];

                Binding binding = GetBinding(be);
                if (be != null)
                {
                    be.ApplyConfiguration(binding);
                }

                return binding;
            }

            return null;
        }

        /// <summary>
        /// Helper method to create the right binding depending on the configuration element
        /// </summary>
        /// <param name="configurationElement"></param>
        /// <returns></returns>
        private Binding GetBinding(IBindingConfigurationElement configurationElement)
        {
            if (configurationElement is CustomBindingElement)
                return new CustomBinding();
            else if (configurationElement is BasicHttpBindingElement)
                return new BasicHttpBinding();
            else if (configurationElement is NetMsmqBindingElement)
                return new NetMsmqBinding();
            else if (configurationElement is NetNamedPipeBindingElement)
                return new NetNamedPipeBinding();
            else if (configurationElement is NetPeerTcpBindingElement)
                return new NetPeerTcpBinding();
            else if (configurationElement is NetTcpBindingElement)
                return new NetTcpBinding();
            else if (configurationElement is WSDualHttpBindingElement)
                return new WSDualHttpBinding();
            else if (configurationElement is WSHttpBindingElement)
                return new WSHttpBinding();
            else if (configurationElement is WSFederationHttpBindingElement)
                return new WSFederationHttpBinding();

            return null;
        }

        /// <summary>
        /// Adds the configured behavior to the selected endpoint
        /// </summary>
        /// <param name="behaviorConfiguration"></param>
        /// <param name="serviceEndpoint"></param>
        /// <param name="group"></param>
        private void AddBehaviors(string behaviorConfiguration, ServiceEndpoint serviceEndpoint, ServiceModelSectionGroup group)
        {
            EndpointBehaviorElement behaviorElement = group.Behaviors.EndpointBehaviors[behaviorConfiguration];
            for (int i = 0; i < behaviorElement.Count; i++)
            {
                BehaviorExtensionElement behaviorExtension = behaviorElement[i];
                object extension = behaviorExtension.GetType().InvokeMember("CreateBehavior",
                    BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                    null, behaviorExtension, null);
                if (extension != null)
                {
                    serviceEndpoint.Behaviors.Add((IEndpointBehavior)extension);
                }
            }
        }

        /// <summary>
        /// Gets the endpoint identity from the configuration file
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private EndpointIdentity GetIdentity(IdentityElement element)
        {
            EndpointIdentity identity = null;
            PropertyInformationCollection properties = element.ElementInformation.Properties;
            if (properties["userPrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateUpnIdentity(element.UserPrincipalName.Value);
            }
            if (properties["servicePrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateSpnIdentity(element.ServicePrincipalName.Value);
            }
            if (properties["dns"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateDnsIdentity(element.Dns.Value);
            }
            if (properties["rsa"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateRsaIdentity(element.Rsa.Value);
            }
            if (properties["certificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                X509Certificate2Collection supportingCertificates = new X509Certificate2Collection();
                supportingCertificates.Import(Convert.FromBase64String(element.Certificate.EncodedValue));
                if (supportingCertificates.Count == 0)
                {
                    throw new InvalidOperationException("UnableToLoadCertificateIdentity");
                }
                X509Certificate2 primaryCertificate = supportingCertificates[0];
                supportingCertificates.RemoveAt(0);
                return EndpointIdentity.CreateX509CertificateIdentity(primaryCertificate, supportingCertificates);
            }

            return identity;
        }


        protected override void ApplyConfiguration(string configurationName)
        {
            //base.ApplyConfiguration(configurationName);
        }
    }


}
