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
#if DEBUG
    builder.Services.AddSwaggerGen();
#endif

bool UseDevelopmentCertificate = builder.Configuration.IsSettingEnabled("X509:UseDevelopmentCertificate");
if (UseDevelopmentCertificate)
{
    builder.Configuration.AddDevelopmentCertificate();
}
else
{
    builder.Configuration.AddCertificateFromKeyVault();
}
builder.Services.AddTransient<IHttpFranceConnectClient, HttpFranceConnectClient>();
builder.Services.AddHttpClient<HttpFranceConnectClient>();

var fcConfig = builder.Configuration
                                .GetSection("FranceConnect")
                                .Get<FranceConnectConfiguration>();

// Lecture des parametres de configuration FranceConnect
fcConfig.ClientSecret = builder.Configuration["FranceConnect:ClientSecret"];

fcConfig.ClientId = builder.Configuration["FranceConnect:ClientId"];

builder.Services.AddScoped<FranceConnectFacadeConfigurationOptions>((provider)=>
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
