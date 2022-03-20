FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG GithubToken
ENV GithubToken=$GithubToken
WORKDIR /src
COPY ["NuGet.config", "NuGet.config"]
COPY ["TicketService.Api/TicketService.Api.csproj", "TicketService.Api/"]
COPY ["TicketService.Infrastructure/TicketService.Infrastructure.csproj", "TicketService.Infrastructure/"]
COPY ["TicketService.Contracts/TicketService.Contracts.csproj", "TicketService.Contracts/"]
COPY ["TicketService.Domain/TicketService.Domain.csproj", "TicketService.Domain/"]
RUN dotnet restore "TicketService.Api/TicketService.Api.csproj"
COPY . .
WORKDIR "/src/TicketService.Api"
RUN dotnet build "TicketService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TicketService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicketService.Api.dll"]
