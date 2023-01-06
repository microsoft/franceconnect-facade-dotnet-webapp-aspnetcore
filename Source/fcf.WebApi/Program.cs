// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authorization/Policy/src/AuthorizationMiddleware.cs

using FranceConnectFacade.Identity.Services;
using FranceConnectFacade.Identity.WebApi.Extensions;
using FranceConnectFacade.Identity.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IHttpFranceConnectClient, HttpFranceConnectClient>();
builder.Services.AddHttpClient<HttpFranceConnectClient>();

var fcConfig = builder.Configuration
                                .GetSection("FranceConnect")
                                .Get<FranceConnectConfiguration>();
bool UseDevelopmentCertificate = builder.Configuration.IsSettingEnabled("UseDevelopmentCertificate");

if (UseDevelopmentCertificate)
{
    builder.Configuration.AddDevelopmentCertificate();
    
}
else
{
    builder.Configuration.AddCertificateFromKeyVault();      
}

fcConfig.ClientSecret = builder.Configuration["ClientSecret"];
fcConfig.ClientId = builder.Configuration["ClientId"];



builder.Services.AddSingleton<FranceConnectFacadeConfigurationOptions>((provider)=>
{

    return new FranceConnectFacadeConfigurationOptions
    {
        FranceConnectOptions = fcConfig,
        // Certificat utilisé pour signer le jeton
        X509Cert = builder.Configuration[builder.Configuration.GetCertificateName()]
    };
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// Utilisation du middleware
app.UseFranceConnectFacade();

app.MapControllers();

app.Run();



