# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build and publish in Release mode
RUN dotnet publish BulkyBookWeb/BulkyBookWeb.csproj -c Release -o /publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published files
COPY --from=build /publish .

# Expose port
EXPOSE 8080

# Start the app
ENTRYPOINT ["dotnet", "BulkyBookWeb.dll"]
