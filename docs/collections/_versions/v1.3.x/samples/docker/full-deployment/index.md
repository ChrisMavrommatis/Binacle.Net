---
title: Full Deployment
permalink: /version/v1.3.x/samples/docker/full-deployment/
nav:
  order: 2
  parent: Docker
  icon: 2️⃣
---

This guide demonstrates how to set up and run Binacle.Net with all features enabled,
including Azurite as the database provider for the Service Module, OpenTelemetry for monitoring, and Aspire Dashboard 
for observability.


## 🛠️ Prerequisites

Before you start, make sure you have the following installed on your machine.

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://www.docker.com/get-started) (included with Docker Desktop)


## Download the following files

- [`docker-compose.yml`]({% vlink /samples/docker/full-deployment/docker-compose.yml %}){:download="" target="_blank"}
- [`Presets.json`]({% vlink /samples/docker/full-deployment/Presets.json %}){:download="" target="_blank"}
- [`JwtAuth.json`]({% vlink /samples/docker/full-deployment/JwtAuth.json %}){:download="" target="_blank"}
- [`OpenTelemetry.Production.json`]({% vlink /samples/docker/full-deployment/OpenTelemetry.Production.json %}){:download="" target="_blank"}
- [`aspire-dashboard-config.json`]({% vlink /samples/docker/full-deployment/aspire-dashboard-config.json %}){:download="" target="_blank"}


Place these files in a directory of your choice. This directory will be your project root.

## Customize (Optional)

> **Generate your own OTLP API key.** `aspire-dashboard-config.json` ships a placeholder as `PrimaryApiKey`,
> and `docker-compose.yml` repeats it in `OTEL_EXPORTER_OTLP_HEADERS`. It is published here, so anyone can read
> it. Replace it in both files with a value you generate yourself, and keep the two the same or the app cannot
> send telemetry to the dashboard.
{:.block-warning}

Edit the `Presets.json` file to adjust the bin configurations as per your needs.

Create a `.env` file in the same directory with the content:
```text
 COMPOSE_PROJECT_NAME=binacle-net-full-deployment
```  

This will set the project name for Docker Compose, allowing you to run multiple instances without conflicts.

## 🚀 Running the Application
In the project directory, start the application by running:

```bash
docker compose up
```

This will launch Binacle.Net with the following features:
- 🌐 **Swagger UI**: http://localhost:8080/swagger/ for easy API exploration.
- 💾 **Azurite Storage Emulator**: Simulates Azure Storage locally. Data is persisted in the `./azurite` folder 
  across container restarts.
- ⚙️ **Service Module**: Uses Azurite as its database provider for local storage operations.
- 🖥️ **UI Module**: Accessible via http://localhost:8080/, offering an interactive packing demo.
- 📊 **OpenTelemetry**: Collects telemetry data for monitoring and exports it to the **Aspire Dashboard**.
- 📈 **Aspire Dashboard**: Accessible via http://localhost:18888 for real-time metrics and performance monitoring.
- 📂 **Logs Folder**: A `./data/logs` folder will be created to store application logs for monitoring and debugging.


## 🔍 Accessing the API, UI and Aspire Dashboard

- **API Documentation (Swagger UI)**:<br>
  http://localhost:8080/swagger/ <br>
  Use this to explore and test API endpoints directly.

- **UI Module (Packing Demo)**:<br>
  http://localhost:8080/<br>
  Interact with Binacle.Net through a user-friendly interface.

- **Aspire Dashboard**:<br>
  http://localhost:18888<br>
  Monitor real-time metrics and performance of Binacle.Net services.


## ⚙️Customizing the Configuration

- 🗂️**Custom Presets**:  
  Edit the `Presets.json` file to modify bin configurations. Changes will take effect after restarting the application:
  ```bash
  docker compose down
  docker compose up
  ```
- 🔐 **JWT Authentication**:  
  Adjust authentication settings in the `JwtAuth.json` file as needed.
- 📊 **OpenTelemetry**:  
  Customize telemetry configuration by editing `OpenTelemetry.Production.json`.
- 📈 **Aspire Dashboard**:  
  Adjust settings in `aspire-dashboard-config.json` to fine-tune the dashboard. Its `PrimaryApiKey` is a
  placeholder - replace it with your own, and match it in `OTEL_EXPORTER_OTLP_HEADERS` in `docker-compose.yml`.

## 📂 Logs Folder
When running the application, a `./data` folder will be created to store application data, 
including logs for monitoring and debugging. It's important to ensure that the `./data` and `./data/logs` 
directories have write permissions for proper functionality.

### Setting Permissions
Run the following commands to create the directory and set the required permissions:

```bash
mkdir -p ./data/logs
sudo chmod -R 777 ./data
```
This will grant full access to `./data` and its subdirectories.

> 777 gives full access to all users. Adjust permissions as needed for security.
{:.block-note }

## 🛠️ Getting the Aspire Dashboard Login Token
The Aspire Dashboard generates a login URL with a token for access.

View the Logs: After starting the deployment, run the following command:
```bash
docker-compose logs -f aspire-dashboard
```
Locate the Login URL: In the logs, find a line like:
```text
Login to the dashboard at http://localhost:18888/login?t=your_token_here.
```
Access the Dashboard: Copy the URL and open it in your browser. You’ll be logged in automatically

## 📄 Additional Resources
- [Docker Compose Reference](https://docs.docker.com/compose/)
- [Aspire Dashboard Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/overview?tabs=bash)
- [OpenTelemetry Documentation](https://opentelemetry.io/docs/)

Happy packing! 📦✨
