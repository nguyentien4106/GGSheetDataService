FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build

WORKDIR /src
COPY ["DataService.Web/DataService.Web.csproj", "."]
COPY ["DataService.Core/DataService.Core.csproj", "."]
COPY ["DataService.Application/DataService.Application.csproj", "."]
COPY ["DataService.Infrastructure/DataService.Infrastructure.csproj", "."]
COPY . .
# WORKDIR "/src/."

RUN msbuild /t:build DataService.Web.csproj
RUN msbuild /p:Configuration=Release /p:Platform="Any CPU" DataService.Web.csproj

FROM build AS publish
RUN dotnet publish "DataService.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "DataService.Web.dll" ]