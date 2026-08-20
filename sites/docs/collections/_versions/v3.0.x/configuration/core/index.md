---
title: Core
permalink: /version/v3.0.x/configuration/core/
nav:
  parent: Configuration
  order: 1
  icon: 🏗️
---


The Core module is the foundation of Binacle.Net.

It provides essential API functionality, as detailed in the [API]({% vlink /api/index.md %}) page.

It also supports customizable presets and includes Swagger UI, which is disabled by default.

## ⚙️ Configuration
All configuration files for Binacle.Net Core are located in the `/app/Config_Files` directory.

### 📑 Directory Structure
```text
app
└── Config_Files
    ├── Presets.json
    ├── Cors.json
    └── ForwardedHeaders.json
```

## 🎛️ Presets
Binacle.Net allows you to predefine bin configurations using presets, so you don't have to send them with each request.

visit the [Presets]({% vlink /configuration/core/presets.md %}) page for more details.

## 🌐 Running Behind a Proxy
If a proxy, load balancer or CDN sits in front of Binacle.Net, it sees the proxy as the caller unless you tell
it otherwise.

Visit the [Forwarded Headers]({% vlink /configuration/core/forwarded-headers.md %}) page for more details.

## 🔑 Swagger UI
Swagger UI provides an interactive interface for exploring and testing the API.

By default, it is **disabled**. To enable it, set the environment variable:

```bash
SWAGGER_UI=True
```

## 🖥️ Scalar UI
Scalar UI is a web-based interface for interacting with Binacle.Net.
It's an alternative to Swagger UI, providing a more user-friendly experience for managing bins and viewing results.

By default, it is **disabled**. To enable it, set the environment variable:

```bash
SCALAR_UI=True
```

## 🧰 Debug Endpoint
`/_debug` echoes your own request back to you: the address the app resolved you to, and every header you sent.
It is the quickest way to see what a proxy is actually sending.

By default, it is **disabled**. To enable it, set the environment variable:

```bash
DEBUG_ENDPOINT=True
```

> The endpoint needs no authentication and echoes **every** header, including `Authorization`. Turn it on to
> read a value, then turn it off again. Do not leave it enabled where other people can reach it.
{: .block-warning}

## 🌍 CORS
CORS only matters when a **browser** calls the API directly. Server-to-server callers are unaffected.

Allowed origins are configured in `Cors.json`, which is **not** in the image - you supply it. Until you do, no
browser origin is allowed through.

```json
{
  "Cors": {
    "CoreApi": {
      "AllowedOrigins": [ "https://your-site.example" ]
    }
  }
}
```

Give each origin its exact scheme, host and port.

- 📁 **Location**: `/app/Config_Files`
- 📌 **Full Path**: `/app/Config_Files/Cors.json`

## 🔌 Changing the Internal Port
By default, Binacle.Net runs on port `8080`.

To change this inside a container, set the `ASPNETCORE_HTTP_PORTS` environment variable.

Example: Run on port `80` inside the container
```bash
docker run --name binacle-net \
  -e ASPNETCORE_HTTP_PORTS=80 \
  -e SWAGGER_UI=True \
  -p 8080:80 \
  binacle/binacle-net:{{ page.version_tag }}
```
