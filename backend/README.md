# stone-challenge-bank-api

WebApi REST in .NET Core 2.2 created for the stone challenge.

## Technologies/libs used
- .Net Core 2.2
- AutoMapper
- AspNetCore Identity
- Entity Framework Core
- SQLite
- Moq
- XUnit
- Visual Studio 2019
- Swagger UI

## Follow steps to run the project
`$ cd stone-challenge/backend`

`$ dotnet build StoneChallenge.Bank.API.sln`

`$ dotnet run StoneChallenge.Bank.API/StoneChallenge.Bank.API.csproj`

You will be able to see the project running at https://localhost:5001 or http://localhost:5000

## Endpoints

You can go to https://localhost:5001/swagger to see the endpoints and use it as you wish.

Ps: To access the endpoints you have to get a Bearer access token provided by `api/auth/login` endpoint passing e-mail and password, for that you have to open an account on `api/auth/open-account` endpoint.

## Auth - Authentications endpoints
POST `/api/auth/open-account` - Open a bank account and create a customer given a json.
```
{
  "email": "marcello@hotmail.com",
  "password": "Marcello@123",
  "agency": 3032,
  "customer": {
    "cpf": "46555447964",
    "name": "Marcello Guimarães",
    "birthDate": "1995-03-16"
  }
}
```
POST `/api/auth/login` - Perform authentication and get an access token.
```
{
  "email": "marcello@hotmail.com",
  "password": "Teste@123"
}
```
## Account - Endpoints for withdraw, deposit, etc.
POST `/api/accounts/transactions` - Get all the transactions given an agency and account number.
```
{
  "accountNumber": 654421,
  "agency": 3012
}
```
POST `/api/accounts/transfer` - Perform a transfer between two accounts, you have to provide both and a value.
```
{
  "sourceAccount": {
    "accountNumber": 657899,
    "agency": 6544
  },
  "targetAccount": {
    "accountNumber": 988658,
    "agency": 5446
  },
  "value": 10.69
}
```
POST `/api/accounts/withdraw` - Perform a withdraw given an account and a value.
```
{
  "value": 50.69,
  "accountNumber": 657899,
  "agency": 6544
}
```
POST `/api/accounts/deposit` - Perform a deposit given an account and a value.
```
{
  "value": 50.69,
  "accountNumber": 657899,
  "agency": 6544
}
```
GET `/api/accounts/users/{userId}` - Returns an Account model given a user id.
```
{
  "accountNumber": 657899,
  "agency": 6544,
  "balance": 56.8,
  "customerName": "Marcello Guimarães"
}
```
## Customer

GET `/api/customers/transactions/{cpf}` - Returns a list of transactions of customer given a cpf.
```
[
  {
    "transactionId": "8f00d290-c07a-481c-9697-3cfa076fb637",
    "transactionType": "Depósito",
    "date": "2019-11-01T19:27:27.5273561",
    "dateFormatted": "01/11/2019 19:27:27",
    "value": 49.5
  },
  
  {
    "transactionId": "ao8a7dd-c07a-8aa8-9697-3cfa07kia637",
    "transactionType": "Saque",
    "date": "2019-11-05T19:30:27.5273561",
    "dateFormatted": "05/11/2019 19:30:27",
    "value": 49.5
  }
]
```
