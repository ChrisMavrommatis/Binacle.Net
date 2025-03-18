# Binacle.Net with All Features

This guide demonstrates how to set up and run Binacle.Net with all features enabled, including Azurite as the database provider for the Service Module, OpenTelemetry for monitoring, and Aspire Dashboard for observability.


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
   Edit the `Presets.json`, `JwtAuth.json`, `OpenTelemetry.Production.json`, and `aspire-dashboard-config.json` files to match your specific requirements.
   

## 🚀 Running the Application
In the project directory, start the application by running:

```bash
docker compose up
```
This will launch Binacle.Net with the following features:
- 🌐 **Swagger UI**: http://localhost:8080/swagger/ for easy API exploration.
- 💾 **Azurite Storage Emulator**: Simulates Azure Storage locally. Data is persisted in the ./azurite folder across container restarts.
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
- 🗂️**Custom Presets**:<br>
  Edit the Presets.json file to modify bin configurations. Changes will take effect after restarting the application:
  ```bash
  docker compose down
  docker compose up
  ```
- 🔐 **JWT Authentication**:<br>
  Adjust authentication settings in the JwtAuth.json file as needed.
- 📊 **OpenTelemetry**:<br>
  Customize telemetry configuration by editing OpenTelemetry.Production.json.
- 📈 **Aspire Dashboard**:<br>
  Adjust settings in aspire-dashboard-config.json to fine-tune the dashboard.

## 📂 Logs Folder
When running the application, a `./data` folder will be created to store application data, including logs for monitoring and debugging. It's important to ensure that the `./data` and `./data/logs` directories have write permissions for proper functionality.

### Setting Permissions
Run the following commands to create the directory and set the required permissions:

```bash
mkdir -p ./data/logs
sudo chmod -R 777 ./data ./data/logs
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
- [Binacle.Net Documentation](https://github.com/ChrisMavrommatis/Binacle.Net/wiki)
- [Docker Compose Reference](https://docs.docker.com/compose/)
- [Aspire Dashboard Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/overview?tabs=bash)
- [OpenTelemetry Documentation](https://opentelemetry.io/docs/)
 
Happy packing! 📦✨
