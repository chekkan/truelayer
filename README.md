# Payments Challenge

## Running Locally

Use dotnet secret manager to store the client id and client secret information for TrueLayer
```console
dotnet user-secrets set "TrueLayer:ClientId" "xxxxx" --project src/Api
dotnet user-secrets set "TrueLayer:ClientSecret" "xxxxx" --project src/Api
```