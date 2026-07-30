---
title: Core
permalink: /version/v1.3.x/configuration/core/
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
    └── Presets.json
```

## 🎛️ Presets
Binacle.Net allows you to predefine bin configurations using presets, so you don’t have to send them with each request.

visit the [Presets]({% vlink /configuration/core/presets.md %}) page for more details.

## 🔑 Swagger UI
Swagger UI provides an interactive interface for exploring and testing the API.

By default, it is **disabled**. To enable it, set the environment variable:

```bash
SWAGGER_UI=True
```

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