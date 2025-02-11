# Binacle.NET

## 📝 Overview
Binacle.NET is an API created to address the 3D Bin Packing Problem in real time.

It is an ideal fit for e-commerce platforms offering parcel shipments to self-service locker systems, providing optimal bin packing calculations to ensure efficient use of space and smooth customer experiences during checkout.

Explore the following sections to get started and learn more about Binacle.NET:


## 🚀 [Quick Start](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Quick-Start)
Get started with Binacle.Net in just a few steps! The [Quick Start](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Quick-Start) Guide covers the basics you need to run it quickly.

### ⚡ Run Binacle.Net with Docker and UI Module
Simply execute the following command in your terminal:

```bash
docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -e UI_MODULE=True binacle/binacle-net:latest
```

### 🌐 Access the Interface
- API Documentation (Swagger UI): http://localhost:8080/swagger/
- UI Module & Packing Demo: http://localhost:8080/

Start exploring Binacle.Net now! 🚀

## 🔍 [How Binacle.NET Works](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/How-Binacle.Net-Works)
Discover the algorithm behind Binacle.NET, including its First Fit Decreasing (FFD) strategy, and learn how it handles fitting and packing items into bins.

## 📦 [About the API](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/About-the-API)
Understand the core API endpoints, including how to use presets, send custom bin and item data, and query by preset to find the most efficient packing solution.

## 🔗 [Integration](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Integration)
Learn how to integrate Binacle.NET into your platform. Includes detailed sections on:

- [Dimensions and Unit of Measurement](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Integration%3A-Dimensions-and-Unit-of-Measurement)
- [Bin Set](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Integration%3A-The-Bin-Set)
- [Typical Implementation](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Integration%3A-A-Typical-Implementation)


## ⚙️ [Configuration](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Configuration)
Customize Binacle.NET to suit your environment. Explore the following configuration modules:

- [Presets](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Configuration%3A-Presets): Set up pre-defined bin sets.
- [Diagnostics Module](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Configuration%3A-Diagnostics-Module): Configure logging, health checks, and telemetry.
- [Service Module](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Configuration%3A-Service-Module): Enable rate limiting, authentication, and cloud logging.
- [UI Module](https://github.com/ChrisMavrommatis/Binacle.Net/wiki/Configuration%3A-UI-Module): Enable the visual demo interface for packing simulations.


## 📖 [Wiki](https://github.com/ChrisMavrommatis/Binacle.Net/wiki)
This wiki will guide you through every aspect of using and integrating Binacle.NET. Whether you're looking for a quick deployment or deep customization, all the resources you need are here!

## 🔗 Links
- [🐳 Binacle.Net on Dockerhub](https://hub.docker.com/r/binacle/binacle-net)
- [Postman Collection](https://www.postman.com/chrismavrommatis/workspace/binacle-net/)

  
