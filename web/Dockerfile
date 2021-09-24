FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /build
COPY webapp.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/aspnet:5.0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
COPY --from=build /build/bin/Release/net5.0/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "webapp.dll"]
