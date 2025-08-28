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
    useradd -r -g appuser -s /sbin/nologin appuser && \
    chown -R appuser:appuser /app

# Create necessary directories with proper permissions
RUN mkdir -p /app/logs /app/uploads && \
    chown -R appuser:appuser /app/logs /app/uploads && \
    chmod -R 750 /app/logs /app/uploads

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Install necessary system dependencies
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        tzdata \
        icu-libs \
        libgdiplus \
        libc6-dev \
        libx11-6 \
        libxcb1 \
        libxcomposite1 \
        libxcursor1 \
        libxdamage1 \
        libxi6 \
        libxtst6 \
        libnss3 \
        libcups2 \
        libxss1 \
        libxrandr2 \
        libasound2 && \
    rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Set file permissions
RUN chown -R appuser:appuser /app && \
    chmod -R 750 /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "BankManagement.API.dll"]
