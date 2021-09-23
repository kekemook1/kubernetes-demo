FROM mcr.microsoft.com/dotnet/aspnet:5.0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

COPY bin/Release/net5.0/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "webapp.dll"]
