# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 AS build
ARG Configuration
WORKDIR /app
COPY BookHotelService BookHotelService
COPY Contracts Contracts
COPY EventBusTransmitting EventBusTransmitting
RUN dotnet publish BookHotelService/BookHotelService.csproj -c $Configuration -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim-amd64 AS release
ARG Environment
WORKDIR /app
ENV DOTNET_ENVIRONMENT=$Environment
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "BookHotelService.dll"]