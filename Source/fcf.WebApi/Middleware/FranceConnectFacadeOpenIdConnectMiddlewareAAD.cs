// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.



using FranceConnectFacade.Identity.Extensions;
using FranceConnectFacade.Identity.Helpers;
using FranceConnectFacade.Identity.Services;
using FranceConnectFacade.Identity.WebApi.Middleware;
using System.Security.Claims;
using System.Text;


namespace FranceConnectFacade.Identity.Middleware
{
   
    
    public class FranceConnectFacadeOpenIdConnectMiddlewareAAD
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        
        private readonly IHttpFranceConnectClient _httpFcClient;
        private readonly ILogger<FranceConnectFacadeOpenIdConnectMiddleware> _logger;
        public FranceConnectFacadeOpenIdConnectMiddlewareAAD(RequestDelegate next, 
                                          IConfiguration configuration,
                                          IHttpFranceConnectClient httpFcClient,
                                          ILogger<FranceConnectFacadeOpenIdConnectMiddleware> logger)
        {

            _next = next ?? throw new ArgumentNullException(nameof(next));
            
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            
            _httpFcClient = httpFcClient ?? throw new ArgumentNullException(nameof(httpFcClient)); ;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
        }

        /// <summary>
        /// Déclenché si le EndPoint ajoute l'attribut 
        /// FranceConnectFacadeEndPoint(EndPoint = "authorize")
        /// <see cref="FranceConnectFacadeOpenIdConnectController"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public void AuthorizeEndPoint(HttpContext context)
        {
            _logger.LogInformation($"Middle : AuthorizeEndPoint");
            
    
            QueryString qs = context.Request.QueryString;
            string? fromQuery = qs.Value;            
            
            context.Items["query"] = fromQuery;
        }

        /// <summary>
        /// Déclenché si le EndPoint ajoute l'attribut 
        /// FranceConnectFacadeEndPoint(EndPoint = "token")
        /// <see cref="FranceConnectFacadeOpenIdConnectController"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task TokenEndPoint(HttpContext context, 
                                        FranceConnectFacadeConfigurationOptions options)
        {
                _logger.LogInformation($"Middle : TokenEndPoint");
            
                options = options ?? throw new ArgumentNullException(nameof(options));
                if (options.FranceConnectOptions == null)
                {
                    throw new ArgumentNullException(nameof(FranceConnectConfiguration));
                }

                if (string.IsNullOrEmpty(options.FranceConnectOptions.ClientSecret))
                {
                    throw new ArgumentNullException("FranceConnect Client Secret not found");
                }
                if (string.IsNullOrEmpty(options.FranceConnectOptions.ClientId))
                {
                    throw new ArgumentNullException("FranceConnect Client Id not found");
                }
                if (string.IsNullOrEmpty(options.X509Cert))
                {
                    throw new ArgumentNullException("Certificate not found");
                }
            
                
                string fromBody = await context.Request.GetBodyAsync();
            
                // France Connect ne supporte pas le flux PKCE            
                


                if (options.ValidateSecret)
                {
                    // Vérifie si l'appelant utilise le même secret FranceConnect
                    MemoryStream bodyStream =
                        new MemoryStream(Encoding.UTF8.GetBytes(fromBody));
                    context.Request.Body = bodyStream;
                    string? bodySecret = Common.GetValue("client_secret", fromBody);
                    if (bodySecret == null)
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }
                    if (!bodySecret.Equals(options.FranceConnectOptions.ClientSecret))
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }
            }


               // Appel du endpoint FranceConnect api/v1/token
               _logger.LogInformation("Middle : Get FranceConnect Token");
                var fcResult = await _httpFcClient.GetFranceConnectToken(fromBody);
                if (fcResult == null)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                if (string.IsNullOrEmpty(fcResult.IdToken))
                {
                    context.Response.StatusCode = 401;
                    return;
                }
                 
               

                _logger.LogInformation("Middle : Validate FC Token : OK");
                                                    
                context.Items.Add("token", fcResult);                        
        }
        public async Task InvokeAsync(HttpContext context,
                                      FranceConnectFacadeConfigurationOptions options)
        {
           
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
           
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {

                var baseAddress = context.Request.FormatBaseAddress();
                
                baseAddress = _configuration["ngrok"];

                string route = ((RouteEndpoint)endpoint).RoutePattern.RawText;
                _logger.LogInformation($"Middle : {baseAddress}/{route}");
                FranceConnectFacadeEndPoint? endpointAttribute =
                    endpoint.Metadata.GetMetadata<FranceConnectFacadeEndPoint>();
                
                if (endpointAttribute !=null)
                {
                    if (endpointAttribute.EndPoint == null)
                    {
                        throw new ArgumentNullException("FranceConnectFacade EndPoint not found");
                    }
                    if (endpointAttribute.EndPoint.Equals("token",
                                                           StringComparison.InvariantCultureIgnoreCase))
                    {
                        await TokenEndPoint(context, options);
                        if (context.Response.StatusCode == 401)
                        {
                            return;
                        }

                    }
                    else if (endpointAttribute.EndPoint.Equals("authorize",
                                                                StringComparison.InvariantCultureIgnoreCase))
                    {

                        
                        AuthorizeEndPoint(context);

                    }                                       
                    else
                    {
                        throw new ArgumentException("CustomAAD EndPoint not found");
                    }
                }
            }

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }
   
}

