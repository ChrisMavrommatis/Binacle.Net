---
title: Quick Start
nav:
  order: 3
  icon: 🚀
---

Getting started with Binacle.Net is easy! You can run it locally using Docker or deploy it to the cloud, all with
minimal setup. The setup runs Binacle.Net with **Swagger UI** and the **UI Module**.

##### 🖥️ Run Locally with Docker

The fastest way to try Binacle.Net is by running it in a Docker container.

###### 1️⃣ Install Docker

Ensure Docker is installed on your system. If not, follow the instructions on
the [Docker website](https://www.docker.com/get-started/).

###### 2️⃣ Run Binacle.Net

Open your terminal and run:

```bash
docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -e UI_MODULE=True binacle/binacle-net:latest
```

This command will:

- ✔️ Pull the latest Binacle.Net image.
- ✔️ Start the container with the **Swagger UI** and **UI Module** enabled.
- ✔️ Expose the API on port 8080.

###### 3️⃣ Access Binacle.Net

- **Swagger UI**: http://localhost:8080/swagger/
- **UI Module**: http://localhost:8080/

##### 📦 Run with Docker Compose

For more advanced setups, you can use Docker Compose. Choose one of the
provided [Samples](https://github.com/ChrisMavrommatis/Binacle.Net/tree/main/samples/docker) for a quick start:

- 🔹 [Minimal Setup](https://github.com/ChrisMavrommatis/Binacle.Net/tree/main/samples/docker/minimal-setup): Set up
  Binacle.Net with minimal configuration.
- 🔹 [Full Deployment](https://github.com/ChrisMavrommatis/Binacle.Net/tree/main/samples/docker/full-deployment): Run
  Binacle.Net with all features enabled, including Azurite for local storage, Open Telemetry and Aspire Dashboard.
- 🔹 [UI Module Only](https://github.com/ChrisMavrommatis/Binacle.Net/tree/main/samples/docker/ui-module-only): Run
  Binacle.Net with just the UI Module for quick visual demos.

##### ☁️ Cloud Deployment

Want to deploy Binacle.Net to the cloud? Here are the recommended platforms:

- 🔷 [Azure App Service](https://azure.microsoft.com/en-us/products/app-service/)
- 🟠 [AWS Elastic Container Service (ECS)](https://aws.amazon.com/ecs/) or [Fargate](https://aws.amazon.com/fargate/) for
  serverless container management.
- 🟢 [Google Cloud Run](https://cloud.google.com/run)

##### 🗂️ Choosing the Right Option

| Deployment     | Best For                                                        | Platform                                                                             |
|----------------|-----------------------------------------------------------------|--------------------------------------------------------------------------------------|
| Local (Docker) | Quick development, testing, and demos                           | [Docker](https://www.docker.com/)                                                    |
| Azure          | Seamless integration with Microsoft services, scalable web apps | [Azure App Service](https://azure.microsoft.com/en-us/products/app-service/)         |
| AWS            | High-scale microservices, flexible container management         | [AWS ECS](https://aws.amazon.com/ecs/) or [Fargate](https://aws.amazon.com/fargate/) |
| Google Cloud   | Serverless deployments, cost-effective auto-scaling APIs        | [Google Cloud Run](https://cloud.google.com/run)                                     |

You’re now ready to get started with Binacle.Net—locally or in the cloud! 🚀