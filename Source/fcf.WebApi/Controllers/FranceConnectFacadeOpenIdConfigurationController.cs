// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FranceConnectFacade.Identity.Extensions;
using FranceConnectFacade.Identity.Model;
using FranceConnectFacade.Identity.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace FranceConnectFacade.Identity.Controllers
{
    [Route("common")]
    [Produces("application/json")]
    [ApiController]
    public class FranceConnectFacadeOpenIdConfigurationController : ControllerBase
    {
        private readonly ILogger<FranceConnectFacadeOpenIdConfigurationController> _logger;
        private readonly IConfiguration _configuration;

        public FranceConnectFacadeOpenIdConfigurationController(ILogger<FranceConnectFacadeOpenIdConfigurationController> logger,
                                  IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Point de terminaison permettant de récupérer la clé
        /// publique pour la signature  et le chiffrement des jetons Web
        /// 
        /// </summary>
        /// <see href=""/>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("discovery/keys")]
        [Produces("application/json")]             
        public IActionResult Keys()
        {
            _logger.LogInformation($"Controller : Discovery/Keys");
            OpenIdDiscoveryKeys? discovery = _configuration
                       .GetSection("OpenIdDiscoveryKeys")
                       .Get<OpenIdDiscoveryKeys>();
            if (discovery == null)
            {
                return StatusCode(500);
            }
            string CertificateNameKeyVault = _configuration.GetCertificateName();
            string rawCertificate = _configuration[CertificateNameKeyVault];

            if (string.IsNullOrEmpty(rawCertificate))
            {
                return StatusCode(500);
            }
            X509Certificate2 x509 = new X509Certificate2(Convert.FromBase64String(rawCertificate));
            
            if (discovery.keys == null)
            {
                return StatusCode(500);
            }
            
            // Récupère le certificat avec la clé publique qui
            // permettra de signer le jeton
            discovery.keys[0].x5c = new string[] { Convert.ToBase64String(x509.GetRawCertData()) };

            // TODO : A vérifier
            // https://www.rfc-editor.org/rfc/rfc7517.html#section-4.5
            //discovery.keys[0].n = x509.GetSerialNumberString();
            discovery.keys[0].kid = x509.Thumbprint;
            //discovery.keys[0].x5t = x509.Thumbprint;
            return Ok(discovery);
        }

        /// <summary>
        /// Point d'entré afin d'obtenir les informations au format Json de découverte 
        /// du fournisseur FranceConnectFacade afin que l'application portal/page
        /// puisse intéragir avec lui.
        /// </summary>
        /// <see href="https://openid.net/specs/openid-connect-discovery-1_0.html"/>
        /// <returns>Ensemble de revendications sur la configuration du fournisseur
        /// OpenID, y compris tous les points de terminaison nécessaires 
        /// et les informations d’emplacement de clé publique</returns>
        [HttpGet]
        [Route(".well-known/openid-configuration")]                
        public IActionResult OpenIdConfiguration()
        {
            _logger.LogInformation($"Controller  : .well-know/OpenIdConfiguration");
            var baseAddress = Request.FormatBaseAddress();

#if TEST_FC_IN_PORTAL
            // Changer adresse dans AppSettings.json du
            // paramètre ngrok pour test/debug avec Portal
            baseAddress = _configuration["ngrok"];         
#endif
            var openIdConfiguration = _configuration
                                .GetSection("OpenIdConfiguration")
                                .Get<OpenIdConfiguration>();

            if (openIdConfiguration == null)
            {
                return StatusCode(500);
            }
            // Construction des URLS qui seront appelées
            // par l'application Portal/Page
            // A noter que la baseAddress en mode debug pointera
            // sur l'adresse fournie par ngrok

            openIdConfiguration.AuthorizationEndpoint=$"{baseAddress}/{openIdConfiguration.AuthorizationEndpoint}";
            openIdConfiguration.JwksUri= $"{baseAddress}/{openIdConfiguration.JwksUri}";            
            openIdConfiguration.EndSessionEndpoint= $"{baseAddress}/{openIdConfiguration.EndSessionEndpoint}";
            openIdConfiguration.UserInfoEndpoint= $"{baseAddress}/{openIdConfiguration.UserInfoEndpoint}";
            openIdConfiguration.TokenEndpoint = $"{baseAddress}/{openIdConfiguration.TokenEndpoint}";
            openIdConfiguration.Issuer = baseAddress;

            return Ok(openIdConfiguration);
        }
    }
}
