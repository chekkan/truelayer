# Payments Challenge

## Running Locally

Use dotnet secret manager to store the client id and client secret information for TrueLayer
```console
dotnet user-secrets set "TrueLayer:ClientId" "xxxxx" --project src/Api
dotnet user-secrets set "TrueLayer:ClientSecret" "xxxxx" --project src/Api
```

**Note**
If using Postman to call `/v1/providers/{id}/connect`, make sure to turn off "Automatically follow redirects" under 
_Settings_ tab. Otherwise, you will be redirected to the auth link. 