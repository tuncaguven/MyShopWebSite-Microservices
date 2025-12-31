# Discount uses .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/Discount/MyShopWebSite.Discount/MyShopWebSite.Discount.csproj", "Services/Discount/MyShopWebSite.Discount/"]
RUN dotnet restore "Services/Discount/MyShopWebSite.Discount/MyShopWebSite.Discount.csproj"

COPY . .
WORKDIR "/src/Services/Discount/MyShopWebSite.Discount"
RUN dotnet publish "MyShopWebSite.Discount.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyShopWebSite.Discount.dll"]