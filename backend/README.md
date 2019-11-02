# stone-challenge-bank-api

Diretório que contém a API REST em .NET Core(2.2) solicitada para atender as necessidades do desafio.

## O que esta api é?
Simula um sistema de banco onde fornece as operações básicas de saque, depósito, transferência e extrato(estes possuem taxas diferenciadas e devem ser refletidas no extrato). Também possui uma camada de cadastro e login de usuário para deixar o mais próximo de um cenário real possível.

## Tecnologias e bibliotecas usadas
- .Net Core 2.2
- AutoMapper
- AspNetCore Identity
- Entity Framework Core
- SQLite
- Moq
- XUnit(TDD)
- Testes de unidade
- Arquitetura em camadas, cada qual com sua responsabilidade
- Injeção de dependência nativa
- Repository Pattern
- Docker
- Visual Studio 2019
- Swagger UI

## Como rodar o projeto?

Clone este repositório com o comando `git clone https://github.com/marcelloguimaraes/stone-challenge.git`

Abra a solution pelo Visual Studio e execute o build com F5, ou execute os comandos abaixo caso queira executar por linha de comando:

`$ cd stone-challenge/backend`

`$ dotnet build StoneChallenge.Bank.API.sln`

`$ dotnet run StoneChallenge.Bank.API/StoneChallenge.Bank.API.csproj`

Nesse momento a API estará rodando nos seguintes endereços: https://localhost:5001 ou http://localhost:5000

## Endpoints

Você pode acessar a url https://localhost:5001/swagger para ver os endpoints de forma visual pelo Swagger UI.

A aplicação possui 3 entidades principais: 

- Conta: É onde as transações são efetuadas, possui saldo, número e agencia. é relacionada com cliente(`customerId`) e com o cadastro de usuário(`userId`)
- Cliente: É relacionado à uma conta, possui nome, cpf e data de nascimento
- Transação: Onde residem todos os tipos de transações realizadas na conta, possui o tipo da transação, data e valor.

## Como usar os endpoints?

Os endpoints necessitam de autenticação para serem usados, para isso envie um POST para o endpoint `api/auth/open-account` com o modelo definido abaixo para criar o acesso.

POST `/api/auth/open-account`
```
{
  "email": "teste@hotmail.com",
  "password": "Teste@123",
  "agency": 3032,
  "customer": {
    "cpf": "46555447964",
    "name": "Marcello Guimarães",
    "birthDate": "1995-03-16"
  }
}
```

Após criado, deve ser enviado um POST para o endpoint `api/auth/login` com e-mail e senha cadastrados previamente para conseguir o token como mostra o modelo abaixo.

POST `/api/auth/login`
```
{
  "email": "joao@hotmail.com",
  "password": "Joao@123"
}
```
Retorno obtido:
```
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE1NzI2NjM1NjgsImV4cCI6MTU3MjY2NDE2OCwiaWF0IjoxNTcyNjYzNTY4LCJpc3MiOiJTdG9uZUNoYWxsZW5nZS1CYW5rLUFQSSIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0In0.rA1T0WxW5MkwVe42ysLrZq7LwCYtNL6ISqe208Cs4Cw",
  "user": {
    "userName": "joao@hotmail.com",
    "email": "joao@hotmail.com",
    "id": "75884412-b5d2-4619-b123-e213cbe10db0"
  }
}
```
O Token deve ser utilizado no headers `Authorization` da requisição HTTP, um exemplo de valor: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE1NzI2NjM1NjgsImV4cCI6MTU3MjY2NDE2OCwiaWF0IjoxNTcyNjYzNTY4LCJpc3MiOiJTdG9uZUNoYWxsZW5nZS1CYW5rLUFQSSIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0In0.rA1T0WxW5MkwVe42ysLrZq7LwCYtNL6ISqe208Cs4Cw`

## accounts - Rota que contém os endpoints para saque, depósito, etc.
POST `/api/accounts/transactions` - Recupera o extrato de uma conta dado uma agência e número da conta.
```
{
  "accountNumber": 654421,
  "agency": 3012
}
```
POST `/api/accounts/transfer` - Realiza uma transferência entre duas contas, é necessário informar a conta origem e destino, junto com um valor. Possui taxa de R$ 1,00 sobre o valor transferido(para a conta origem)
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
POST `/api/accounts/withdraw` - Realiza um saque dado uma conta, agência e valor. Possui taxa de R$ 4,00 sobre o valor sacado).
```
{
  "value": 50.69,
  "accountNumber": 657899,
  "agency": 6544
}
```
POST `/api/accounts/deposit` - Realiza um depósito dado uma conta, agência e valor. Possui taxa de 1% sobre o valor depositado.
```
{
  "value": 50.69,
  "accountNumber": 657899,
  "agency": 6544
}
```
GET `/api/accounts/users/{userId}` - Retorna um modelo de conta dado um `userId`, usado no dashboard frontend para exibir os dados da conta logada.
```
{
  "accountNumber": 657899,
  "agency": 6544,
  "balance": 56.8,
  "customerName": "Marcello Guimarães"
}
```
## customers

GET `/api/customers/transactions/{cpf}` - Retorna uma lista de transações(extrato) de um cliente pelo seu cpf.
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
