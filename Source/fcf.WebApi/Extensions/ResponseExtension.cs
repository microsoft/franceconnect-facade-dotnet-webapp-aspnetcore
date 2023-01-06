// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using System.Text.Json;

namespace FranceConnectFacade.Identity.Extensions
{
    public static class ResponseExtension
    {
        public static async Task<T?> Deserialize<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return default(T);
        }
    }
}
