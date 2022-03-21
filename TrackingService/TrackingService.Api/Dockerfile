FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG GithubToken
ENV GithubToken=$GithubToken
WORKDIR /src
COPY ["NuGet.config", "NuGet.config"]
COPY ["TrackingService/TrackingService.Api/TrackingService.Api.csproj", "TrackingService.Api/"]
COPY ["TrackingService/TrackingService.Contracts/TrackingService.Contracts.csproj", "TrackingService.Contracts/"]
COPY ["TrackingService/TrackingService.Domain/TrackingService.Domain.csproj", "TrackingService.Domain/"]
COPY ["TrackingService/TrackingService.Infrastructure/TrackingService.Infrastructure.csproj", "TrackingService.Infrastructure/"]
RUN dotnet restore "TrackingService.Api/TrackingService.Api.csproj"
COPY TrackingService/ .
WORKDIR "/src/TrackingService.Api"
RUN dotnet build "TrackingService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrackingService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrackingService.Api.dll"]
