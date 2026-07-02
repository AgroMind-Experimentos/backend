FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY API/EcotrackPlatform.API.csproj API/
RUN dotnet restore API/EcotrackPlatform.API.csproj

COPY API/ API/
RUN dotnet publish API/EcotrackPlatform.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["sh", "-c", "ASPNETCORE_HTTP_PORTS=$PORT exec dotnet EcotrackPlatform.API.dll"]
