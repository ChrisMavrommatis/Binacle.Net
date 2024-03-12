FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY ["Build/Output", "."]

ENTRYPOINT ["dotnet", "Binacle.Net.Api.dll"]
