FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine AS base
WORKDIR /app
EXPOSE 34567/tcp

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["rustdeskapi/RustDeskApi.csproj", "rustdeskapi/"]

RUN dotnet restore "rustdeskapi/RustDeskApi.csproj"
COPY . .
RUN dotnet publish "rustdeskapi/RustDeskApi.csproj" --no-restore -c Release -r alpine-x64 -o /app/publish --self-contained true
RUN rm /app/publish/appsettings.Development.json

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

RUN apk --no-cache add icu-libs

ENTRYPOINT ["./RustDeskApi"]