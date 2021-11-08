# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0.301-buster-slim-amd64 AS build
WORKDIR /app
COPY BookVacationService BookVacationService
COPY Contracts Contracts
RUN dotnet restore BookVacationService/BookVacationService.csproj
RUN dotnet publish BookVacationService/BookVacationService.csproj -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/runtime:5.0.8-buster-slim-amd64 AS release
WORKDIR /app
ENV DOTNET_ENVIRONMENT=Production
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "BookVacationService.dll"]
