FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ["MagicalKitties.Api/MagicalKitties.Api.csproj", "MagicalKitties.Api/"]
COPY ["MagicalKitties.Application/MagicalKitties.Application.csproj", "MagicalKitties.Application/"]
COPY ["MagicalKitties.Contracts/MagicalKitties.Contracts.csproj", "MagicalKitties.Contracts/"]
RUN dotnet restore "MagicalKitties.Api/MagicalKitties.Api.csproj"
COPY . .
WORKDIR "/src/MagicalKitties.Api"
RUN dotnet build "./MagicalKitties.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Development
RUN dotnet publish "./MagicalKitties.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicalKitties.Api.dll"]
