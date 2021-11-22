# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0.301-buster-slim-amd64 AS build
ARG Configuration
WORKDIR /app
COPY Api Api
COPY Contracts Contracts
RUN dotnet publish Api/Api.csproj -c $Configuration -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0.7-buster-slim-amd64 AS release
ARG Environment
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=$Environment
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Api.dll"]