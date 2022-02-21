# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 AS build
ARG Configuration
WORKDIR /app
COPY FlightService.Api FlightService.Api
COPY FlightService.Domain FlightService.Domain
COPY FlightService.Contracts FlightService.Contracts
COPY FlightService.Infrastructure FlightService.Infrastructure
COPY EventBusTransmitting EventBusTransmitting
COPY Swagger Swagger
RUN dotnet publish FlightService/FlightService.csproj -c $Configuration -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-amd64 AS release
ARG Environment
EXPOSE 80
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=$Environment
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "FlightService.dll"]