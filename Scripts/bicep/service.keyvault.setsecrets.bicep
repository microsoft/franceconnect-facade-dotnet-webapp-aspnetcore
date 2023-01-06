@secure()
param FranceConnectClientSecret string
@secure()
param FranceConnectClientId string
param keyvaultName string

resource fcClientId 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: '${keyvaultName}/ClientId'
  properties: {
    value: FranceConnectClientId
  }
}

resource fcClientSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: '${keyvaultName}/ClientSecret'
  properties: {
    value: FranceConnectClientSecret
  }
}
