---
title: Quick Start
nav:
  order: 1
  icon: 🚀
---

Getting started with Binacle.Net is simple.

The default setup includes **Swagger UI**, **Scalar UI** and  **UI Module** for easy testing and interaction.

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

This starts Binacle.Net with Swagger and Scalar UI and the UI Module enabled on port 8080.

##### 3️⃣ Access Locally

- Swagger UI: [http://localhost:8080/swagger/](http://localhost:8080/swagger/)
- Swagger UI: [http://localhost:8080/scalar/](http://localhost:8080/scalar/)
- UI Module: [http://localhost:8080/](http://localhost:8080/)

