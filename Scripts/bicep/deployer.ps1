param ([string][Parameter( Mandatory=$false)]$FranceConnectClientId,
       [string][Parameter( Mandatory=$false)]$FranceConnectClientSecret,
       [bool][Parameter( Mandatory=$true)]$isLinuxWebApp)

$FranceConnectClientId="211286433e39cce01db448d80181bdfd005554b19cd51b3fe7943f6b3b86ab6e"
$FranceConnectClientSecret="2791a731e6a59f56b6b4dd0d08c9b1f593b5f3658b9fd731cb24248e2669af4b"

$AzureActiveDirectoryTenantId="1ef8d71b-7c85-420d-be7d-207d2959a5e6"
$AzureSubscriptionId="6a031348-279f-40ed-be25-0b3787f3ae15"
$location="francecentral"
$rgname="FranceConnectFacadeLnx03-rg"


az login --tenant $AzureActiveDirectoryTenantId

az account set --subscription $AzureSubscriptionId


write-host  "Creating '$rgname' resource group in '$location'" 
az group create -n $rgname -l $location 

write-host "Set default '$rgname' as resource group for bicep deployment"
az configure --defaults group=$rgname

write-host  "Deploying services to '$rgname'" 
az deployment group create --name "Services.Main.FranceConnectFacade" `
                    --template-file "main.bicep" `
                    --parameters location=$location  `
                    franceConnectClientId=$FranceConnectClientId `
                    franceConnectClientSecret=$FranceConnectClientSecret `
                    isLinuxWebApp=$isLinuxWebApp


    
write-host  "Create a self certificat"
$uniquePrefix=$(az deployment group show `
                -g $rgname `
                -n  "Services.Main.FranceConnectFacade" `
                 --query properties.outputs | convertfrom-json).uniqueNamePrefix.value

# Retrouve le nom du keyvault à partir de l'output du déploiement
$keyvaultname="fcfkeyvault"+$uniquePrefix
$certname="keyvaultcert"

# A Récupèrer l'objet id de l'utilisateur connecté (admin) 
# sur le portail Azure Active Directory
$oid="316d6a30-1fc2-45fa-80b4-9ec4ae440590"
# Donne les droits de gestion du keyvault à l'utilisateur connecté
az keyvault set-policy -n $keyvaultname `
                        --object-id $oid `
                        -g $rgname `
                        --certificate-permissions "all" `
                        --key-permissions "all" `
                        --secret-permissions "all"

az keyvault certificate create --vault-name $keyvaultname `
                               --name $certname `
                               --policy "@defaultcertificatepolicy.json"

 
$webapiName="fcfapi"+$uniquePrefix

write-host  "Deploying '$webapiName' web app"
$sourcePath="..\..\Source\fcf.WebApi\"
$destinationPath=".\publier"
$zipFile=".\publier\fcf.zip"

dotnet publish  $sourcePath -c Release -o $destinationPath

Compress-Archive -Path ".\publier\*.*" -DestinationPath $zipFile -force

az webapp deployment source config-zip `
          -g $rgname -n $webapiName --src $zipFile         

