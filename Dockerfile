# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/MoFs.Prototype/MoFs.Prototype.csproj src/MoFs.Prototype/
RUN dotnet restore src/MoFs.Prototype/MoFs.Prototype.csproj

COPY . .
WORKDIR /src/src/MoFs.Prototype
RUN dotnet publish MoFs.Prototype.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "MoFs.Prototype.dll"]
