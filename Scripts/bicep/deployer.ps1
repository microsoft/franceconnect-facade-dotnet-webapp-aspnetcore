# Description: Deploys the FranceConnect Facade services to Azure
# SubscriptionId: The Azure subscription Id
# FranceConnectClientId: The FranceConnect client Id
# FranceConnectClientSecret: The FranceConnect client secret
# ResourceGroupName: The resource group name
# TenantId: The Azure Active Directory tenant Id
# Location: The Azure location
# IsLinuxWebApp: Indicates if the web app is a Linux web app or Windows web app
param ([string][Parameter(Mandatory=$true)]$FranceConnectClientId,
       [string][Parameter(Mandatory=$true)]$FranceConnectClientSecret,
       [string][Parameter(Mandatory=$true)]$ResourceGroupName,
       [string][Parameter(Mandatory=$true)]$SubscriptionId,       
       [string][Parameter(Mandatory=$true)]$TenantId,
       [string][Parameter(Mandatory=$true)]$location,
       [bool][Parameter(Mandatory=$false)]$IsLinuxWebApp=$false)

# Connecte l'utilisateur à Azure
$userInfo=$(az login --tenant $TenantId) | convertfrom-json
# Récupère l'objet id de l'utilisateur connecté 
# Nous servira à donner les droits de gestion du keyvault 
# à l'utilisateur connecté
$oid=az ad user show --id $userInfo.user.name --query id

# Définir la souscription par défaut
az account set --subscription $SubscriptionId


write-host  "Creating '$ResourceGroupName' resource group in '$Location'" 
az group create -n $ResourceGroupName -l $Location 

# Définir le groupe de ressources par défaut utile pour le déploiement biceps
write-host "Set default '$ResourceGroupName' as resource group for bicep deployment"
az configure --defaults group=$ResourceGroupName

# Déployer les services 
write-host  "Deploying services to '$ResourceGroupName'" 
az deployment group create --name "Services.Main.FranceConnectFacade" `
                    --template-file "main.bicep" `
                    --parameters location=$Location  `
                    franceConnectClientId=$FranceConnectClientId `
                    franceConnectClientSecret=$FranceConnectClientSecret `
                    isLinuxWebApp=$IsLinuxWebApp



# Retrouve le prefix unique à partir de la srotie du déploiement
$uniquePrefix=$(az deployment group show `
                -g $resourceGroupName `
                -n  "Services.Main.FranceConnectFacade" `
                 --query properties.outputs | convertfrom-json).uniqueNamePrefix.value


# Construire le nom du keyvault            
$keyvaultname=$(.\Get-Name.ps1 -service "keyvault") + $uniquePrefix

$certname="keyvaultcert"


# Donne les droits de gestion du keyvault à l'utilisateur connecté
az keyvault set-policy -n $keyvaultname `
                        --object-id $oid `
                        -g $ResourceGroupName `
                        --certificate-permissions "all" `
                        --key-permissions "all" `
                        --secret-permissions "all"

write-host  "Create a self signin certificat"
az keyvault certificate create --vault-name $keyvaultname `
                               --name $certname `
                               --policy "@defaultcertificatepolicy.json"


# Construire le nom de la webapp
$webapiName=$(.\Get-Name.ps1 -service "website") + $uniquePrefix

write-host  "Deploying '$webapiName' web app"
$sourcePath="..\..\Source\fcf.WebApi\"
$destinationPath=".\publier"
$zipFile=".\publier\fcf.zip"


dotnet publish  $sourcePath -c Release -o $destinationPath

Compress-Archive -Path ".\publier\*.*" -DestinationPath $zipFile -force

az webapp deployment source config-zip `
          -g $resourceGroupName -n $webapiName --src $zipFile         

