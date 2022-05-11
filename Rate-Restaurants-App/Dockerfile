FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Rate-Restaurants-App/Rate-Restaurants-App.csproj", "Rate-Restaurants-App/"]
RUN dotnet restore "Rate-Restaurants-App/Rate-Restaurants-App.csproj"
COPY . .
WORKDIR "/src/Rate-Restaurants-App"
RUN dotnet build "Rate-Restaurants-App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rate-Restaurants-App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rate-Restaurants-App.dll"]
