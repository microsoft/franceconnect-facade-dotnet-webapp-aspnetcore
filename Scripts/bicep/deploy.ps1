param ([string][Parameter( Mandatory=$true)]$clientId,
       [string][Parameter( Mandatory=$true)]$clientSecret)


$tenantId="<your_tenant_id>"
$subscription="<your_subscription_id>"
$location="francecentral"
$rgname="<your_resource_group_name>"

#az login --tenant $tenantId

az account set --subscription $subscription


write-host  "Creating '$rgname'  resource group" 
az group create -n $rgname -l $location 

write-host "Configure localy the default resource group '$rgname' "
az configure --defaults group=$rgname

write-host  "Deploying services to '$rgname' resource group" 
az deployment group create --name "Main" `
                    --template-file "main.bicep" `
                    --parameters location=$location  `
                    clientId=$clientId `
                    clientSecret=$clientSecret 
    