FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG GithubToken
ENV GithubToken=$GithubToken
WORKDIR /src
COPY ["NuGet.config", "NuGet.config"]
COPY ["CarService.Api/CarService.Api.csproj", "CarService.Api/"]
COPY ["CarService.Contracts/CarService.Contracts.csproj", "CarService.Contracts/"]
COPY ["CarService.Domain/CarService.Domain.csproj", "CarService.Domain/"]
COPY ["CarService.Infrastructure/CarService.Infrastructure.csproj", "CarService.Infrastructure/"]
RUN dotnet restore "CarService.Api/CarService.Api.csproj"
COPY . .
WORKDIR "/src/CarService.Api"
RUN dotnet build "CarService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarService.Api.dll"]
