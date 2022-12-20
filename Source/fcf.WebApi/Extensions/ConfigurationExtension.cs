// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Identity;
using System.Security.Cryptography.X509Certificates;

namespace FranceConnectFacade.Identity.WebApi.Extensions
{
    public static class ConfigurationExtension
    {
        const string ERROR_APPSETTINGS_NULL_VALUE = "A rajouter dans le fichier appsettings.json, ou secrets.json si c'est un secret";
        const string ERROR_CERTIFICATE_NOT_FOUND = "Le certificat n'a pas été trouvé. est-il bien présent dans le répertoire ./fcf.WebApi ?";
        public static bool IsSettingEnabled(this IConfiguration setting, string key)
        {
            string set = setting[key];
            bool IsSet = true;
            bool.TryParse(set, out IsSet);
            return IsSet;
        }
      
        public static string GetCertificateName(this IConfiguration configuration)
        {
            string CertificateName;
            bool UseDevelopmentCertificate = configuration.IsSettingEnabled("X509:UseDevelopmentCertificate");
            if (UseDevelopmentCertificate)
            {
                CertificateName = configuration["X509:DevCertificateName"];
            }
            else
            {
                CertificateName = configuration["X509:CertificateNameKeyVault"];
            }
            
            if (string.IsNullOrEmpty(CertificateName))
            {
                throw new ArgumentNullException(nameof(CertificateName),ERROR_APPSETTINGS_NULL_VALUE);
            }
            return CertificateName;
        }
        
        /// <summary>
        /// Récupère le certificat de développement à partir 
        /// d'un fichier sur disque au format pfx.
        /// </summary>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void AddDevelopmentCertificate(this IConfiguration configuration)
        {

            string DevelopmentCertificatePfx = configuration["X509:DevelopmentCertificatePfx"];
            if (string.IsNullOrEmpty(DevelopmentCertificatePfx))
            {
                throw new ArgumentNullException(nameof(DevelopmentCertificatePfx),ERROR_APPSETTINGS_NULL_VALUE);
            }
            if (!System.IO.File.Exists(DevelopmentCertificatePfx))
            {
                throw new FileNotFoundException(nameof(DevelopmentCertificatePfx),ERROR_CERTIFICATE_NOT_FOUND);
            }

            string DevelopmentCertificatePassword = configuration["X509:DevelopmentCertificatePassword"];

            if (string.IsNullOrEmpty(DevelopmentCertificatePassword))
            {
                throw new ArgumentNullException(nameof(DevelopmentCertificatePassword), ERROR_APPSETTINGS_NULL_VALUE);
            }

            X509Certificate2 x509 = new X509Certificate2(DevelopmentCertificatePfx, DevelopmentCertificatePassword,X509KeyStorageFlags.Exportable);
            string certificatName = configuration["X509:DevCertificateName"];
            configuration[certificatName] = Convert.ToBase64String(x509.Export(X509ContentType.Pkcs12));
        }
        
        /// <summary>
        /// Récupère le certificat à partir
        /// du Service Azure Keyvault
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="configuration"></param>
        public static void AddCertificateFromKeyVault(this ConfigurationManager configuration)
        {

#if (DEBUG)
            // Autentification au service Azure Key Vault à partir de Visual Studio
            // en mode debug
            string TenantId = configuration["TenantId"];
            if (string.IsNullOrEmpty(TenantId))
            {
                throw new ArgumentNullException(nameof(TenantId), ERROR_APPSETTINGS_NULL_VALUE);
            }

            VisualStudioCredentialOptions options = new VisualStudioCredentialOptions
                {
                    // Paramètre TenantId Azure Active Directory de l'abonnement
                    // Azure qui contient le Key Vault
                    TenantId = configuration["TenantId"],
                    
                };
                
                string AzureKeyVaultEndpoint = configuration["X509:AzureKeyVaultEndpoint"];
                if (string.IsNullOrEmpty(AzureKeyVaultEndpoint))
                {
                    throw new ArgumentNullException(nameof(AzureKeyVaultEndpoint), ERROR_APPSETTINGS_NULL_VALUE);
                }
                
                // Le compte utilisé est celui qui est connecté à Visual Studio et 
                // utilisé dans le paramètre "Azure Service Authentification" des options
                // Visual Studio.

                // D'autre part il faut que ce compte ai les droits d'accès au Key vault.
                // https://learn.microsoft.com/en-us/azure/key-vault/general/assign-access-policy?tabs=azure-portal

                VisualStudioCredential VisualStudioCredential = new VisualStudioCredential(options);
                configuration.AddAzureKeyVault(new Uri(AzureKeyVaultEndpoint),
                                                VisualStudioCredential);
#else
                    {
                        if (builder.Environment.IsProduction())
                        {
                    
                                configuration.AddAzureKeyVault(new Uri(AzureKeyVaultEndpoint),
                                                               new DefaultAzureCredential());
                        }
                    }
#endif
        }
    }   
}