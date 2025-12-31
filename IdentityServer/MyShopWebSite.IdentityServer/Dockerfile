# IdentityServer uses .NET Core 3.1 with IdentityServer4
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["IdentityServer/MyShopWebSite.IdentityServer/MyShopWebSite.IdentityServer.csproj", "IdentityServer/MyShopWebSite.IdentityServer/"]
RUN dotnet restore "IdentityServer/MyShopWebSite.IdentityServer/MyShopWebSite.IdentityServer.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/IdentityServer/MyShopWebSite.IdentityServer"
RUN dotnet publish "MyShopWebSite.IdentityServer.csproj" -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyShopWebSite.IdentityServer.dll"]