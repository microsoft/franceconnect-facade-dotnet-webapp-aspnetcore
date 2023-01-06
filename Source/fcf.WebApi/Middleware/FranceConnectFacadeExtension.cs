// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using FranceConnectFacade.Identity.Middleware;

namespace FranceConnectFacade.Identity.WebApi.Middleware
{
    /// <summary>
    /// Extension du middleware FCF
    /// </summary>
    public static class FCFOpenIdConnectMiddlewareExtensions
    {
        public static IApplicationBuilder UseFranceConnectFacade(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FranceConnectFacadeOpenIdConnectMiddleware>();
        }
       
    }

    
}
