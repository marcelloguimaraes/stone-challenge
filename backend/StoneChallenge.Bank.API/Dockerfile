FROM microsoft/dotnet:2.2-aspnetcore-runtime-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-alpine AS build
WORKDIR /src
COPY ["StoneChallenge.Bank.API/StoneChallenge.Bank.API.csproj", "StoneChallenge.Bank.API/"]
COPY ["StoneChallenge.Bank.Application/StoneChallenge.Bank.Application.csproj", "StoneChallenge.Bank.Application/"]
COPY ["StoneChallenge.Bank.Domain/StoneChallenge.Bank.Domain.csproj", "StoneChallenge.Bank.Domain/"]
COPY ["StoneChallenge.Bank.Infra.Data/StoneChallenge.Bank.Infra.Data.csproj", "StoneChallenge.Bank.Infra.Data/"]
RUN dotnet restore "StoneChallenge.Bank.API/StoneChallenge.Bank.API.csproj"
COPY . .
WORKDIR "/src/StoneChallenge.Bank.API"
RUN dotnet build "StoneChallenge.Bank.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "StoneChallenge.Bank.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet StoneChallenge.Bank.API.dll