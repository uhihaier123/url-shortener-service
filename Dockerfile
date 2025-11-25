FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /src

COPY ["UrlShortener.API/UrlShortener.API.csproj", "UrlShortener.API/"]

RUN dotnet restore "UrlShortener.API/UrlShortener.API.csproj"

COPY . .
WORKDIR "/src/UrlShortener.API"

RUN dotnet build "UrlShortener.API.csproj" -c Release -o /app/build

RUN dotnet publish "UrlShortener.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "UrlShortener.API.dll"]