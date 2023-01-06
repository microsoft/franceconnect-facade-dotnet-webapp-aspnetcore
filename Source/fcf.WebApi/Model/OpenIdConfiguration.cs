// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//https://openid.net/specs/openid-connect-discovery-1_0.html

using System.Text.Json.Serialization;

namespace FranceConnectFacade.Identity.Model
{
    /// <summary>
    /// Modèle de données 
    /// </summary>
    /// https://openid.net/specs/openid-connect-discovery-1_0.html
    public class OpenIdConfiguration
    {

        [JsonPropertyName("token_endpoint")]
        public string? TokenEndpoint { get; set; }

        //REQUIRED
        [JsonPropertyName("jwks_uri")]
        public string? JwksUri { get; set; }
        //REQUIRED
        [JsonPropertyName("response_modes_supported")]
        public string[]? ResponseModesSupported { get; set; }

        //OPTIONAL
        [JsonPropertyName("acr_values_supported")]
        public string[]? AcrValuesSupported { get; set; }

        //REQUIRED
        [JsonPropertyName("subject_types_supported")]
        public string[]? SubjectTypesSupported { get; set; }
        //REQUIRED
        [JsonPropertyName("id_token_signing_alg_values_supported")]
        public string[]? IdTokenSigningAlgValuesSupported { get; set; }
        //REQUIRED
        [JsonPropertyName("response_types_supported")]
        public string[]? ResponseTypesSupported { get; set; }

        [JsonPropertyName("scopes_supported")]
        public string[]? ScopesSupported { get; set; }

        //REQUIRED
        [JsonPropertyName("issuer")]
        public string? Issuer { get; set; }

        //RECOMMENDED
        [JsonPropertyName("userinfo_endpoint")]
        public string? UserInfoEndpoint { get; set; }

        //REQUIRED
        [JsonPropertyName("authorization_endpoint")]
        public string? AuthorizationEndpoint { get; set; }

        [JsonPropertyName("http_logout_supported")]
        public bool HttpLogoutSupported { get; set; }

        [JsonPropertyName("end_session_endpoint")]
        public string? EndSessionEndpoint { get; set; }

        //RECOMMENDED
        [JsonPropertyName("claims_supported")]
        public string[]? ClaimsSupported { get; set; }


    }


    

}
