#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LoginAPI/LoginAPI.csproj", "LoginAPI/"]
RUN dotnet restore "LoginAPI/LoginAPI.csproj"
COPY . .
WORKDIR "/src/LoginAPI"
RUN dotnet build "LoginAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoginAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoginAPI.dll"]
