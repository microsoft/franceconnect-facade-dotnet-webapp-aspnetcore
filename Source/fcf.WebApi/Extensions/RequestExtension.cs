// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;

namespace FranceConnectFacade.Identity.Extensions
{
    public static class RequestExtension
    {
        /// <summary>
        /// Récupère les paramètre de la requête encours
        /// ie (?param1=value1&param2=value2)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string? GetQuery(this HttpRequest request)
        {
            QueryString qs = request.QueryString;            
            return qs.Value; 
            
        }
        /// <summary>
        /// Récupère le body de la requête encours
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<string> GetBodyAsync(this HttpRequest request)
        {
            
            
            var result = await request.BodyReader.ReadAsync();
            string fromBody = ASCIIEncoding.UTF8.GetString(result.Buffer);            
            request.BodyReader.AdvanceTo(result.Buffer.Start,
                                         result.Buffer.End);
            return fromBody;
        }
        /// <summary>
        /// Construit l'adresse de base d'une requête
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string FormatBaseAddress(this HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}";
        }
    }
}
