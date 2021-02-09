#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Channels/Kafka/Controller/Neuroglia.K8s.Eventing.Kafka.Controller/Neuroglia.K8s.Eventing.Kafka.Controller.csproj", "src/Channels/Kafka/Controller/Neuroglia.K8s.Eventing.Kafka.Controller/"]
COPY ["src/BuildingBlocks/Neuroglia.K8s.Eventing/Neuroglia.K8s.Eventing.csproj", "src/BuildingBlocks/Neuroglia.K8s.Eventing/"]
RUN dotnet restore "src/Channels/Kafka/Controller/Neuroglia.K8s.Eventing.Kafka.Controller/Neuroglia.K8s.Eventing.Kafka.Controller.csproj"
COPY . .
WORKDIR "/src/src/Channels/Kafka/Controller/Neuroglia.K8s.Eventing.Kafka.Controller"
RUN dotnet build "Neuroglia.K8s.Eventing.Kafka.Controller.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Neuroglia.K8s.Eventing.Kafka.Controller.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neuroglia.K8s.Eventing.Kafka.Controller.dll"]