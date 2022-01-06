# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 AS build
ARG Configuration
WORKDIR /app
COPY TrackingService TrackingService
COPY Contracts Contracts
COPY EventBusTransmitting EventBusTransmitting
RUN dotnet publish TrackingService/TrackingService.csproj -c $Configuration -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-amd64 AS release
ARG Environment
EXPOSE 80
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=$Environment
ENV DOTNET_ENVIRONMENT=$Environment
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TrackingService.dll"]