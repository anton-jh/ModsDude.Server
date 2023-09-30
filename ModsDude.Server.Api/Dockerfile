#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ModsDude.Server.Api/ModsDude.Server.Api.csproj", "ModsDude.Server.Api/"]
RUN dotnet restore "ModsDude.Server.Api/ModsDude.Server.Api.csproj"
COPY . .
WORKDIR "/src/ModsDude.Server.Api"
RUN dotnet build "ModsDude.Server.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ModsDude.Server.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ModsDude.Server.Api.dll"]