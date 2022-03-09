# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 AS build
ARG Configuration
ARG GithubToken
ENV GithubToken=$GithubToken
WORKDIR /app
COPY NuGet.config NuGet.config
COPY TrackingService.Api TrackingService.Api
COPY TrackingService.Domain TrackingService.Domain
COPY TrackingService.Contracts TrackingService.Contracts
COPY TrackingService.Infrastructure TrackingService.Infrastructure
RUN dotnet publish TrackingService.Api/TrackingService.Api.csproj -c $Configuration -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-amd64 AS release
ARG Environment
EXPOSE 80
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=$Environment
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TrackingService.Api.dll"]
