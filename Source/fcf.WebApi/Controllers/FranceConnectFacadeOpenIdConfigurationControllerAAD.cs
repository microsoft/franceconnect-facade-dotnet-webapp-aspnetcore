// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FranceConnectFacade.Identity.Extensions;
using FranceConnectFacade.Identity.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FranceConnectFacade.Identity.Controllers
{
    [Route("commonaad")]
    [Produces("application/json")]
    [ApiController]
    public class FranceConnectFacadeOpenIdConfigurationControllerAAD: ControllerBase
    {
        private readonly ILogger<FranceConnectFacadeOpenIdConfigurationController> _logger;
        private readonly IConfiguration _configuration;

        public FranceConnectFacadeOpenIdConfigurationControllerAAD(ILogger<FranceConnectFacadeOpenIdConfigurationController> logger,
                                  IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        [HttpGet]
        [Route("discovery/keys")]
        [Produces("application/json")]             
        public IActionResult KeysAAD()
        {
            _logger.LogInformation($"Controller: Discovery/Keys");
            var discovery = _configuration
                       .GetSection("DiscoveryKeyModel")
                       .Get<OpenIdKeys>();            
            
            
            return Ok(discovery);
        }

        [HttpGet]
        [Route(".well-known/openid-configuration")]
        public async Task<IActionResult> OpenIdConfigurationAAD()
        {
            
            _logger.LogInformation($"Controller: .well-know/OpenIdConfiguration");


            //HttpClient httpClient = new HttpClient();
            //var response = await httpClient.GetAsync("https://login.microsoftonline.com/38afde78-8c0b-41f8-b6a7-1f145a83aa9f/.well-known/openid-configuration");
            //var openIdConfiguration = await response.Content.ReadAsStringAsync();


            //Changer addresse ngrok pour test avec Portal
            string baseAddress = _configuration["ngrok"];
            

            var openIdConfiguration = _configuration
                                .GetSection("OpenIdConfiguration")
                                .Get<AADDiscovery>();


            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://login.microsoftonline.com/38afde78-8c0b-41f8-b6a7-1f145a83aa9f/.well-known/openid-configuration");
            AADDiscovery fromAad = JsonSerializer.Deserialize<AADDiscovery>(await response.Content.ReadAsStringAsync());

            openIdConfiguration.authorization_endpoint = $"{baseAddress}/api/beta/authorize";


            //openIdConfiguration.jwks_uri = "https://login.microsoftonline.com/38afde78-8c0b-41f8-b6a7-1f145a83aa9f/discovery/v2.0/keys";
            openIdConfiguration.end_session_endpoint = $"{baseAddress}/api/beta/logout";
            openIdConfiguration.userinfo_endpoint = $"{baseAddress}/api/beta/userinfo";
            openIdConfiguration.token_endpoint = $"{baseAddress}/api/beta/token";
            //openIdConfiguration.issuer = "https://login.microsoftonline.com/38afde78-8c0b-41f8-b6a7-1f145a83aa9f/";

            openIdConfiguration.jwks_uri = fromAad.jwks_uri;
            openIdConfiguration.issuer = fromAad.issuer;
            openIdConfiguration.tenant_region_scope = fromAad.tenant_region_scope;
            openIdConfiguration.rbac_url = fromAad.rbac_url;
            openIdConfiguration.check_session_iframe = fromAad.check_session_iframe;
            openIdConfiguration.claims_supported = fromAad.claims_supported;
            openIdConfiguration.cloud_graph_host_name = fromAad.cloud_graph_host_name;
            openIdConfiguration.cloud_instance_name = fromAad.cloud_instance_name;
            openIdConfiguration.device_authorization_endpoint = fromAad.device_authorization_endpoint;
            openIdConfiguration.frontchannel_logout_supported = fromAad.frontchannel_logout_supported;




            return Ok(openIdConfiguration);
            
        }


    }
}
