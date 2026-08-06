---
title: Quick Start
nav:
  order: 1
  icon: 🚀
---

Getting started with Binacle.Net is simple.
Run it locally using Docker or deploy it to the cloud with minimal setup.

The default setup includes **Swagger UI** and the **UI Module** for easy testing and interaction.

For version-specific details, see the
[Quick Start Guide for {{ site.data.versions.current }}]({{ '/version/' | append: site.data.versions.current | append: '/quick-start/' | relative_url }}),
or pick your version from the [Versions]({% link _common_pages/version.html %}) page.

## 🖥️ Run Locally with Docker

##### 1️⃣ Install Docker

Download and install Docker from [docker.com](https://www.docker.com/get-started).

##### 2️⃣ Launch Binacle.Net

Run this command in your terminal:

```bash
docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -e UI_MODULE=True binacle/binacle-net:latest
```

This starts Binacle.Net with Swagger UI and the UI Module on port 8080.

> ⚠️ `latest` always follows the newest release, and releases can bring breaking changes.
>
> It is fine for a quick look. For anything you keep, pin a version instead - the Quick Start Guide for your
> version gives the exact tag to use.
{: .block-warning}

##### 3️⃣ Access Locally

- Swagger UI: [http://localhost:8080/swagger/](http://localhost:8080/swagger/)
- UI Module: [http://localhost:8080/](http://localhost:8080/)

---

## ☁️ Recommended Cloud Deployment Platforms

Binacle.Net works on all major cloud platforms. Choose based on your needs:

- 🔷 **Azure App Service**: Tight Microsoft ecosystem integration; scalable web apps.
- 🟠 **AWS ECS / Fargate**: Powerful container orchestration; flexible microservice scaling.
- 🟢 **Google Cloud Run**: Serverless container runtime; automatic scaling.
- 🟣 **Koyeb**: Simple deployment with affordable pricing, ideal for smaller workloads.
- 🌊 **Digital Ocean App Platform**: User-friendly with straightforward app management, good for small to medium
  deployments.

---

## 🗂️ Deployment Options Comparison

| Deployment     | Best Use Case                                | Platform URL                                                                        |
|----------------|----------------------------------------------|-------------------------------------------------------------------------------------|
| Local (Docker) | Quick development, testing, demos            | [Docker](https://www.docker.com/)                                                   |
| Azure          | Microsoft stack integration, scalable apps   | [Azure App Service](https://azure.microsoft.com/en-us/products/app-service/)        |
| AWS            | Large-scale microservices, container scaling | [AWS ECS](https://aws.amazon.com/ecs/) / [Fargate](https://aws.amazon.com/fargate/) |
| Google Cloud   | Serverless, efficient API deployment         | [Google Cloud Run](https://cloud.google.com/run)                                    |
| Koyeb          | Simple, cost-effective small workloads       | [Koyeb](https://www.koyeb.com/)                                                     |
| Digital Ocean  | Easy, affordable for SMB apps                | [Digital Ocean](https://www.digitalocean.com/products/app-platform/)                |

---

Choose the platform that aligns best with your workload, scalability needs, and budget to deploy Binacle.Net smoothly in
the cloud. 🚀