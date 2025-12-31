# Basket uses .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/Basket/MyShopWebSite.Basket/MyShopWebSite.Basket.csproj", "Services/Basket/MyShopWebSite.Basket/"]
RUN dotnet restore "Services/Basket/MyShopWebSite.Basket/MyShopWebSite.Basket.csproj"

COPY . .
WORKDIR "/src/Services/Basket/MyShopWebSite.Basket"
RUN dotnet publish "MyShopWebSite.Basket.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyShopWebSite.Basket.dll"]