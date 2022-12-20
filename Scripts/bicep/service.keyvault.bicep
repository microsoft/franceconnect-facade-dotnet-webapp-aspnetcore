param location string
param keyvaultName string 
param identityName string

@description('Récupère les informations de l`identité afin de les affecter aux politiques du Key Vaul')
resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' existing = {
  name:identityName
}
var tenantId=identity.properties.tenantId
var principalId=identity.properties.principalId


var servicesProperties =json(loadTextContent('Services.Properties.json'))

//TODO : Change policies to RBAC
resource keyvault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: keyvaultName
  location:location
  tags: {
    project:'FranceConnectFacade'
  }
  properties: {
    sku: servicesProperties.parameters.keyvault.value.sku
    tenantId:tenantId
    enabledForDeployment:true
    accessPolicies: [
      {
        objectId:principalId
        tenantId:tenantId
        permissions:{
          certificates: [
            'get'
            'list'
          ]
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
  }
}


