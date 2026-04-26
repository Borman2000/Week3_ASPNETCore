FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7119
EXPOSE 5275

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src/
COPY ["Common/JwtHelperService/JwtHelperService.csproj", "Common/JwtHelperService/"]
COPY ["Common/OpenTelemetryService/OpenTelemetryService.csproj", "Common/OpenTelemetryService/"]
COPY ["NotificationAPI/NotificationAPI.Domain/NotificationAPI.Domain.csproj", "NotificationAPI/NotificationAPI.Domain/"]
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["CSVParser/CSVParser.csproj", "CSVParser/"]
COPY ["Domain/Domain.csproj", "Domain/"]
RUN dotnet restore "WebAPI/WebAPI.csproj"
COPY . .

WORKDIR /src/WebAPI/
RUN dotnet build "./WebAPI.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebAPI.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]
