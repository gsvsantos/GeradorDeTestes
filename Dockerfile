# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GeradorDeTestes.WebApp/GeradorDeTestes.WebApp.csproj", "GeradorDeTestes.WebApp/"]
COPY ["GeradorDeTestes.Aplicacao/GeradorDeTestes.Aplicacao.csproj", "GeradorDeTestes.Aplicacao/"]
COPY ["GeradorDeTestes.Dominio/GeradorDeTestes.Dominio.csproj", "GeradorDeTestes.Dominio/"]
COPY ["GeradorDeTestes.Infraestrutura.IA.Gemini/GeradorDeTestes.Infraestrutura.IA.Gemini.csproj", "GeradorDeTestes.Infraestrutura.IA.Gemini/"]
COPY ["GeradorDeTestes.Infraestrutura.ORM/GeradorDeTestes.Infraestrutura.ORM.csproj", "GeradorDeTestes.Infraestrutura.ORM/"]
RUN dotnet restore "./GeradorDeTestes.WebApp/GeradorDeTestes.WebApp.csproj"
COPY . .
WORKDIR "/src/GeradorDeTestes.WebApp"
RUN dotnet build "./GeradorDeTestes.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GeradorDeTestes.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeradorDeTestes.WebApp.dll"]