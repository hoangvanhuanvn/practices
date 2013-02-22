using System.Security.Cryptography.X509Certificates;
using System.Web;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.SecurityTokenService;

namespace GdNet.Accounts
{
    /// <summary>
    /// A custom SecurityTokenServiceConfiguration implementation.
    /// </summary>
    public class CustomSecurityTokenServiceConfiguration : SecurityTokenServiceConfiguration
    {
        private static readonly object SyncRoot = new object();
        private const string CustomSecurityTokenServiceConfigurationKey = "CustomSecurityTokenServiceConfigurationKey";

        /// <summary>
        /// Provides a model for creating a single Configuration object for the application. The first call creates a new CustomSecruityTokenServiceConfiguration and 
        /// places it into the current HttpApplicationState using the key "CustomSecurityTokenServiceConfigurationKey". Subsequent calls will return the same
        /// Configuration object.  This maintains any state that is set between calls and improves performance.
        /// </summary>
        public static CustomSecurityTokenServiceConfiguration GetCurrent(string issuerName, string signingCertificateName)
        {
            var httpAppState = HttpContext.Current.Application;
            var customConfiguration = httpAppState.Get(CustomSecurityTokenServiceConfigurationKey) as CustomSecurityTokenServiceConfiguration;

            if (customConfiguration == null)
            {
                lock (SyncRoot)
                {
                    customConfiguration = httpAppState.Get(CustomSecurityTokenServiceConfigurationKey) as CustomSecurityTokenServiceConfiguration;

                    if (customConfiguration == null)
                    {
                        customConfiguration = new CustomSecurityTokenServiceConfiguration(issuerName, signingCertificateName);
                        httpAppState.Add(CustomSecurityTokenServiceConfigurationKey, customConfiguration);
                    }
                }
            }

            return customConfiguration;
        }

        /// <summary>
        /// CustomSecurityTokenServiceConfiguration constructor.
        /// </summary>
        public CustomSecurityTokenServiceConfiguration(string issuerName, string signingCertificateName)
            : base(issuerName, new X509SigningCredentials(CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine, signingCertificateName)))
        {
            SecurityTokenService = typeof(CustomSecurityTokenService);
        }
    }
}
