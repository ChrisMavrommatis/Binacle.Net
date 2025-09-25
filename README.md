# Binacle.Net

## 📝 Overview
Binacle.Net is an API created to address the 3D Bin Packing Problem in real time.

It is an ideal fit for e-commerce platforms offering parcel shipments to self-service locker systems,
providing optimal bin packing calculations to ensure efficient use of space and smooth customer experiences during checkout.

Explore the following sections to get started and learn more about Binacle.Net:


## 🚀 Quick Start
Simply execute the following command in your terminal:

```bash
docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -e UI_MODULE=True binacle/binacle-net:latest
```

### 🌐 Access the Interface
- API Documentation (Swagger UI): http://localhost:8080/swagger/
- UI Module & Packing Demo: http://localhost:8080/

Start exploring Binacle.Net now! 🚀

## 🔍 [How Binacle.Net Works](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/How-Binacle.Net-Works)
Discover the algorithms behind Binacle.Net and learn how it handles fitting and packing items into bins.

## 📡 [About the API](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/About-the-API)
Understand the API endpoints, including how to use presets, send custom bin and item data, and query by preset to find the most efficient packing solution.

Below are the main API versions and related resources:
- 🚨 [V1](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/API-‐-V1): The original version of the API. **It is deprecated**
- ⚖️ [V2](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/API-‐-V2): Introduces several new features, including the packing function which tracks the positions of the items.
- 🧪 [V3](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/API-‐-V3): Introduces algorithm selection: **This version is still experimental**
- 👥 [Users](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/API-‐-Users): Becomes available only after you enable the Service Module.

## 🔗 [Integration Guide](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Integration-Guide)
Learn how to integrate Binacle.Net into your platform. Includes detailed sections on:

- [📏 Dimensions and Unit of Measurement](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Integration-Guide-‐-Dimensions-and-Unit-of-Measurement)
- [📦 Your Bin Set](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Integration-Guide-‐-Your-Bin-Set)
- [🌟 Typical Integration Process](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Integration-Guide-‐-Typical-Integration-Process)

## 🔧 [Configuration](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Configuration)
Customize Binacle.Net to suit your environment. Explore the following configuration modules:

- [🏗️ Core](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Configuration-‐-Core): Provides essential API functionality, including Presets customization.
- [📊 Diagnostics Module](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Configuration-‐-Diagnostics-Module): Configure logging, health checks, and telemetry.
- [🛡️ Service Module](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Configuration-‐-Service-Module): Enable rate limiting, authentication, and cloud logging.
- [🖥️ UI Module](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Configuration-‐-UI-Module): Enable the visual demo interface for packing simulations.


## 📖 [Wiki](https://github.com/ChrisMavrommatis/Binacle.Net/wiki)
This wiki will guide you through every aspect of using and integrating Binacle.Net. Whether you're looking for a quick deployment or deep customization, all the resources you need are here!

## 🔗 Links
- [🐳 Binacle.Net on Dockerhub](https://hub.docker.com/r/binacle/binacle-net)
- [Postman Collection](https://www.postman.com/chrismavrommatis/workspace/binacle-net/)

  
