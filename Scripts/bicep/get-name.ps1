# Description: Get the name of a service from the services.properties.json file
# Param: The service name
param ([string][Parameter( Mandatory=$true)]$service)

$currentlocation=Get-Location
$namesfilepath="$currentlocation\services.properties.json"
$names = ([System.IO.File]::ReadAllText($namesfilepath)  | ConvertFrom-Json)


return $names.parameters.$service.value.name