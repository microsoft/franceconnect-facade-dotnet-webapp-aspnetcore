// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace FranceConnectFacade.Identity.Model
{
    /// <summary>
    /// Modèle de données correspondant au format
    /// d'un jeton JWT
    /// </summary>
    public class FranceConnectAuthenticationResult
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("ext_expires_in")]
        public int ExtexpiresIn { get; set; }

        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
        [JsonPropertyName("state")]
        public string? State { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize<FranceConnectAuthenticationResult>(this);
        }
    }


   

}
