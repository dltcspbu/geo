FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["DltcGeoServer/DltcGeoServer.csproj", "DltcGeoServer/"]
RUN dotnet restore "DltcGeoServer/DltcGeoServer.csproj"
COPY . .
WORKDIR "/src/DltcGeoServer"
RUN dotnet build "DltcGeoServer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DltcGeoServer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY ["DltcGeoServer/LO.pbf", "/app/"]
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DltcGeoServer.dll"]