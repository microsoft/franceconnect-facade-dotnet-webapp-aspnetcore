// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace FranceConnectFacade.Identity.Model
{
    /// <summary>
    /// Claims utilisateur inclus dans le Jeton JWT
    /// </summary>
    public class FranceConnectUserInfo
    {
        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; }
        [JsonPropertyName("family_name")]
        public string? FamilyName { get; set; }
        [JsonPropertyName("birthdate")]
        public string? BirthDate { get; set; }
        [JsonPropertyName("gender")]
        public string? Gender {get; set; }
        [JsonPropertyName("birthplace")]
        public string? BirthPlace { get; set; }
        [JsonPropertyName("birthcountry")]
        public string? BirthCountry { get; set; }
        [JsonPropertyName("preferred_username")]
        public string? PreferredUsername { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("sub")]
        public string? Sub { get; set; }
    }
}
