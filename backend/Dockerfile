FROM mcr.microsoft.com/dotnet/nightly/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore backend.csproj --disable-parallel
RUN dotnet publish backend.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/nightly/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT [ "dotnet",  "backend.dll"]