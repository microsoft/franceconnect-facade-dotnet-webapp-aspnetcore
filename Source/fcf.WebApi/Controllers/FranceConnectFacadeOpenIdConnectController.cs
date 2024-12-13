// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using FranceConnectFacade.Identity.Helpers;
using FranceConnectFacade.Identity.Middleware;
using FranceConnectFacade.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace FranceConnectFacade.Identity.Controllers
{
    /// <summary>
    /// Points d'entrée d'autentification et d'autorisation.
    /// </summary>
    [ApiController]
    [Route("/api/beta")]
    public class FranceConnectFacadeOpenIdConnectController : ControllerBase
    {
        
        private readonly ILogger<FranceConnectFacadeOpenIdConnectController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpFranceConnectClient _httpFcClient;
        public FranceConnectFacadeOpenIdConnectController(ILogger<FranceConnectFacadeOpenIdConnectController> logger,
                                      IConfiguration configuration,
                                      IHttpFranceConnectClient httpFcClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpFcClient = httpFcClient;
            
        }

        /// <summary>
        /// Pas encore implémentée.
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [Route("logout")]
        public IActionResult Logout()
        {
            //TODO : Logout
            return Ok();
        }

        /// <summary>
        /// Point de terminaison afin d'obtenir un jeton compatible portal/page.
        /// </summary>
        /// <remarks>
        /// Ce point d'entrée est appelé par l'application portal/page une fois 
        /// l'utilisateur authentifié.
        ///  Le middleware aura pour charge d'obtenir le jeton FC et de le 
        ///  transformer afin qu'il soit compatible avec portal/page.
        /// </remarks>
        /// <returns></returns>
        [HttpPost]        
        [Route("token")]
        [Produces("application/json")]
        [FranceConnectFacadeEndPoint(EndPoint = "token")]

        public IActionResult Token()
        {

            var fcfResult = HttpContext.Items["token"];

            if (fcfResult == null)
            {
                return new UnauthorizedResult();
            }
            _logger.LogInformation(fcfResult.ToString());
            _logger.LogInformation("Controller : token");
            // Retourne le nouveau jeton à l'application Portal/Page
            return new OkObjectResult(fcfResult);

        }

        /// <summary>
        /// Point de terminaison pour l'authentification auprès 
        /// de FranceConnect.
        /// </summary>
        /// <remarks> 
        /// Lorsque l'application portal/page invoque ce point d'entrée,             
        /// Le middleware construit une requête compatible avec 
        /// FranceConnect et redirige uniquement l'appel.
        /// A ce stade, l'authentification ce fait à l'aide des
        /// mécanismes FranceConnect.
        /// </remarks>
        /// <returns></returns>
        [HttpGet()]        
        [Route("authorize")]
        [FranceConnectFacadeEndPoint(EndPoint = "authorize")]        
        public RedirectResult Authorize()
        {
            

            string baseAddress = _configuration["FranceConnect:AuthorizationEndpoint"];
            if (string.IsNullOrEmpty(baseAddress))
            {
                throw new ArgumentNullException("AuthorizationEndpoint", "Vous devez rajouter le point d'entrée FranceConnect dans le fichier appsettings.json");
            }
            // Contient la requête compatible FranceConnect
            string? query = HttpContext.Items["query"] as string;
            

            string redirectUri = $"{baseAddress}/{query}";
            var redirectReponse = this.Redirect(redirectUri);

            _logger.LogInformation($"Controller : authorize");
            // Redirige l'appel vers FranceConnect
            return redirectReponse;
        }
        
    }
    
}