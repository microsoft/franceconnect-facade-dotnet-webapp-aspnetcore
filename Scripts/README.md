# About the series of Bicep scripts for the FranceConnect Facade (FCF) code sample

**The [Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/overview?tabs=bicep) scripts available under the Bicep folder are a Work In Progress (WIP).**

Bicep is a domain-specific language (DSL) that uses declarative syntax to deploy Azure resources. In a Bicep file, you define the infrastructure you want to deploy to Azure, and then use that file throughout the development lifecycle to repeatedly deploy your infrastructure. Your resources are deployed in a consistent manner. 

As such, this series of scripts will help fulfilling the prerequites to deploy the code sample in your Azure subscription. Please refer to the ["Getting Started" guide](https://github.com/microsoft/franceconnect-facade-dotnet-webapp-aspnetcore/tree/master/Documentation).



##  What the deployment scripts does?

- Create an [Azure User Managed Identity](https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview) to access silently to the Azure Keyvault. With this identity, managed directly by Azure, no need to store a secret in code. 

- Create an [Azure Keyvault](https://learn.microsoft.com/en-us/azure/key-vault/general/overview), 

    - Enable access policies (Get, List) to the User Managed Identity.
    - Create a self signin certificat directly from Azure keyvault.
    - Add FranceConnect client id and client Secret as secrets.

- Create an [Azure Web App](https://learn.microsoft.com/en-us/azure/app-service/) (either Linux or Windows), and assign it the User Managed Identity.


- Create an [Azure AppInsights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview?tabs=net) in order to monitor/debug the the Web App.



## Run the deployment script

First  collect the following information from the Azure portal.

- The Azure Active Directory Tenant Id
- The Azure Subscription Id

Then add these following lines in a powershell script and replace the parameters accordinly.

$FranceConnectClientId="[FRANCE CONNECT ACCOUNT]"
$FranceConnectClientSecret="[FRANCE CONNECT CLIENT ID]"

$AzureActiveDirectoryTenantId="[TENANT ID]

$AzureSubscriptionId="[SUBSCRIPTION ID]"

$ResourceGroupName="[RESOURCE GROUP NAME]"

$location="[LOCATION]"

$IsLinuxWebApp=$true/$false

.\Deployer.ps1 -FranceConnectClientId $FranceConnectClientId `
               -FranceConnectClientSecret $FranceConnectClientSecret `
               -ResourceGroupName $ResourceGroupName `
               -SubscriptionId $AzureSubscriptionId `
               -TenantId $AzureActiveDirectoryTenantId `
               -Location $location `
               -IsLinuxWebApp $IsLinuxWebApp


> Note: The bicep script use  the 
