FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/core/accrual/EntryManager.Core.Accrual.Api/EntryManager.Core.Accrual.Api.csproj", "core/accrual/EntryManager.Core.Accrual.Api/"]
RUN dotnet restore "core/accrual/EntryManager.Core.Accrual.Api/EntryManager.Core.Accrual.Api.csproj"
COPY . .
WORKDIR "/src/src/core/accrual/EntryManager.Core.Accrual.Api"
RUN dotnet build "./EntryManager.Core.Accrual.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EntryManager.Core.Accrual.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "entrymanager-core-accrual-api.dll"]
