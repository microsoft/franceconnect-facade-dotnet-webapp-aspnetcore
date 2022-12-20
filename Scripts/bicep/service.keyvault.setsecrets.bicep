@secure()
param clientSecret string
@secure()
param clientId string
param keyvaultName string

resource fcClientId 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: '${keyvaultName}/ClientId'
  properties: {
    value: clientId
  }
}

resource fcClientSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: '${keyvaultName}/ClientSecret'
  properties: {
    value: clientSecret
  }
}
