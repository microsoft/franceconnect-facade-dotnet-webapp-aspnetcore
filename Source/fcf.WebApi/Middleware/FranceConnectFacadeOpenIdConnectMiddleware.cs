﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.



using FranceConnectFacade.Identity.Extensions;
using FranceConnectFacade.Identity.Helpers;
using FranceConnectFacade.Identity.Services;
using FranceConnectFacade.Identity.WebApi.Middleware;
using System.Security.Claims;

namespace FranceConnectFacade.Identity.Middleware
{


    public class FranceConnectFacadeOpenIdConnectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        private readonly IHttpFranceConnectClient _httpFcClient;
        private readonly ILogger<FranceConnectFacadeOpenIdConnectMiddleware> _logger;
        public FranceConnectFacadeOpenIdConnectMiddleware(RequestDelegate next,
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
        /// Crée une requête compatible France Connect        
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public void CreateQueryForAuthorizeEndPoint(HttpContext context)
        {
            _logger.LogInformation($"Middle : CreateQueryForAuthorizeEndPoint");

            QueryString qs = context.Request.QueryString;
            string? fromQuery = qs.Value;
            if (string.IsNullOrEmpty(fromQuery))
            {
                context.Response.StatusCode = 401;
                return;

            }

            // Le redirectUri va être ajouté au paramètre state pour
            // être récupéré plus tard par le endpoint login-callback
            // TODO sauvegarder redirectUri dans une map et passer
            // uniquement son index à state
            string? redirectUri = Common.GetValue("redirect_uri", fromQuery);

            // Construction d'une nouvelle QueryString avec ajout
            // de acr_values obligatoire pour FranceConnect
            string query = $"?client_id={Common.GetValue("client_id", fromQuery)}" +
                        $"&redirect_uri={_configuration["FranceConnect:fcredirecturi"]}" +
                        $"&response_type={Common.GetValue("response_type", fromQuery)}" +
                        $"&scope={Common.GetValue("scope", fromQuery)}" +
                        $"&response_mode={Common.GetValue("response_mode", fromQuery)}" +
                        $"&nonce={Common.GetValue("nonce", fromQuery)}" +
                        $"&acr_values={_configuration["OpenIdConfiguration:AcrValuesSupported:0"]}" +
                        $"&state={Common.GetValue("state", fromQuery)}{redirectUri}";

            // Sauvegarde de la QueryString dans le contexte http
            // pour réutilisation avec le EndPoint api/authorize
            /// <see cref="FranceConnectFacadeOpenIdConnectController"/>
            context.Items["query"] = query;
        }

        /// <summary>
        /// Déclenché si on ajoute l'attribut 
        /// FranceConnectFacadeEndPoint(EndPoint = "token")
        /// <see cref="FranceConnectFacadeOpenIdConnectController"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task InvokeTokenEndPointAsync(HttpContext context,
                                                   FranceConnectFacadeConfigurationOptions options)
        {
            _logger.LogInformation($"Middle : InvokeTokenEndPointAsync");
            #region VERIFICATION DES PARAMETRES     
            options = options ?? throw new ArgumentNullException(nameof(options));
            if (options.FranceConnectOptions == null)
            {
                throw new ArgumentNullException(nameof(FranceConnectConfiguration));
            }

            if (string.IsNullOrEmpty(options.FranceConnectOptions.ClientSecret))
            {
                throw new ArgumentNullException("Secret FranceConnect non trouvé");
            }
            if (string.IsNullOrEmpty(options.FranceConnectOptions.ClientId))
            {
                throw new ArgumentNullException("Client Id FranceConnect non trouvé");
            }
            if (string.IsNullOrEmpty(options.X509Cert))
            {
                throw new ArgumentNullException("Certificat non trouvé");
            }
            #endregion

            // Récupère le corps du message car nous allons le transformer
            // pour être compatible avec portal
            string fromBody = await context.Request.GetBodyAsync();
            fromBody = $"client_id={Common.GetValue("client_id", fromBody)}" +
                $"&client_secret={Common.GetValue("client_secret", fromBody)}" +
                $"&code={Common.GetValue("code", fromBody)}" +
                $"&grant_type=authorization_code" +
                $"&redirect_uri={_configuration["FranceConnect:fcredirecturi"]}";

            // France Connect ne supporte pas le flux PKCE            
            fromBody = Helpers.OAuth.DisablePKCE(fromBody);

            // Obtenir le jeton FranceConnect (endpoint api/v1/token)
            _logger.LogInformation("Middle : Obtient le jeton FranceConnect");
            var franceConnectResult = await _httpFcClient.GetFranceConnectToken(fromBody);
            if (franceConnectResult == null)
            {
                context.Response.StatusCode = 401;
                return;
            }

            if (string.IsNullOrEmpty(franceConnectResult.IdToken))
            {
                context.Response.StatusCode = 401;
                return;
            }



            // Besoin de valider et récupèrer les claims du Jeton FranceConnect
            // afin de créer le nouveau id_token compatible Portal
            ClaimsPrincipal? claimsPrincipal = Token.ValidateFranceConnectToken(options.FranceConnectOptions,
                                                                                        franceConnectResult.IdToken);
            if (claimsPrincipal == null)
            {
                context.Response.StatusCode = 401;
                return;
            }


            // le champ nonce est necessaire pour la création du jeton compatible portal
            Claim? nonce = (from claim in claimsPrincipal.Claims
                            where claim.Type == "nonce"
                            select claim)
                            .FirstOrDefault<Claim>();
            if (nonce == null)
            {
                context.Response.StatusCode = 401;
                return;
            }

            // La Façade c'est elle qui fait office d'issuer
            string? issuerEndPoint = null;

            // L'issuer ici doit bien indiquer l'adresse ngrok et non localhost
            // s'il teste avec le compte dev de FranceConnect sinon echec
#if NGROK
            issuerEndPoint = _configuration["ngrok"];
#else
            issuerEndPoint = context.Request.FormatBaseAddress();
#endif


            // Obtient les informations utilisateur à l'aide du jeton d'accès
            string authorization = $"Bearer {franceConnectResult.AccessToken}";
            var UserInfo = await _httpFcClient.GetFranceConnectUserInfo(authorization);
            if (UserInfo == null)
            {
                context.Response.StatusCode = 401;
                return;
            }


            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
                            {
                                                    new Claim("family_name", UserInfo.FamilyName != null ? UserInfo.FamilyName : ""),
                                                    new Claim("given_name", UserInfo.GivenName != null ? UserInfo.GivenName : ""),
                                                    new Claim("email", UserInfo.Email != null ? UserInfo.Email : ""),
                                                    new Claim("gender", UserInfo.Gender != null ? UserInfo.Gender : ""),
                                                    new Claim("birthcountry",UserInfo.BirthCountry != null ? UserInfo.BirthCountry : ""),
                                                    new Claim("birthdate",UserInfo.BirthDate != null ? UserInfo.BirthDate : ""),
                                                    new Claim("birthplace",UserInfo.BirthPlace != null ? UserInfo.BirthPlace : "" ),
                                                    new Claim("preferred_username",UserInfo.PreferredUsername !=null ? UserInfo.PreferredUsername : ""),
                                                    new Claim("sub", UserInfo.Sub != null ? UserInfo.Sub : "" ),
                                                    new Claim("nonce", nonce.Value),
                                                    new Claim("identity_provider", "franceconnect"),
                                                });
            context.User.AddIdentity(claimsIdentity);

