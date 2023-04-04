# Services-Inventory

.NET 7 Inventory Web API for Small Microservices Solution

```dotnetcli
dotnet pack -o C:\LordKrishna\Packages\
```

## Create and publish package to GitHub using PowerShell

```powershell
$version="1.0.8"
$owner="Microservices-for-Small-App"
$username="vishipayyallore"
$repo="Services-Inventory"
$libname="Inventory.Contracts"
$gh_pat="ghp_Your_GitHib_Classic_PAT"

dotnet clean
dotnet build -c Release
dotnet pack --configuration Release -o C:\LordKrishna\SSP\Packages

dotnet nuget push C:\LordKrishna\SSP\Packages\$libname.$version.nupkg --source "gHmicroservices" --api-key $gh_pat
```
