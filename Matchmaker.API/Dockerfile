# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 6071
EXPOSE 6012

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["Matchmaker.API/Matchmaker.API.csproj", "Matchmaker.API/"]
COPY ["Matchmaker.API/serilog.json", "Matchmaker.API/"]
RUN dotnet restore "Matchmaker.API/Matchmaker.API.csproj"
COPY . .
WORKDIR "/src/Matchmaker.API"

RUN dotnet build "Matchmaker.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Matchmaker.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Matchmaker.API.Build.dll"]
