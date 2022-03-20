FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG GithubToken
ENV GithubToken=$GithubToken
WORKDIR /src
COPY ["NuGet.config", "NuGet.config"]
COPY ["FlightService.Api/FlightService.Api.csproj", "FlightService.Api/"]
COPY ["FlightService.Contracts/FlightService.Contracts.csproj", "FlightService.Contracts/"]
COPY ["FlightService.Infrastructure/FlightService.Infrastructure.csproj", "FlightService.Infrastructure/"]
COPY ["FlightService.Domain/FlightService.Domain.csproj", "FlightService.Domain/"]
RUN dotnet restore "FlightService.Api/FlightService.Api.csproj"
COPY . .
WORKDIR "/src/FlightService.Api"
RUN dotnet build "FlightService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlightService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlightService.Api.dll"]
