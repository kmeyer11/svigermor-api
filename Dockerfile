FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SvigermorApi.Api/SvigermorApi.Api.csproj", "SvigermorApi.Api/"]
COPY ["SvigermorApi.Core/SvigermorApi.Core.csproj", "SvigermorApi.Core/"]

RUN dotnet restore "SvigermorApi.Api/SvigermorApi.Api.csproj"
COPY . .
WORKDIR "/src/SvigermorApi.Api"
RUN dotnet build "SvigermorApi.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SvigermorApi.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SvigermorApi.Api.dll"]
