FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY ["build/output", "."]

USER $APP_UID

ENTRYPOINT ["dotnet", "Binacle.Net.Api.dll"]
