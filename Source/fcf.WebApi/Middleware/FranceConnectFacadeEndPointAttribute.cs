// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace FranceConnectFacade.Identity.Middleware
{
    /// <summary>
    /// Attribut pour <see cref="FranceConnectFacadeOpenIdConnectMiddleware"</see> 
    /// </summary>
    /// <remarks>Peut prendre deux valeurs : authorize et token 
    /// <see cref="FranceConnectFacadeOpenIdConnectController" </see>
    /// </remarks>    
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class FranceConnectFacadeEndPoint : Attribute
    {
        public string? EndPoint { get; set; }
        public FranceConnectFacadeEndPoint() { }
    }
}
