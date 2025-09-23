# Binacle.Net - As a Service using Azure services

This Docker setup runs Binacle.Net as a Service (with Service Module enabled),
Azure services as the database backend and the UI Module enabled for a user-friendly interface.

## 🛠️ Prerequisites
Before you start, make sure you have [Docker](https://www.docker.com) and [Docker Compose](https://docs.docker.com/compose/) installed on your machine.


## 📥 Getting Started

1. **Clone the Repository**<br>
   Clone or download this repository to your local machine.
   ```bash
   git clone https://github.com/ChrisMavrommatis/Binacle.Net.git
   ```
   Alternatively, download the contents of this folder directly.

2. **Verify Files**<br>
   Ensure the following files are present in the same directory:
    - `docker-compose.yml` – Docker Compose configuration for all services.
    - `Presets.json` – Your custom bin configurations.
    - `JwtAuth.json` – JWT authentication configuration.
    - `OpenTelemetry.Production.json` – OpenTelemetry configuration for monitoring.
    - `aspire-dashboard-config.json` – Configuration for the Aspire Dashboard.

3. **Customize (Optional)**<br>
   Edit the `Presets.json`, `JwtAuth.json`, 
   `OpenTelemetry.Production.json`, and `aspire-dashboard-config.json` files to match your specific requirements.

## 🚀 Running the Application
In the project directory, start the application by running:
```bash
docker compose up
```

This will launch the following features:
- 📖 **Custom Presets**: Loaded from your `Presets.json`.
- 🌐 **Swagger UI**: http://localhost:8080/swagger/ for easy API exploration.
- 🌐 **Scalar UI**: http://localhost:8080/scalar/ as an alternative to Swagger API.
- 🖥️ **UI Module**: Accessible via http://localhost:8080/, providing an intuitive user interface for interacting with Binacle.Net.
- 📂 **Logs Folder**: A `./data/logs` folder will be created to store API logs for monitoring and debugging.
- ✅ **Health Checks**: Available at http://localhost:8080/_health to monitor service status.
- 📦 **Packing Logs**: Stored in the `./data/pack-logs` directory for analytics.
- ⚙️ **Service Module**: With Azure services as the database backend. </br>
  default admin credentials `admin@binacle.net:B1n4cl3Adm!n` 
- 🐘 **Azurite Storage Emulator**: Simulates Azure Storage locally. Data is persisted in the `./azurite` folder across container restarts.
- 📊 **OpenTelemetry**: Collects telemetry data for monitoring and exports it to the **Aspire Dashboard**.
- 📈 **Aspire Dashboard**: Accessible via http://localhost:18888 for real-time metrics and performance monitoring.


## 🔍 Accessing the API and UI
- **API Documentation (Swagger UI)**:<br>
  http://localhost:8080/swagger/ <br>
  Use this to explore and test API endpoints directly.

- **Alternative API Documentation (Scalar UI)**:<br>
  http://localhost:8080/scalar/ <br>
  Use this as an alternative method for exploring and testing API endpoints.

- **UI Module (Packing Demo)**:<br>
  http://localhost:8080/<br>
  Interact with Binacle.Net through a user-friendly interface.

- **Aspire Dashboard**:<br>
  http://localhost:18888<br>
  Monitor real-time metrics and performance of Binacle.Net services.


## ⚙️Customizing the Configuration
- 🗂️**Custom Presets**:<br>
  Edit the `Presets.json` file to modify bin configurations. Changes will take effect after restarting the application:
  ```bash
  docker compose down
  docker compose up
  ```
- 🔐 **JWT Authentication**:<br>
  Adjust authentication settings in the `JwtAuth.json` file as needed.
- 📊 **OpenTelemetry**:<br>
  Customize telemetry configuration by editing `OpenTelemetry.Production.json`.
- 📈 **Aspire Dashboard**:<br>
  Adjust settings in `aspire-dashboard-config.json` to fine-tune the dashboard.


## 📂 Logs Folder
When the application runs, it automatically creates a `./data` directory to store all persistent data, 
including log files used for monitoring and debugging.

To ensure proper functionality, the following directories must exist and have write permissions:
- `./data`
- `./data/logs`
- `./data/pack-logs/fitting`
- `./data/pack-logs/packing`

### Setting Permissions
Run the following commands to create the directory and set the required permissions:

```bash
mkdir -p ./data/logs \
         ./data/pack-logs/fitting \
         ./data/pack-logs/packing

sudo chmod -R 777 ./data
```
This will grant full access to `./data` and its subdirectories.

> [!Note]
> 777 gives full access to all users. Adjust permissions as needed for security.

## 🛠️ Getting the Aspire Dashboard Login Token
The Aspire Dashboard generates a login URL with a token for access.

View the Logs: After starting the deployment, run the following command:
```bash
docker-compose logs -f aspire-dashboard
```
Locate the Login URL: In the logs, find a line like:
```pgsql
Login to the dashboard at http://localhost:18888/login?t=your_token_here.
```
Access the Dashboard: Copy the URL and open it in your browser. You’ll be logged in automatically

## 📄 Additional Resources
- [Binacle.Net Documentation](https://docs.binacle.net/)
- [Docker Compose Reference](https://docs.docker.com/compose/)
- [Aspire Dashboard Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/overview?tabs=bash)
- [OpenTelemetry Documentation](https://opentelemetry.io/docs/)

Happy packing! 📦✨