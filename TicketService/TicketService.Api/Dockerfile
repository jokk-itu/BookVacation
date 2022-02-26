# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 AS build
ARG Configuration
ARG GithubToken
ENV GithubToken=$GithubToken
WORKDIR /app
COPY NuGet.config NuGet.config
COPY TicketService.Api TicketService.Api
COPY TicketService.Domain TicketService.Domain
COPY TicketService.Contracts TicketService.Contracts
COPY TicketService.Infrastructure TicketService.Infrastructure
RUN dotnet publish TicketService.Api/TicketService.Api.csproj -c $Configuration -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-amd64 AS release
ARG Environment
EXPOSE 80
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=$Environment
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TicketService.Api.dll"]