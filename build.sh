#!/bin/bash

echo "=== Starting Build Process ==="

# Navigate to the backend directory
echo "Building .NET application..."
cd backend/BankManagement.API

# Restore dependencies
dotnet restore

# Build the application
dotnet build -c Release --no-restore

# Publish the application
echo "Publishing application..."
dotnet publish -c Release -o ../../publish --no-restore --no-build

# Navigate back to the root directory
cd ../..

echo "=== Build Complete ==="

# Run the application
echo "Starting application..."
cd /app
dotnet BankManagement.API.dll --urls "http://*:10000"
