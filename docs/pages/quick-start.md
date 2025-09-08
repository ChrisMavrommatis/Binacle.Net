---
title: Quick Start
permalink: quick-start
nav:
  order: 1
  icon: 🚀
---

Getting started with Binacle.Net is easy! You can run it locally using Docker or deploy it to the cloud, all with
minimal setup. The setup runs Binacle.Net with **Swagger UI** and the **UI Module**.

For more information visit the dedicated **Quick Start Guide** in the specific version documentation.

## 🖥️ Run Locally with Docker

To run Binacle.Net locally using Docker, follow these steps:

### 1️⃣ Install Docker

Make sure you have Docker installed on your machine. You can download it from
the [Docker Website](https://www.docker.com/get-started).

### 2️⃣ Run Binacle.Net

Open your terminal and run the following command to start Binacle.Net:

```bash
docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -e UI_MODULE=True binacle/binacle-net:latest
```

This command does the following:

- `-d`: Runs the container in detached mode.
- `--name binacle-net`: Names the container "binacle-net".
- `-p 8080:8080`: Maps port 8080 of the container to port 8080 on your host machine.
- `-e SWAGGER_UI=True`: Enables Swagger UI.
- `-e UI_MODULE=True`: Enables the UI Module.
- `binacle/binacle-net:latest`: Specifies the Docker image to use.

### 3️⃣ Access Binacle.Net

- **Swagger UI**: http://localhost:8080/swagger/
- **UI Module**: http://localhost:8080/

## ☁️ Cloud Deployment

Want to deploy Binacle.Net to the cloud? Here are the recommended platforms:

- 🔷 [Azure App Service](https://azure.microsoft.com/en-us/products/app-service/)
- 🟠 [AWS Elastic Container Service (ECS)](https://aws.amazon.com/ecs/) or [Fargate](https://aws.amazon.com/fargate/) for
  serverless container management.
- 🟢 [Google Cloud Run](https://cloud.google.com/run)
- 🟣 [Koyeb](https://www.koyeb.com/)
- 🌊 [Digital Ocean App Platform](https://www.digitalocean.com/products/app-platform/)

## 🗂️ Choosing the Right Option

| Deployment     | Best For                                                        | Platform                                                                             |
|----------------|-----------------------------------------------------------------|--------------------------------------------------------------------------------------|
| Local (Docker) | Quick development, testing, and demos                           | [Docker](https://www.docker.com/)                                                    |
| Azure          | Seamless integration with Microsoft services, scalable web apps | [Azure App Service](https://azure.microsoft.com/en-us/products/app-service/)         |
| AWS            | High-scale microservices, flexible container management         | [AWS ECS](https://aws.amazon.com/ecs/) or [Fargate](https://aws.amazon.com/fargate/) |
| Google Cloud   | Serverless deployments, cost-effective auto-scaling APIs        | [Google Cloud Run](https://cloud.google.com/run)                                     |
| Koyeb          | Simple, easy to use and offers affordable pricing.              | [Koyeb](https://www.koyeb.com/)                                                      |
| Digital Ocean  | Easy-to-use platform for small to medium apps.                  | [Digital Ocean App Platform](https://www.digitalocean.com/products/app-platform/)    |

You’re now ready to get started with Binacle.Net—locally or in the cloud! 🚀