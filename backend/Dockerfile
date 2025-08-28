FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BankManagement.API.csproj", "./"]
RUN dotnet restore "BankManagement.API.csproj"
COPY . .
RUN dotnet build "BankManagement.API.csproj" -c Release -o /app/build
RUN dotnet publish "BankManagement.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/logs
EXPOSE 80
ENTRYPOINT ["dotnet", "BankManagement.API.dll"]
