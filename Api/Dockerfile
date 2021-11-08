# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0.301-buster-slim-amd64 AS build
WORKDIR /app
COPY Api Api
COPY Contracts Contracts
RUN dotnet restore Api/Api.csproj
RUN dotnet publish Api/Api.csproj -c Release -o out --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:5.0.7-buster-slim-amd64 AS release
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Api.dll"]
