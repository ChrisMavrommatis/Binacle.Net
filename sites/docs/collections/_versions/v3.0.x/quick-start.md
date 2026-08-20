---
title: Quick Start
nav:
  order: 1
  icon: 🚀
---

Getting started with Binacle.Net is simple.

The setup below turns on **Swagger UI**, **Scalar UI** and the **UI Module** so you can try it out from a
browser. All three are off by default.

## 🖥️ Run Locally with Docker

##### 1️⃣ Install Docker

Download and install Docker from [docker.com](https://www.docker.com/get-started).

##### 2️⃣ Launch Binacle.Net

Run this command in your terminal:

```bash
docker run -d --name binacle-net \
  -p 8080:8080 \
  -e SWAGGER_UI=True \
  -e SCALAR_UI=True \
  -e UI_MODULE=True \
  binacle/binacle-net:{{ page.version_tag }}
```

This starts Binacle.Net with Swagger UI, Scalar UI and the UI Module enabled on port 8080.

The tag `{{ page.version_tag }}` is the minor tag: it follows the newest patch in this line and never a
breaking change.

##### 3️⃣ Access Locally

- Swagger UI: [http://localhost:8080/swagger/](http://localhost:8080/swagger/)
- Scalar UI: [http://localhost:8080/scalar/](http://localhost:8080/scalar/)
- UI Module: [http://localhost:8080/](http://localhost:8080/)

## ➡️ Where to go next

- [API]({% vlink /api/index.md %}) - the endpoints, for V3 and experimental V4.
- [Presets]({% vlink /configuration/core/presets.md %}) - replace the example bins with your own.
- [Samples]({% vlink /samples/index.md %}) - Docker Compose and Kubernetes setups to copy, including one for
  running behind your own backend.
