FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["DataService.csproj", "DataService/"]
COPY ["../DataService.Core/DataService.Core.csproj", "DataService/"]
COPY ["../DataService.Infrastructure/DataService.Infrastructure.csproj", "DataService/"]

RUN dotnet restore "DataService/DataService.csproj"

COPY . .
WORKDIR "/src/DataService"

RUN dotnet build "DataService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "DataService.dll" ]