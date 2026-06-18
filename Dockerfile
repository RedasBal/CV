# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first (cached layer) then build & publish.
COPY CV.csproj ./
RUN dotnet restore CV.csproj

COPY . ./
RUN dotnet publish CV.csproj -c Release -o /app /p:UseAppHost=false

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Render provides $PORT at runtime; Program.cs binds to it. 8080 is the local default.
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "CV.dll"]
