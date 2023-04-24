// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace FranceConnectFacade.Identity.Model
{
    

    public class OpenIdDiscoveryKeys
    {
        public Key[]? keys { get; set; }
    }

    public class Key
    {
        public string? kty { get; set; }
        public string? alg { get; set; }
        public string? use { get; set; }
        public string? kid { get; set; }
        //public string? x5t { get; set; }
        public string? n { get; set; }
        public string? e { get; set; }
        public string[]? x5c { get; set; }        
    }

}
