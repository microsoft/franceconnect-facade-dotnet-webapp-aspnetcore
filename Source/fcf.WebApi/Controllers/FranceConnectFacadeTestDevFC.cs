// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FranceConnectFacade.Identity.Extensions;
using FranceConnectFacade.Identity.Middleware;
using FranceConnectFacade.Identity.Model;
using FranceConnectFacade.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Text;
using System.Text.Json;

namespace FranceConnectFacade.Identity.Controllers
{
    
    [Produces("application/json")]
    [ApiController]
    public class FranceConnectFacadeTestDevFC : ControllerBase
    {
        private readonly ILogger<FranceConnectFacadeTestDevFC> _logger;
        private readonly IConfiguration _configuration;
        
        public FranceConnectFacadeTestDevFC(ILogger<FranceConnectFacadeTestDevFC> logger,
                                  IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;            
        }
        
        /// <summary>
        /// N'existe que pour répondre aux exigeances
        /// du compte de dev France Connect
        /// La facade doit démarrer sur le port 4242
        /// <see cref="launchSettings.json" />
        /// </summary>
        /// <param name="code">Code de retour retourné par France Connect une fois 
        ///                     l'utilisateur autentifié et qui servira à obtenir un jeton</param>
        /// <param name="state">Etat renvoyé par FC</param>
        /// <returns></returns>
        [HttpGet]
        [Route("login-callback")]                
        public RedirectResult FCDEVLoginCallBack([FromQuery] string code, 
                                                 [FromQuery] string state)
        {
            // Code de test avec FC et portal
            _logger.LogInformation("Controller : testfcinportal");
            
            // A Récupèrer cette URL sur le portail de powerapps  
            // lors de l'ajout du fournisseur d'identité 
            // OpenId Connect pour l'application Portal/Page

            string? redirectUrl = $"{_configuration["FranceConnect:portalredirecturi"]}?code={code}&state={state}";
            //Redirige vers l'application Portal/Page
            return new RedirectResult(redirectUrl);
        }
    }
}
