#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Channels/EventStore/Neuroglia.K8s.Eventing.Channels.EventStore.Api/Neuroglia.K8s.Eventing.Channels.EventStore.Api.csproj", "src/Channels/EventStore/Neuroglia.K8s.Eventing.Channels.EventStore.Api/"]
COPY ["src/Channels/EventStore/Neuroglia.K8s.Eventing.Channels.EventStore.Application/Neuroglia.K8s.Eventing.Channels.EventStore.Application.csproj", "src/Channels/EventStore/Neuroglia.K8s.Eventing.Channels.EventStore.Application/"]
COPY ["src/Channels/EventStore/Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure/Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure.csproj", "src/Channels/EventStore/Neuroglia.K8s.Eventing.Channels.EventStore.Infrastructure/"]
COPY ["src/Gateway/Neuroglia.K8s.Eventing.Gateway.Integration/Neuroglia.K8s.Eventing.Gateway.Integration.csproj", "src/Gateway/Neuroglia.K8s.Eventing.Gateway.Integration/"]
COPY ["src/BuildingBlocks/Neuroglia.Mediation/Neuroglia.Mediation.csproj", "src/BuildingBlocks/Neuroglia.Mediation/"]
COPY ["src/BuildingBlocks/Neuroglia.K8s.Eventing/Neuroglia.K8s.Eventing.csproj", "src/BuildingBlocks/Neuroglia.K8s.Eventing/"]
COPY ["src/BuildingBlocks/Neuroglia.Mediation.AspNetCore/Neuroglia.Mediation.AspNetCore.csproj", "src/BuildingBlocks/Neuroglia.Mediation.AspNetCore/"]
RUN dotnet restore "src/Channels/EventStore/Neuroglia.K8s.Eventing.Channels.EventStore.Api/Neuroglia.K8s.Eventing.Channels.EventStore.Api.csproj"
COPY . .
WORKDIR "/src/src/Channels/EventStore/Neuroglia.K8s.Eventing.Channels.EventStore.Api"
RUN dotnet build "Neuroglia.K8s.Eventing.Channels.EventStore.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Neuroglia.K8s.Eventing.Channels.EventStore.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neuroglia.K8s.Eventing.Channels.EventStore.Api.dll"]