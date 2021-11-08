# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0.301-buster-slim-amd64 AS build
WORKDIR /app
COPY BookFlightService BookFlightService
COPY Contracts Contracts
RUN dotnet restore BookFlightService/BookFlightService.csproj
RUN dotnet publish BookFlightService/BookFlightService.csproj -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/runtime:5.0.8-buster-slim-amd64 AS release
WORKDIR /app
ENV DOTNET_ENVIRONMENT=Production
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "BookFlightService.dll"]