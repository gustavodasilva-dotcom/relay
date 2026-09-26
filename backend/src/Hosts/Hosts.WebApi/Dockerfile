FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

COPY . .
RUN dotnet restore "src/Hosts/Hosts.WebApi/Hosts.WebApi.csproj"
RUN dotnet publish "src/Hosts/Hosts.WebApi/Hosts.WebApi.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Hosts.WebApi.dll"]
