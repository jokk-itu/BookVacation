# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0.301-buster-slim-amd64 AS build
ARG Configuration
WORKDIR /app
COPY RentCarService RentCarService
COPY Contracts Contracts
RUN dotnet publish RentCarService/RentCarService.csproj -c $Configuration -o out

FROM mcr.microsoft.com/dotnet/runtime:5.0.8-buster-slim-amd64 AS release
ARG Environment
WORKDIR /app
ENV DOTNET_ENVIRONMENT=$Environment
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "RentCarService.dll"]
