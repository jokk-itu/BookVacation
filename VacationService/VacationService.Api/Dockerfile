FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG GithubToken
ENV GithubToken=$GithubToken
WORKDIR /src
COPY ["NuGet.config", "NuGet.config"]
COPY ["VacationService/VacationService.Api/VacationService.Api.csproj", "VacationService.Api/"]
COPY ["VacationService/VacationService.Contracts/VacationService.Contracts.csproj", "VacationService.Contracts/"]
COPY ["VacationService/VacationService.Infrastructure/VacationService.Infrastructure.csproj", "VacationService.Infrastructure/"]
RUN dotnet restore "VacationService.Api/VacationService.Api.csproj"
COPY VacationService/ .
WORKDIR "/src/VacationService.Api"
RUN dotnet build "VacationService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VacationService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VacationService.Api.dll"]
