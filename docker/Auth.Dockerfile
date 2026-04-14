FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7219
EXPOSE 5276

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src/AuthAPI/
RUN echo "BUILD_CONFIGURATION = $BUILD_CONFIGURATION"
COPY ["AuthAPI/AuthAPI.Api/AuthAPI.Api.csproj", "AuthAPI.Api/"]
COPY ["AuthAPI/AuthAPI.Application/AuthAPI.Application.csproj", "AuthAPI.Application/"]
COPY ["AuthAPI/AuthAPI.Domain/AuthAPI.Domain.csproj", "AuthAPI.Domain/"]
COPY ["AuthAPI/AuthAPI.Infrastructure/AuthAPI.Infrastructure.csproj", "AuthAPI.Infrastructure/"]
RUN dotnet restore "AuthAPI.Api/AuthAPI.Api.csproj"

COPY "AuthAPI/" .

WORKDIR /src/Config/
COPY "Config/" .

WORKDIR /src/AuthAPI/AuthAPI.Api/
RUN dotnet build "./AuthAPI.Api.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthAPI.Api.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthAPI.Api.dll"]
