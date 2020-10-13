#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Sources/Services/Gateway/Neuroglia.K8s.Eventing.Gateway.Api/Neuroglia.K8s.Eventing.Gateway.Api.csproj", "Sources/Services/Gateway/Neuroglia.K8s.Eventing.Gateway.Api/"]
COPY ["Sources/BuildingBlocks/Neuroglia.K8s/Neuroglia.K8s.csproj", "Sources/BuildingBlocks/Neuroglia.K8s/"]
COPY ["Sources/Services/Gateway/Neuroglia.K8s.Eventing.Gateway.Infrastructure/Neuroglia.K8s.Eventing.Gateway.Infrastructure.csproj", "Sources/Services/Gateway/Neuroglia.K8s.Eventing.Gateway.Infrastructure/"]
RUN dotnet restore "Sources/Services/Gateway/Neuroglia.K8s.Eventing.Gateway.Api/Neuroglia.K8s.Eventing.Gateway.Api.csproj"
COPY . .
WORKDIR "/src/Sources/Services/Gateway/Neuroglia.K8s.Eventing.Gateway.Api"
RUN dotnet build "Neuroglia.K8s.Eventing.Gateway.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Neuroglia.K8s.Eventing.Gateway.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neuroglia.K8s.Eventing.Gateway.Api.dll"]