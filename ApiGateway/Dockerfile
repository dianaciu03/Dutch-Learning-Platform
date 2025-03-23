FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8086
EXPOSE 8087
 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApiGateway.csproj", "./"]
RUN dotnet restore "ApiGateway.csproj"
COPY . .
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build
 
FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish
 
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Properties/ocelot.json Properties/ocelot.json
RUN ls -R /app
ENTRYPOINT ["dotnet", "ApiGateway.dll"]