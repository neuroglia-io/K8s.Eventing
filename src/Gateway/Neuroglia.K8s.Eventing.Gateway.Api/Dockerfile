#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Gateway/Neuroglia.K8s.Eventing.Gateway.Api/Neuroglia.K8s.Eventing.Gateway.Api.csproj", "src/Gateway/Neuroglia.K8s.Eventing.Gateway.Api/"]
COPY ["src/Gateway/Neuroglia.K8s.Eventing.Gateway.Application/Neuroglia.K8s.Eventing.Gateway.Application.csproj", "src/Gateway/Neuroglia.K8s.Eventing.Gateway.Application/"]
COPY ["src/Gateway/Neuroglia.K8s.Eventing.Gateway.Infrastructure/Neuroglia.K8s.Eventing.Gateway.Infrastructure.csproj", "src/Gateway/Neuroglia.K8s.Eventing.Gateway.Infrastructure/"]
COPY ["src/Gateway/Neuroglia.K8s.Eventing.Gateway.Integration/Neuroglia.K8s.Eventing.Gateway.Integration.csproj", "src/Gateway/Neuroglia.K8s.Eventing.Gateway.Integration/"]
COPY ["src/BuildingBlocks/Neuroglia.K8s.Eventing/Neuroglia.K8s.Eventing.csproj", "src/BuildingBlocks/Neuroglia.K8s.Eventing/"]
RUN dotnet restore "src/Gateway/Neuroglia.K8s.Eventing.Gateway.Api/Neuroglia.K8s.Eventing.Gateway.Api.csproj"
COPY . .
WORKDIR "/src/src/Gateway/Neuroglia.K8s.Eventing.Gateway.Api"
RUN dotnet build "Neuroglia.K8s.Eventing.Gateway.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Neuroglia.K8s.Eventing.Gateway.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neuroglia.K8s.Eventing.Gateway.Api.dll"]