# Use the official .NET 8 SDK image from Microsoft to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore any dependencies (via NuGet)
COPY ["SimpleApi.csproj", "./"]
RUN dotnet restore

RUN apt-get update && \
    apt-get install -y curl && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Copy the project files and build the release
COPY . ./
RUN dotnet publish -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# Set environment variables
ENV AE_VERSION=1.0.1 \
    APP_ID=simpleapi \
    APP_PORT=8080 \
    APP_SECRET=12345 \
    APP_VERSION=1.0.1 \
    NEXTCLOUD_URL=http://nextcloud.local 

WORKDIR /app
COPY --from=build-env /app/out ./
COPY img/ /app/wwwroot/img/
ENTRYPOINT ["dotnet", "SimpleApi.dll"]
