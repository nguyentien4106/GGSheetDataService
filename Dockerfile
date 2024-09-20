FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview-windowsservercore-ltsc2022 AS base
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["DataService.Web/DataService.Web.csproj", "DataService.Web/"]
COPY ["DataService.Core/DataService.Core.csproj", "DataService.Web/"]
COPY ["DataService.Application/DataService.Application.csproj", "DataService.Web/"]
COPY ["DataService.Infrastructure/DataService.Infrastructure.csproj", "DataService.Web/"]

RUN dotnet restore "DataService.Web/DataService.Web.csproj"

COPY . .
WORKDIR "/src/DataService.Web"

RUN dotnet build "DataService.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataService.Web.csproj" -c Release -o /app/publish -r linux-x64 /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "DataService.Web.dll" ]