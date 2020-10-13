#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Channels/NATSS/Neuroglia.K8s.Eventing.Channels.Nats.Api/Neuroglia.K8s.Eventing.Channels.Nats.Api.csproj", "src/Channels/NATSS/Neuroglia.K8s.Eventing.Channels.Nats.Api/"]
COPY ["src/BuildingBlocks/Neuroglia.K8s.Eventing/Neuroglia.K8s.Eventing.csproj", "src/BuildingBlocks/Neuroglia.K8s.Eventing/"]
COPY ["src/Channels/NATSS/Neuroglia.K8s.Eventing.Channels.Nats.Application/Neuroglia.K8s.Eventing.Channels.Nats.Application.csproj", "src/Channels/NATSS/Neuroglia.K8s.Eventing.Channels.Nats.Application/"]
COPY ["src/Channels/NATSS/Neuroglia.K8s.Eventing.Channels.Nats.Infrastructure/Neuroglia.K8s.Eventing.Channels.Nats.Infrastructure.csproj", "src/Channels/NATSS/Neuroglia.K8s.Eventing.Channels.Nats.Infrastructure/"]
COPY ["src/Gateway/Neuroglia.K8s.Eventing.Gateway.Integration/Neuroglia.K8s.Eventing.Gateway.Integration.csproj", "src/Gateway/Neuroglia.K8s.Eventing.Gateway.Integration/"]
RUN dotnet restore "src/Channels/NATSS/Neuroglia.K8s.Eventing.Channels.Nats.Api/Neuroglia.K8s.Eventing.Channels.Nats.Api.csproj"
COPY . .
WORKDIR "/src/src/Channels/NATSS/Neuroglia.K8s.Eventing.Channels.Nats.Api"
RUN dotnet build "Neuroglia.K8s.Eventing.Channels.Nats.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Neuroglia.K8s.Eventing.Channels.Nats.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neuroglia.K8s.Eventing.Channels.Nats.Api.dll"]