# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy API folder (keep structure)
COPY API/ ./API/

# Restore solution
WORKDIR /src/API
RUN dotnet restore TIMEAPI.sln

# Publish API project
WORKDIR /src/API/TIMEAPI
RUN dotnet publish TIMEAPI.csproj -c Release -o /app/publish

# ---------- RUNTIME STAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000

COPY --from=build /app/publish .

EXPOSE 5000
ENTRYPOINT ["dotnet", "TIMEAPI.dll"]
