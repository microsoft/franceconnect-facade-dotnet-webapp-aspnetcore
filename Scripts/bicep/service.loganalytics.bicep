param location string
param logsAnalyticsName string 


var servicesProperties =json(loadTextContent('services.properties.json'))

resource loganalytics 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name:logsAnalyticsName
  location:location
  properties: servicesProperties.parameters.loganalytics.value.properties  
  tags: {
    project:'FranceConnectFacade'
  }
}
