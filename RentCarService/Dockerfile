# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0.301-buster-slim-amd64 AS build
WORKDIR /app
COPY RentCarService RentCarService
COPY Contracts Contracts
RUN dotnet restore RentCarService/RentCarService.csproj
RUN dotnet publish RentCarService/RentCarService.csproj -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/runtime:5.0.8-buster-slim-amd64 AS release
WORKDIR /app
ENV DOTNET_ENVIRONMENT=Production
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "RentCarService.dll"]
