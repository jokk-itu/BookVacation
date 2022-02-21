# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 AS build
ARG Configuration
WORKDIR /app
COPY VacationService.Api VacationService.Api
COPY VacationService.Contracts VacationService.Contracts
COPY EventBusTransmitting EventBusTransmitting
COPY Swagger Swagger
RUN dotnet publish VacationService/VacationService.csproj -c $Configuration -o out


FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-amd64 AS release
ARG Environment
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=$Environment
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "VacationService.dll"]
