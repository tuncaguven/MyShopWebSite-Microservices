FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/Catalog/MyShopWebSite.Catalog/MyShopWebSite.Catalog.csproj", "Services/Catalog/MyShopWebSite.Catalog/"]
RUN dotnet restore "Services/Catalog/MyShopWebSite.Catalog/MyShopWebSite.Catalog.csproj"

COPY . .
WORKDIR "/src/Services/Catalog/MyShopWebSite.Catalog"
RUN dotnet publish "MyShopWebSite.Catalog.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyShopWebSite.Catalog.dll"]
