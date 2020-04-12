#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["OpenNetworkStatus.csproj", "OpenNetworkStatus/"]
RUN dotnet restore "OpenNetworkStatus/OpenNetworkStatus.csproj"
COPY . "OpenNetworkStatus/"
WORKDIR "/src/OpenNetworkStatus"
RUN dotnet build "OpenNetworkStatus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenNetworkStatus.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

CMD ["dotnet", "OpenNetworkStatus.dll"]
