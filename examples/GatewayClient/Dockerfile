#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["examples/GatewayClient/GatewayClient.csproj", "examples/GatewayClient/"]
COPY ["src/Gateway/Neuroglia.K8s.Eventing.Gateway.Integration/Neuroglia.K8s.Eventing.Gateway.Integration.csproj", "src/Gateway/Neuroglia.K8s.Eventing.Gateway.Integration/"]
RUN dotnet restore "examples/GatewayClient/GatewayClient.csproj"
COPY . .
WORKDIR "/src/examples/GatewayClient"
RUN dotnet build "GatewayClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GatewayClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GatewayClient.dll"]