# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["QueueWorker/UserService.API.csproj", "QueueWorker/"]
RUN dotnet restore "QueueWorker/QueueWorker.csproj"
COPY . .
WORKDIR "/src/QueueWorker"

RUN dotnet build "QueueWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "QueueWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QueueWorker.Build.dll"]
