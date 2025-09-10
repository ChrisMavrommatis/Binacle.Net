---
title: Quick Start
nav:
  order: 3
  icon: 🚀
---

Getting started with Binacle.Net is easy! 

The following setup runs Binacle.Net with **Swagger UI** and the **UI Module**.

---

## 🖥️ Run Locally with Docker

The fastest way to try Binacle.Net is by running it in a Docker container.

##### 1️⃣ Install Docker

Ensure Docker is installed on your system. If not, follow the instructions on
the [Docker website](https://www.docker.com/get-started/).

##### 2️⃣ Run Binacle.Net

Open your terminal and run:

```bash
docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -e UI_MODULE=True binacle/binacle-net:{{ page.version_tag }}
```

This command will:

- ✔️ Pull the {{ page.version }} Binacle.Net image.
- ✔️ Start the container with the **Swagger UI** and **UI Module** enabled.
- ✔️ Expose the API on port 8080.

##### 3️⃣ Access Binacle.Net

- **Swagger UI**: http://localhost:8080/swagger/
- **UI Module**: http://localhost:8080/

---

## 📦 Run with Docker Compose

For more advanced setups, you can use Docker Compose. Choose one of the
provided [Samples]({% vlink /samples/docker/index.md %}) for a quick start:

- 🔹 [Minimal Setup]({% vlink /samples/docker/minimal-setup/index.md %}): Set up
  Binacle.Net with minimal configuration.
- 🔹 [Full Deployment]({% vlink /samples/docker/full-deployment/index.md %}): Run
  Binacle.Net with all features enabled, including Azurite for local storage, Open Telemetry and Aspire Dashboard.
- 🔹 [UI Module Only]({% vlink /samples/docker/ui-module-only/index.md %}): Run
  Binacle.Net with just the UI Module for quick visual demos.


