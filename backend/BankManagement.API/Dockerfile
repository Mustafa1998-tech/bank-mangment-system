# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore as distinct layers
COPY ["BankManagement.API.csproj", "./"]
RUN dotnet restore "BankManagement.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src"
RUN dotnet build "BankManagement.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BankManagement.API.csproj" -c Release -o /app/publish \
    --no-restore \
    --no-build \
    --nologo \
    --verbosity minimal \
    -p:UseAppHost=false \
    -p:DebugType=None \
    -p:DebugSymbols=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user and group
RUN groupadd -r appuser && \
    useradd -r -g appuser appuser && \
    chown -R appuser:appuser /app

# Copy published files
COPY --from=publish /app/publish .

# Set the user to run the application
USER appuser

# Expose the port the app runs on
EXPOSE 10000

# Set the entry point
ENTRYPOINT ["dotnet", "BankManagement.API.dll"]
