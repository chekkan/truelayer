# Payments Challenge

## Running Locally

There are 3 hard coded users.

| Username | Password |
|----------|----------|
| john     | doe      |
| john1    | doe1     |
| john2    | doe2     |

The application uses Sqlite database at location `./src/Api/payments-challenge.db`. The users are seeded into the 
application when application runs.

1. Use dotnet secret manager to store the client id and client secret information for TrueLayer

	```console
	dotnet user-secrets set "TrueLayer:ClientId" "xxxxx" --project src/Api
	dotnet user-secrets set "TrueLayer:ClientSecret" "xxxxx" --project src/Api
	```

1. Add TrueLayer redirect url `http://localhost:5000/callback`

1. Run API using dotnet cli
	
	```console
	dotnet run --project src/Api
	```

Use Basic Authentication in order to perform the following operations

1. Connect a bank account with user

	```console
	curl -I --request POST 'https://localhost:5001/v1/providers/mock/connect' \ 
--header 'Authorization: Basic am9objpkb2U=' --header 'Content-Length: 0'
	```

	**Note**
	If using Postman to call `/v1/providers/{id}/connect`, make sure to turn off "Automatically follow redirects" 
	under _Settings_ tab. Otherwise, you will be redirected to the auth link. 
	
	Copy the content from response header `Location` to your browser and give permission
	
1. Get list of transactions from all accounts

	```console
	curl 'https://localhost:5001/v1/transactions' --header 'Authorization: Basic am9objpkb2U='
	```
	
1. Get Transaction summary for last week

	```console
	curl 'https://localhost:5001/v1/transactions/_summary' --header 'Authorization: Basic am9objpkb2U='
	```
	
## Running unit tests

```console
dotnet test
```