            // Crée un nouveau jeton et signe le avec la clé
            // privée contenue dans le certificat X509
            string franceConnectFacadeIdToken = Helpers.Token.CreateTokenAndSignWithX509Cert(options.X509Cert,
                                                                                                     options.FranceConnectOptions.ClientId,
                                                                                                     issuerEndPoint,
                                                                                                     claimsIdentity);

            if (string.IsNullOrEmpty(franceConnectFacadeIdToken))
            {
                context.Response.StatusCode = 401;
                return;
            }


            // Sauvegarde du nouveau jeton
            // pour réutilisation avec le EndPoint /api/token
            /// <see cref="FranceConnectFacadeOpenIdConnectController"/>
            /// <remarks>Voir si c'est judicieux de faire comme cela
            /// en terme de sécurité</remarks>                
            franceConnectResult.IdToken = franceConnectFacadeIdToken;
            context.Items.Add("token", franceConnectResult);
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
                FranceConnectFacadeEndPoint? endpointAttribute =
                    endpoint.Metadata.GetMetadata<FranceConnectFacadeEndPoint>();

                if (endpointAttribute != null)
                {
                    if (endpointAttribute.EndPoint == null)
                    {
                        throw new ArgumentNullException("Point de terminaison de FranceConnectFacade non trouvé");
                    }


                    /// Déclenché si on ajoute l'attribut suivant dans le controller
                    /// FranceConnectFacadeEndPoint(EndPoint = "authorize")
                    /// <see cref="FranceConnectFacadeOpenIdConnectController"/>
                    else if (endpointAttribute.EndPoint.Equals("authorize",
                                                                StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Création de la requête d'authentification compatible FranceConnect
                        CreateQueryForAuthorizeEndPoint(context);
                    }


                    /// Déclenché si on ajoute l'attribut suivant dans le controller
                    /// FranceConnectFacadeEndPoint(EndPoint = "token")
                    /// <see cref="FranceConnectFacadeOpenIdConnectController"/>
                    else if (endpointAttribute.EndPoint.Equals("token",
                                                                StringComparison.InvariantCultureIgnoreCase))
                    {
                        await InvokeTokenEndPointAsync(context, options);
                        if (context.Response.StatusCode == 401)
                        {
                            return;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Point de terminaison FranceConnectFacade non trouvé");
                    }
                }
            }


            await _next(context);
        }
    }

}

