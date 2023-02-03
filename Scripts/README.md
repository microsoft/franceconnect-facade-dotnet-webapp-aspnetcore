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

To deploy the resources required by the FranceConnect Facade (FCF), proceed with the following steps:
- First, gather the following information from the Azure portal:

    - Your Azure Active Directory (Azure AD) tenant ID, referred as to <your_tenant_id>. see section Create your Azure AD tenant above to create one. 
    - Your Azure subscription ID, referred as to <your_subscription_id> below. See Azure subscription in section Fulfill the prerequisites for your Azure testing environment above.
- Now open a PowerShell console and set the following parameters accordingly:

```
$AzureActiveDirectoryTenantId="<your_tenant_id>"
$AzureSubscriptionId="<your_subscription_id>"
```
Replace the values in brackets by your own values to reflect your specific configuration in the Azure cloud.

- Specify the resource group (to create) and the location to which to deploy the FranceConnect façade (FCF):

```
$ResourceGroupName="<your_resourcegroup_name>"
$location="<your_location>"
```
- Set your FranceConnect (dev) account, i.e., the corresponding Client id and Client Secret. 
```
$FranceConnectClientId="<your_franceconnect_clientid>"
$FranceConnectClientSecret="<your_franceconnect_clientsecret>"
```
For example, the FranceConnect dev account through the available so-called “integration key for public use”, in this illustration:
```
$FranceConnectClientId = "211286433e39cce01db448d80181bdfd005554b19cd51b3fe7943f6b3b86ab6e"
$FranceConnectClientSecret = "2791a731e6a59f56b6b4dd0d08c9b1f593b5f3658b9fd731cb24248e2669af4b"
```
- Specify whether or not you’d like to deploy a Linux ($true), vs. Windows ($false), Web application :
```
$IsLinuxWebApp=[$true|$false]
```
- Eventually execute the provided PowerShell launcher script Deployer.ps1 with the following command parameters. This PowerShell script is located in the folder bicep located under Scripts:
```
.\Deployer.ps1 -FranceConnectClientId $FranceConnectClientId `
    -FranceConnectClientSecret $FranceConnectClientSecret `
    -ResourceGroupName $ResourceGroupName `
    -SubscriptionId $AzureSubscriptionId `
    -TenantId $AzureActiveDirectoryTenantId `
    -Location $location `
       -IsLinuxWebApp $IsLinuxWebApp
```
This script executes the main.bicep script, which runs in sequence the service.managed.identity.bicep file for the managed identity, the service.keyvault.bicep file for the keyvault, the service.keyvault.setsecrets.bicep file for the certificate, etc. 