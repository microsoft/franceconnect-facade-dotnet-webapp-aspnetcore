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
    public class FranceConnectFacadeCallBackController : ControllerBase
    {
        private readonly ILogger<FranceConnectFacadeCallBackController> _logger;
        private readonly IConfiguration _configuration;
        
        public FranceConnectFacadeCallBackController(ILogger<FranceConnectFacadeCallBackController> logger,
                                  IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;            
        }
        
        /// <summary>
        /// Récupère les paramètres retournés par FranceConnect et redirige vers l'application Portal/Page.
        /// </summary>
        /// <param name="code">Code de retour retourné par France Connect une fois 
        ///                     l'utilisateur authentifié et qui servira à obtenir un jeton</param>
        /// <param name="state">Etat renvoyé par FC</param>
        /// <returns></returns>
        [HttpGet]
        [Route("login-callback")]                
        public RedirectResult FCLoginCallBack([FromQuery] string code, 
                                                 [FromQuery] string state)
        {
            _logger.LogInformation($"Controller : login-callback");

            string? redirectUrl = $"{_configuration["FranceConnect:portalredirecturi"]}?code={code}&state={state}";
            //Redirige vers l'application Portal/Page
            return new RedirectResult(redirectUrl);
        }
    }
}
