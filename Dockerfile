FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

ARG VERSION
ENV BINACLE_VERSION=$VERSION

# Npgsql probes for GSSAPI whenever it opens a connection. The app works without it — we authenticate with a
# password, not Kerberos — but it prints "Cannot load library libgssapi_krb5.so.2" on every start, which reads
# like a fatal error in the logs of anyone running the Postgres backend. Cheaper to ship the library than to
# explain the message. Kept above the COPY so it caches across builds.
RUN apt-get update \
 && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
 && rm -rf /var/lib/apt/lists/*

# Copy everything needed to run the app from the "build" stage.
COPY ["build/binacle-net", "."]

# Logs, pack-logs, and the SQLite database are written here. It has to exist in the image and be owned by the
# app user: docker creates a mount point that the image does not have as root, and the app does not run as
# root, so a volume mounted over a missing /app/data is unwritable. A fresh named volume inherits this
# ownership from the image.
RUN mkdir -p /app/data && chown $APP_UID:$APP_UID /app/data

USER $APP_UID

ENTRYPOINT ["dotnet", "Binacle.Net.dll"]
