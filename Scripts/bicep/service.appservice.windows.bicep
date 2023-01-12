
param location string 
param appServicePlanName string
param websiteName string 
param identityName string
param keyvaultName string
param appInsightsName string



var servicesProperties =json(loadTextContent('services.properties.json'))

resource serviceplan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name:appServicePlanName
  location:location
  kind: servicesProperties.parameters.appserviceplan.value.kind
  sku: servicesProperties.parameters.appserviceplan.value.sku
  tags: {
    project:'FranceConnectFacade'
  }
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' existing = {
  name:identityName
}


resource webSite 'Microsoft.Web/sites@2021-02-01' = {
  name:websiteName
  location:location
  kind: servicesProperties.parameters.website.value.kind
  properties: {
    serverFarmId: serviceplan.id   
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identity.id}': {}
    }
  }
  tags: {
    project:'FranceConnectFacade'
  }
}


resource keyvault 'Microsoft.KeyVault/vaults@2022-07-01'  existing = {
  name: keyvaultName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource appsettings 'Microsoft.Web/sites/config@2021-02-01' = {
  parent:webSite  
  name : 'appsettings'
  
  properties: {
    APPINSIGHTS_INSTRUMENTATIONKEY:appInsights.properties.InstrumentationKey
    APPINSIGHTS_PROFILERFEATURE_VERSION: '1.0.0'
    APPLICATIONINSIGHTS_CONNECTION_STRING: appInsights.properties.ConnectionString
    WEBSITE_LOAD_USER_PROFILE:'1'    
    CertificateNameKeyVault:'keyvaultcert'
    AzureKeyVaultEndpoint:keyvault.properties.vaultUri
    ManagedIdentityId:identity.properties.clientId
  }
}
