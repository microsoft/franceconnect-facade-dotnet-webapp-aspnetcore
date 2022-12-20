// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FranceConnectFacade.Identity.Extensions;
using FranceConnectFacade.Identity.Model;
using System.Text;
using System.Text.Json;

namespace FranceConnectFacade.Identity.Services
{
    /// <summary>
    /// Class helper d'appels HTTP pour les points d'entrés
    /// FranceConnect
    /// </summary>
    public class HttpFranceConnectClient : IHttpFranceConnectClient
    {
        
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpFranceConnectClient> _logger;
        private readonly IConfiguration _configuration; 
        public HttpFranceConnectClient(IConfiguration configuration, 
                          IHttpClientFactory httpClientFactory, 
                          ILogger<HttpFranceConnectClient> logger)
        {
            _httpClient = httpClientFactory.CreateClient();            
            _logger = logger;
            _configuration = configuration;

        }
        /// <summary>
        /// Appel point d'entrée /api/v1/token de FranceConnect
        /// </summary>
        /// <param name="payload">Payload</param>
        /// <returns></returns>
        public async Task<FranceConnectAuthenticationResult?> GetFranceConnectToken(string payload)
        {
            if (string.IsNullOrEmpty(payload))
            {
                throw new ArgumentNullException(nameof(payload));
            }

            string TokenEndpoint = _configuration["FranceConnect:TokenEndpoint"];
            if (string.IsNullOrEmpty(TokenEndpoint))
            {
                throw new ArgumentNullException(nameof(TokenEndpoint), "Vous devez rajouter le point d'entrée FranceConnect dans le fichier appsettings.json");
            }

            StringContent content = new StringContent(payload,
                                                      Encoding.UTF8,
                                                      "application/x-www-form-urlencoded");


            var response = await _httpClient.PostAsync(TokenEndpoint, content);
            return await response.Deserialize<FranceConnectAuthenticationResult>();                        
        }

        /// <summary>
        /// Appel point d'entré api/v1/userinfo de FranceConnect
        /// </summary>
        /// <param name="authorizationheader"></param>
        /// <returns></returns>
        public async Task<FranceConnectUserInfo?> GetFranceConnectUserInfo(string authorizationheader)
        {
            if (string.IsNullOrEmpty(authorizationheader))
            {
                throw new ArgumentNullException(nameof(authorizationheader),"Entête FranceConnect non valide");
            }
            string UserInfoEndpoint = _configuration["FranceConnect:UserInfoEndpoint"];
            if (string.IsNullOrEmpty(UserInfoEndpoint))
            {
                throw new ArgumentNullException(nameof(UserInfoEndpoint), "Vous devez rajouter le point d'entrée FranceConnect dans le fichier appsettings.json");
            }
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", authorizationheader);
            var response = await _httpClient.GetAsync(UserInfoEndpoint);
            return await response.Deserialize<FranceConnectUserInfo>();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }
    }
}
