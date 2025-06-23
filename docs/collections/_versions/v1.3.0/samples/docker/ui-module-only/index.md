---
title: UI Module Only
permalink: /version/v1.3.0/samples/docker/ui-module-only/
nav:
  order: 3
  parent: Docker
  icon: 3️⃣
---

This sample demonstrates how to set up and run Binacle.Net with the UI Module Enabled.

## 🛠️ Prerequisites

Before you start, make sure you have the following installed on your machine.

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://www.docker.com/get-started) (included with Docker Desktop)

## Download the following files  

- [`docker-compose.yml`]({% vlink /samples/docker/ui-module-only/docker-compose.yml %}){:download="" target="_blank"}
- [`Presets.json`]({% vlink /samples/docker/ui-module-only/Presets.json %}){:download="" target="_blank"}  

Place these files in a directory of your choice. This directory will be your project root.

## Customize (Optional)

Edit the `Presets.json` file to adjust the bin configurations as per your needs.  

Create a `.env` file in the same directory with the content:
```text
 COMPOSE_PROJECT_NAME=binacle-net-ui-module-only
```  

This will set the project name for Docker Compose, allowing you to run multiple instances without conflicts.

## 🚀 Running the Application

In the project directory, start the application by running:
```bash
docker compose up
```

This will launch the Binacle.Net API with:
- 🌐 **Swagger UI**: http://localhost:8080/swagger/ for easy API exploration.
- 🖥️ **UI Module**: Accessible via http://localhost:8080/, providing an intuitive user interface for interacting with Binacle.Net.
- 📂 **Logs Folder**: A `./data/logs` folder will be created to store API logs for monitoring and debugging.


## 🔍 Accessing the API and UI
- **API Documentation (Swagger UI)**:  
  http://localhost:8080/swagger/  
  Use this to explore and test API endpoints directly.

- **UI Module (Packing Demo)**:  
  http://localhost:8080/  
  Interact with Binacle.Net through a user-friendly interface.

## ⚙️ Customizing Presets
To modify bin configurations:

1. Open the `Presets.json` file in your preferred editor.
2. Make your changes to the bin definitions.
3. Restart the application to apply the updates:
    ```bash
    docker compose down
    docker compose up
    ```

Your custom presets will now be active in the API.

## 📂 Logs Folder
When running the application, a `./data` folder will be created to store application data, including logs for monitoring and debugging. It's important to ensure that the `./data` and `./data/logs` directories have write permissions for proper functionality.

### Setting Permissions
Run the following commands to create the directory and set the required permissions:

```bash
mkdir -p ./data/logs
sudo chmod -R 777 ./data ./data/logs
```
This will grant full access to `./data` and its subdirectories.

> 777 gives full access to all users. Adjust permissions as needed for security.
{:.block-note}

Happy packing! 📦✨
 