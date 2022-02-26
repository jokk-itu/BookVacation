# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 AS build
ARG Configuration
ARG GithubToken
ENV GithubToken=$GithubToken
WORKDIR /app
COPY NuGet.config NuGet.config
COPY HotelService.Api HotelService.Api
COPY HotelService.Domain HotelService.Domain
COPY HotelService.Contracts HotelService.Contracts
COPY HotelService.Infrastructure HotelService.Infrastructure
RUN dotnet publish HotelService.Api/HotelService.Api.csproj -c $Configuration -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-amd64 AS release
ARG Environment
EXPOSE 80
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=$Environment
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "HotelService.Api.dll"]