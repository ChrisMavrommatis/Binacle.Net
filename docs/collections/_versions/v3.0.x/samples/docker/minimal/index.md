---
title: Minimal
permalink: /version/v3.0.x/samples/docker/minimal/
nav:
  order: 1
  parent: Docker
  icon: 1️⃣
---

The smallest setup that answers: the API, your presets, and somewhere to write logs. Nothing else is switched
on.

## 🛠️ Prerequisites

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://www.docker.com/get-started) (included with Docker Desktop)

## 📥 Download the following files
- [`docker-compose.yml`]({% vlink /samples/docker/minimal/docker-compose.yml %}){:download="" target="_blank"}
- [`Presets.json`]({% vlink /samples/docker/minimal/Presets.json %}){:download="" target="_blank"}

Place both in a directory of your choice. That directory is your project root.

## ✏️ Change this first
Edit `Presets.json` and replace the example bins with your own. Until you do, the answers describe someone
else's packaging.

## 🚀 Running the Application

In the project root, start the application:
```bash
docker compose up
```

This launches the Binacle.Net API with:
- 📖 **Custom Presets**: loaded from your `Presets.json`.
- 📂 **Logs Folder**: a `./data/logs` folder is created for application logs.

## 🌐 Accessing the API
Once the container is running, the API answers on:
```bash
http://localhost:8080/
```

There is no browsable documentation in this setup. Use the [API]({% vlink /api/index.md %}) pages, or the
[Quickstart]({% vlink /samples/docker/quickstart/index.md %}) sample if you want Swagger UI.

## ⚙️ Customizing Presets
To change bin configurations:
1. Open `Presets.json` in your editor.
2. Edit the bin definitions.
3. Restart to apply:<br>
    ```bash
    docker compose down
    docker compose up
    ```

## 📂 Logs Folder
A `./data` folder is created for application data, including logs. The `./data` and `./data/logs` directories
need write permissions.

### Setting Permissions
```bash
mkdir -p ./data/logs
sudo chmod -R 777 ./data
```

> 777 gives full access to all users. Adjust permissions as needed for security.
{:.block-note }

## 📄 Additional Resources
- [Docker Compose Reference](https://docs.docker.com/compose/)

Happy packing! 📦✨
