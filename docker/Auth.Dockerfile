FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ARG APP_UID=10001
RUN adduser \
	--disabled-password \
	--gecos "" \
	--home "/nonexistent" \
	--shell "/usr/sbin/nologin" \
	--uid "${APP_UID}" \
	appuser
USER appuser
WORKDIR /app
EXPOSE 7219
EXPOSE 5276

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src/Common/
COPY ["Common/OpenTelemetryService/OpenTelemetryService.csproj", "OpenTelemetryService/"]
COPY ["Common/JwtHelperService/JwtHelperService.csproj", "JwtHelperService/"]

WORKDIR /src/AuthAPI/
RUN echo "BUILD_CONFIGURATION = $BUILD_CONFIGURATION"
COPY ["AuthAPI/AuthAPI.Api/AuthAPI.Api.csproj", "AuthAPI.Api/"]
COPY ["AuthAPI/AuthAPI.Application/AuthAPI.Application.csproj", "AuthAPI.Application/"]
COPY ["AuthAPI/AuthAPI.Domain/AuthAPI.Domain.csproj", "AuthAPI.Domain/"]
COPY ["AuthAPI/AuthAPI.Infrastructure/AuthAPI.Infrastructure.csproj", "AuthAPI.Infrastructure/"]
RUN dotnet restore "AuthAPI.Api/AuthAPI.Api.csproj"

COPY "AuthAPI/" .

WORKDIR /src/Common/
COPY ["Common/OpenTelemetryService/", "OpenTelemetryService/"]
COPY ["Common/JwtHelperService/", "JwtHelperService/"]

WORKDIR /src/AuthAPI/AuthAPI.Api/
RUN dotnet build "./AuthAPI.Api.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthAPI.Api.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthAPI.Api.dll"]
