FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /app

COPY . . 

RUN dotnet restore ./Traperto.ClamAvRest.sln

COPY . .

RUN dotnet publish ./Traperto.ClamAvRest.Main/Traperto.ClamAvRest.Main.csproj --output /out/ --configuration Release

# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "Traperto.ClamAvRest.Main.dll"]