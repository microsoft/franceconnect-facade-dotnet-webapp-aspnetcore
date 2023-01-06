param location string
@secure()
param franceConnectClientId string
@secure()
param franceConnectClientSecret string
param servicesProperties object =json(loadTextContent('services.properties.json'))
param isLinuxWebApp bool = false

@description('Création chaine unique pour nommer les ressources Azure')
var uniqueNamePrefix = uniqueString(resourceGroup().id)

output uniqueNamePrefix string = uniqueNamePrefix

@description('Création de l`identité managé')
module identity 'service.managed.identity.bicep' = {
  name:'Service.Managed.Identity'
  params:{
    location:location    
    identityName:'${servicesProperties.parameters.managedidentity.value.name}${uniqueNamePrefix}'
  }
}

@description('Création du Key Vault')
module keyvault 'service.keyvault.bicep' = {
  name:'Service.KeyVault'
  params: {
    location: location
    identityName:'${servicesProperties.parameters.managedidentity.value.name}${uniqueNamePrefix}'
    keyvaultName:'${servicesProperties.parameters.keyvault.value.name}${uniqueNamePrefix}'
  }
  dependsOn:[
    identity
  ]
} 

@description('Ajout des secrets dans le key vault')
module keyvaultsecret 'service.keyvault.setsecrets.bicep' = {
  name:'Service.KeyVault.SetSecrets'
  params: {
    FranceConnectClientId:franceConnectClientId
    FranceConnectClientSecret: franceConnectClientSecret
    keyvaultName:'${servicesProperties.parameters.keyvault.value.name}${uniqueNamePrefix}'
  }
  dependsOn: [
    keyvault
  ]
}

module logAnalytics 'service.loganalytics.bicep' = {
  name:'Service.LogAnalytics'
  params: {
    location: location
    logsAnalyticsName:'${servicesProperties.parameters.loganalytics.value.name}${uniqueNamePrefix}'
  }
}

module appInsights 'service.appinsights.bicep' = {
  name:'Service.AppInsights'
  params: {
    location: location
    appInsightsName:'${servicesProperties.parameters.appinsights.value.name}${uniqueNamePrefix}'
    logsAnalyticsName:'${servicesProperties.parameters.loganalytics.value.name}${uniqueNamePrefix}'
  }
  dependsOn:[
    logAnalytics
  ]
}


@description('Création de la WebApp qui va héberger l`API Facade FranceConnect')
module webApp 'service.appservice.bicep' = if (!isLinuxWebApp) {
  name:'Service.AppService'
  params: {
    location: location
    appServicePlanName:'${servicesProperties.parameters.appserviceplan.value.name}${uniqueNamePrefix}'
    identityName:'${servicesProperties.parameters.managedidentity.value.name}${uniqueNamePrefix}'
    keyvaultName:'${servicesProperties.parameters.keyvault.value.name}${uniqueNamePrefix}'
    websiteName:'${servicesProperties.parameters.website.value.name}${uniqueNamePrefix}'
    appInsightsName:'${servicesProperties.parameters.appinsights.value.name}${uniqueNamePrefix}'
  }
  dependsOn:[
    identity
    keyvault
    appInsights
  ]
}

module webApplnx 'service.appservicelnx.bicep' = if (isLinuxWebApp) {
  name:'Service.AppService.linux'
  params: {
    location: location
    appServicePlanName:'${servicesProperties.parameters.appserviceplan.value.name}${uniqueNamePrefix}'
    identityName:'${servicesProperties.parameters.managedidentity.value.name}${uniqueNamePrefix}'
    keyvaultName:'${servicesProperties.parameters.keyvault.value.name}${uniqueNamePrefix}'
    websiteName:'${servicesProperties.parameters.website.value.name}${uniqueNamePrefix}'
    appInsightsName:'${servicesProperties.parameters.appinsights.value.name}${uniqueNamePrefix}'
  }
  dependsOn:[
    identity
    keyvault
    appInsights
  ]
}
