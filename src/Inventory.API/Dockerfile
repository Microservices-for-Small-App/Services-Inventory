#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Inventory.API/Inventory.API.csproj", "src/Inventory.API/"]
COPY ["src/Inventory.Contracts/Inventory.Contracts.csproj", "src/Inventory.Contracts/"]
COPY ["src/Inventory.Data/Inventory.Data.csproj", "src/Inventory.Data/"]
RUN dotnet restore "src/Inventory.API/Inventory.API.csproj"
COPY ./src ./src
WORKDIR "/src/src/Inventory.API"
RUN dotnet build "Inventory.API.csproj" -c Release --no-restore -o /app/build

FROM build AS publish
RUN dotnet publish "Inventory.API.csproj" -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inventory.API.dll"]