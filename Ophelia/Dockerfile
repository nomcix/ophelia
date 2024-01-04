# Build Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Ophelia.csproj", "./"]
RUN dotnet restore "Ophelia.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Ophelia.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "Ophelia.csproj" -c Release -o /app/publish

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ophelia.dll"]
