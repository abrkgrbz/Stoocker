FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Stoocker.API/Stoocker.API.csproj", "Stoocker.API/"]
COPY ["Stoocker.Infrastructure/Stoocker.Infrastructure.csproj", "Stoocker.Infrastructure/"]
COPY ["Stoocker.Application/Stoocker.Application.csproj", "Stoocker.Application/"]
COPY ["Stoocker.Domain/Stoocker.Domain.csproj", "Stoocker.Domain/"]
COPY ["Stoocker.Persistence/Stoocker.Persistence.csproj", "Stoocker.Persistence/"]

RUN dotnet restore "Stoocker.API/Stoocker.API.csproj"
COPY . .
WORKDIR "/src/Stoocker.API"
RUN dotnet build "Stoocker.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stoocker.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stoocker.API.dll"]