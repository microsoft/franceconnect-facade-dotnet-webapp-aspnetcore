// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FranceConnectFacade.Identity.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FranceConnectFacade.Identity.Helpers
{
    public class Token
    {
        
        /// <summary>
        /// Création d'un Jeton compatible Portal
        /// </summary>
        /// <param name="x509cert">Certificat X509 contenant la clé privée</param>
        /// <param name="audience">ClientId de FranceConnecy</param>
        /// <param name="issuer">La facade elle même</param>
        /// <param name="claimsIdentity">les claims compatibles Portal/Page</param>
        /// <returns></returns>
        public static string CreateTokenAndSignWithX509Cert(string x509cert,
                                                            string audience,
                                                            string issuer,
                                                            ClaimsIdentity claimsIdentity)
        {
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            
            X509Certificate2 x509 = new X509Certificate2(Convert.FromBase64String(x509cert));

            var x509SecurityKey = new X509SecurityKey(x509);
            SigningCredentials signingCredentials = new SigningCredentials(x509SecurityKey,
                                                                           SecurityAlgorithms.RsaSha256);


            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = audience,
                Issuer = issuer,
                Subject = claimsIdentity,
                IssuedAt = now,
                SigningCredentials = signingCredentials,
            };
            SecurityToken accessToken = jwtHandler.CreateToken(securityTokenDescriptor);
            var tokenString = jwtHandler.WriteToken(accessToken);
            return tokenString;
        }
      
        /// <summary>
        /// Valide le jeton France Connect
        /// </summary>
        /// <param name="config">Paramètre de configuration France Connect</param>
        /// <param name="token">Jeton obtenu via France Connect</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ClaimsPrincipal? ValidateFranceConnectToken(FranceConnectConfiguration config,
                                                                  string token)
        {
            ClaimsPrincipal? claims = null;
            if (config == null) throw new ArgumentNullException("config");
            if (config.Issuer == null) throw new ArgumentNullException("Issuer");
            if (config.ClientId == null) throw new ArgumentNullException("ClientId");
            if (config.ClientSecret == null) throw new ArgumentNullException("ClientSecret");
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException("token");

            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

            try
            {
                claims=jwtHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.ClientSecret)),
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuers = new string[] { config.Issuer },
                    ValidAudiences = new string[] { config.ClientId }
                }, out SecurityToken securJwt);

                
            }
            catch (SecurityTokenException)
            {

                return null;
            }

            return claims;
        }
    }
}
