// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;

namespace FranceConnectFacade.Identity.Helpers
{
    public class OAuth
    {
        private const string pkce= "code_verifier";
        private const char separator = '&';
        /// <summary>
        /// Supprime du corps du message le paramètre 
        /// code_verifier car FC ne le supporte pas
        /// </summary>
        /// <param name="frombody">Corps du message</param>
        /// <returns>Le nouveau corps de message</returns>
        public static string DisablePKCE(string frombody)
        {
            if(frombody.Contains(pkce))
            {
                string[] splittedBody = frombody.Split(separator);
                StringBuilder sb = new StringBuilder();
                foreach (string s in splittedBody)
                {
                    if (!s.Contains(pkce))
                    {
                        sb.Append(s);
                        sb.Append(separator);
                    }
                }
                string f = sb.ToString();
                // Supprime dernier caractère
                return f.Remove(f.Length - 1);
            }

            return frombody;
        }
           
    }
}
