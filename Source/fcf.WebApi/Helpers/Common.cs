// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace FranceConnectFacade.Identity.Helpers
{
    public class Common
    {
        /// <summary>
        /// Extrait la valeur définie par la clé
        /// dans les paramètres de la requête
        /// </summary>
        /// <param name="key">Clé à extraire</param>
        /// <param name="fromquery">paramètres de la requête </param>
        /// <returns></returns>
        public static string? GetValue(string key, string fromquery)
        {
            string[] splittedQuery = fromquery.Split('&');
            foreach (string s in splittedQuery)
            {
                if (s.Contains(key))
                {
                    return s.Split('=')[1];
                }
            }
            return null;
        }
    }
}
