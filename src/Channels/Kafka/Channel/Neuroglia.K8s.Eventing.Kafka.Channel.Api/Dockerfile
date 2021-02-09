#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Channels/Kafka/Channel/Neuroglia.K8s.Eventing.Kafka.Channel.Api/Neuroglia.K8s.Eventing.Kafka.Channel.Api.csproj", "src/Channels/Kafka/Channel/Neuroglia.K8s.Eventing.Kafka.Channel.Api/"]
COPY ["src/Channels/Kafka/Channel/Neuroglia.K8s.Eventing.Kafka.Channel.Application/Neuroglia.K8s.Eventing.Kafka.Channel.Application.csproj", "src/Channels/Kafka/Channel/Neuroglia.K8s.Eventing.Kafka.Channel.Application/"]
COPY ["src/BuildingBlocks/Neuroglia.K8s.Eventing.Channels/Neuroglia.K8s.Eventing.Channels.csproj", "src/BuildingBlocks/Neuroglia.K8s.Eventing.Channels/"]
COPY ["src/BuildingBlocks/Neuroglia.K8s.Eventing/Neuroglia.K8s.Eventing.csproj", "src/BuildingBlocks/Neuroglia.K8s.Eventing/"]
COPY ["src/BuildingBlocks/Neuroglia.Mediation/Neuroglia.Mediation.csproj", "src/BuildingBlocks/Neuroglia.Mediation/"]
RUN dotnet restore "src/Channels/Kafka/Channel/Neuroglia.K8s.Eventing.Kafka.Channel.Api/Neuroglia.K8s.Eventing.Kafka.Channel.Api.csproj"
COPY . .
WORKDIR "/src/src/Channels/Kafka/Channel/Neuroglia.K8s.Eventing.Kafka.Channel.Api"
RUN dotnet build "Neuroglia.K8s.Eventing.Kafka.Channel.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Neuroglia.K8s.Eventing.Kafka.Channel.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neuroglia.K8s.Eventing.Kafka.Channel.Api.dll"]