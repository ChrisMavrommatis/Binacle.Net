---
title: Quickstart
permalink: /version/v3.0.x/samples/docker/quickstart/
nav:
  order: 2
  parent: Docker
  icon: 2️⃣
---

Binacle.Net with everything you need to look at it: Swagger UI, Scalar UI and the web UI demo. The setup to
take when you are trying it out for the first time.

## 🛠️ Prerequisites

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://www.docker.com/get-started) (included with Docker Desktop)

## 📥 Download the following files
- [`docker-compose.yml`]({% vlink /samples/docker/quickstart/docker-compose.yml %}){:download="" target="_blank"}
- [`Presets.json`]({% vlink /samples/docker/quickstart/Presets.json %}){:download="" target="_blank"}

Place both in a directory of your choice. That directory is your project root.

## ✏️ Customize (optional)
Edit `Presets.json` to use your own bins. The shipped file is enough to try things out with.

## 🚀 Running the Application
In the project root:
```bash
docker compose up
```

This launches the Binacle.Net API with:
- 📖 **Custom Presets**: loaded from your `Presets.json`.
- 🌐 **Swagger UI**: `http://localhost:8080/swagger/`
- 🌐 **Scalar UI**: `http://localhost:8080/scalar/`
- 🖥️ **UI Module**: `http://localhost:8080/` - the packing demo and the ViPaq protocol decoder.
- 📂 **Logs Folder**: a `./data/logs` folder is created for application logs.

## 🔍 What to look at

- **Swagger UI** - `http://localhost:8080/swagger/`, to explore and call the endpoints.
- **Scalar UI** - `http://localhost:8080/scalar/`, an alternative to Swagger.
- **UI Module** - `http://localhost:8080/`, the visual packing demo.

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
{:.block-note}

> This setup is for looking at Binacle.Net, not for running it. For a real deployment see
> [Prod]({% vlink /samples/docker/prod/index.md %}), or [Service]({% vlink /samples/docker/service/index.md %})
> if you need accounts.
{:.block-note}

## 📄 Additional Resources
- [Docker Compose Reference](https://docs.docker.com/compose/)

Happy packing! 📦✨
