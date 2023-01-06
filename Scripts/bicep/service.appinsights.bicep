param location string
param logsAnalyticsName string 
param appInsightsName string

var servicesProperties =json(loadTextContent('services.properties.json'))

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name:logsAnalyticsName  
}


resource appInsights 'Microsoft.Insights/components@2020-02-02' =  {
  location:location
  name:appInsightsName
  kind:servicesProperties.parameters.appinsights.value.kind
  tags: {
    project:'FranceConnectFacade'
  }
  properties:{
    Application_Type:servicesProperties.parameters.appinsights.value.properties.Application_Type
    IngestionMode:servicesProperties.parameters.appinsights.value.properties.IngestionMode
    WorkspaceResourceId: logAnalytics.id
  }
}
