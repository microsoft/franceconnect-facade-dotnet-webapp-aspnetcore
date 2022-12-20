// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FranceConnectFacade.Identity.Services;

namespace FranceConnectFacade.Identity.WebApi.Middleware
{
    /// <summary>
    /// Options de configuration utilisées dans 
    /// le middleware
    /// </summary>
    public class FranceConnectFacadeConfigurationOptions     {
        public FranceConnectConfiguration? FranceConnectOptions { get; set; }
        public string? X509Cert { get; set; }        
    }
}
