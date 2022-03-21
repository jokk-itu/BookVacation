FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG GithubToken
ENV GithubToken=$GithubToken
WORKDIR /src
COPY ["NuGet.config", "NuGet.config"]
COPY ["HotelService/HotelService.Api/HotelService.Api.csproj", "HotelService.Api/"]
COPY ["HotelService/HotelService.Contracts/HotelService.Contracts.csproj", "HotelService.Contracts/"]
COPY ["HotelService/HotelService.Domain/HotelService.Domain.csproj", "HotelService.Domain/"]
COPY ["HotelService/HotelService.Infrastructure/HotelService.Infrastructure.csproj", "HotelService.Infrastructure/"]
RUN dotnet restore "HotelService.Api/HotelService.Api.csproj"
COPY HotelService .
WORKDIR "/src/HotelService.Api"
RUN dotnet build "HotelService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HotelService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelService.Api.dll"]
