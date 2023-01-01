#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 34567/tcp

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["rustdeskapi/RustDeskApi.csproj", "rustdeskapi/"]
RUN dotnet restore "rustdeskapi/RustDeskApi.csproj"
COPY . .
WORKDIR /src/rustdeskapi
RUN dotnet build "RustDeskApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RustDeskApi.csproj" -c Release -o /app/publish
RUN rm /app/publish/appsettings.Development.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RustDeskApi.dll"]