#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Sources/Services/NatsChannel/Neuroglia.K8s.Eventing.Channels.Nats.Api/Neuroglia.K8s.Eventing.Channels.Nats.Api.csproj", "Sources/Services/NatsChannel/Neuroglia.K8s.Eventing.Channels.Nats.Api/"]
RUN dotnet restore "Sources/Services/NatsChannel/Neuroglia.K8s.Eventing.Channels.Nats.Api/Neuroglia.K8s.Eventing.Channels.Nats.Api.csproj"
COPY . .
WORKDIR "/src/Sources/Services/NatsChannel/Neuroglia.K8s.Eventing.Channels.Nats.Api"
RUN dotnet build "Neuroglia.K8s.Eventing.Channels.Nats.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Neuroglia.K8s.Eventing.Channels.Nats.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neuroglia.K8s.Eventing.Channels.Nats.Api.dll"]