param location string
@secure()
param clientId string
@secure()
param clientSecret string

param services object =json(loadTextContent('services.properties.json'))

@description('Création de l`identité managé')
module identity 'service.managed.identity.bicep' = {
  name:'Service.Managed.Identity'
  params:{
    location:location    
    identityName:services.parameters.managedidentity.value.name
  }
}

@description('Création du Key Vault')
module keyvault 'service.keyvault.bicep' = {
  name:'Service.KeyVault'
  params: {
    location: location
    identityName:services.parameters.managedidentity.value.name
    keyvaultName:services.parameters.keyvault.value.name   
  }
  dependsOn:[
    identity
  ]
} 

module keyvaultsecret 'service.keyvault.setsecrets.bicep' = {
  name:'Service.KeyVault.SetSecrets'
  params: {
    clientId:clientId
    clientSecret: clientSecret
    keyvaultName:services.parameters.keyvault.value.name 
  }
  dependsOn: [
    keyvault
  ]
}
