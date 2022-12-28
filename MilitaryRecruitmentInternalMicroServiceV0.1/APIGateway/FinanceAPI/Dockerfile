#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["APIGateway/FinanceAPI/FinanceAPI.csproj", "APIGateway/FinanceAPI/"]
RUN dotnet restore "APIGateway/FinanceAPI/FinanceAPI.csproj"
COPY . .
WORKDIR "/src/APIGateway/FinanceAPI"
RUN dotnet build "FinanceAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinanceAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinanceAPI.dll"]
