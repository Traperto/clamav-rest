FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /app

COPY . . 

RUN dotnet restore ./VirusScannerService.sln

COPY . .

RUN dotnet publish ./VirusScannerService/VirusScannerService.csproj --output /out/ --configuration Release

# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "VirusScannerService.dll"]