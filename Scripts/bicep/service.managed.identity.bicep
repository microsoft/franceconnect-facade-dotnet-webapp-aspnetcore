param location string
@minLength(5)
@maxLength(24)
param identityName string 

@description('Identité qui permettra de ce connecter de manière transparente au Key Vault')
resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  location:location
  name:identityName  
  tags: {
    project:'FranceConnectFacade'
  }
}
