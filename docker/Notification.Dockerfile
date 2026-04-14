FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7019
EXPOSE 5277

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src/AuthAPI/
COPY ["AuthAPI/AuthAPI.Domain/AuthAPI.Domain.csproj", "AuthAPI.Domain/"]
WORKDIR /src/NotificationAPI/
COPY ["NotificationAPI/NotificationAPI.Api/NotificationAPI.Api.csproj", "NotificationAPI.Api/"]
COPY ["NotificationAPI/NotificationAPI.Application/NotificationAPI.Application.csproj", "NotificationAPI.Application/"]
COPY ["NotificationAPI/NotificationAPI.Domain/NotificationAPI.Domain.csproj", "NotificationAPI.Domain/"]
COPY ["NotificationAPI/NotificationAPI.Infrastructure/NotificationAPI.Infrastructure.csproj", "NotificationAPI.Infrastructure/"]
RUN dotnet restore "NotificationAPI.Api/NotificationAPI.Api.csproj"

COPY "NotificationAPI/" .
WORKDIR /src/AuthAPI/AuthAPI.Domain/
COPY "AuthAPI/AuthAPI.Domain/" .

WORKDIR /src/Config/
COPY "Config/" .

WORKDIR /src/NotificationAPI/NotificationAPI.Api/
RUN dotnet build "./NotificationAPI.Api.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NotificationAPI.Api.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationAPI.Api.dll"]
