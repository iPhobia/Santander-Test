# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and projects
COPY *.sln ./
COPY ["Api/Api.csproj", "Api/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Api/Api.csproj"

# Copy the remaining source code
COPY . .

# Build and publish the app
WORKDIR /src/Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Environment and ports
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
EXPOSE 8081

# Copy published app
COPY --from=build /app/publish .

# Run the app
ENTRYPOINT ["dotnet", "Api.dll"]