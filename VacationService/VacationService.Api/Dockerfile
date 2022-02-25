# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 AS build
ARG Configuration
ARG GithubToken
ENV GithubToken=$GithubToken
WORKDIR /app
COPY VacationService.Api VacationService.Api
COPY VacationService.Contracts VacationService.Contracts
COPY EventDispatcher EventDispatcher
RUN dotnet publish VacationService.Api/VacationService.Api.csproj -c $Configuration -o out


FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-amd64 AS release
ARG Environment
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=$Environment
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "VacationService.Api.dll"]