# Order uses .NET 8 with Clean Architecture
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all Order project files
COPY ["Services/Order/Presentation/MyShopWebSite.Order.WebApi/MyShopWebSite.Order.WebApi.csproj", "Services/Order/Presentation/MyShopWebSite.Order.WebApi/"]
COPY ["Services/Order/Core/MyShopWebSite.Order.Application/MyShopWebSite.Order.Application.csproj", "Services/Order/Core/MyShopWebSite.Order.Application/"]
COPY ["Services/Order/Core/MyShopWebSite.Order.Domain/MyShopWebSite.Order.Domain.csproj", "Services/Order/Core/MyShopWebSite.Order.Domain/"]
COPY ["Services/Order/Infrastructure/MyShopWebSite.Order.Persistence/MyShopWebSite.Order.Persistence.csproj", "Services/Order/Infrastructure/MyShopWebSite.Order.Persistence/"]

# Restore
RUN dotnet restore "Services/Order/Presentation/MyShopWebSite.Order.WebApi/MyShopWebSite.Order.WebApi.csproj"

# Copy all files and publish
COPY . .
WORKDIR "/src/Services/Order/Presentation/MyShopWebSite.Order.WebApi"
RUN dotnet publish "MyShopWebSite.Order.WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyShopWebSite.Order.WebApi.dll"]