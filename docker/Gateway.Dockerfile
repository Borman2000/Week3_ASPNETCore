FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7319
EXPOSE 5278

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src/Common/
COPY ["Common/OpenTelemetryService/OpenTelemetryService.csproj", "OpenTelemetryService/"]
COPY ["Common/JwtHelperService/JwtHelperService.csproj", "JwtHelperService/"]

WORKDIR /src/Yarp.Gateway/
COPY "Yarp.Gateway/Yarp.Gateway.csproj" .
RUN dotnet restore "Yarp.Gateway.csproj"

COPY "Yarp.Gateway/" .

WORKDIR /src/Common/
COPY ["Common/OpenTelemetryService/", "OpenTelemetryService/"]
COPY ["Common/JwtHelperService/", "JwtHelperService/"]

WORKDIR /src/Yarp.Gateway/
RUN dotnet build "./Yarp.Gateway.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Yarp.Gateway.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yarp.Gateway.dll"]
