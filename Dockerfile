FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

ARG VERSION
ENV BINACLE_VERSION=$VERSION

# Copy everything needed to run the app from the "build" stage.
COPY ["build/output", "."]

USER $APP_UID

ENTRYPOINT ["dotnet", "Binacle.Net.dll"]
