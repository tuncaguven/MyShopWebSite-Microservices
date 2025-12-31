# Frontend uses .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Frontend/MultiShop.WebUI/MultiShop.WebUI.csproj", "Frontend/MultiShop.WebUI/"]
RUN dotnet restore "Frontend/MultiShop.WebUI/MultiShop.WebUI.csproj"

COPY . .
WORKDIR "/src/Frontend/MultiShop.WebUI"
RUN dotnet publish "MultiShop.WebUI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MultiShop.WebUI.dll"]