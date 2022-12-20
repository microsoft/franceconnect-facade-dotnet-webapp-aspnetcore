// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FranceConnectFacade.Identity.Model;

namespace FranceConnectFacade.Identity.Services
{
    public interface IHttpFranceConnectClient
    {

        Task<FranceConnectAuthenticationResult?> GetFranceConnectToken(string frombody);        
        Task<FranceConnectUserInfo?> GetFranceConnectUserInfo(string authorizationheader);
        Task Logout();
    }
}
