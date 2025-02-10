# Binacle.Net with All Features

This sample demonstrates how to set up and run Binacle.Net with all features enabled, including Azurite as the database provider for the Service Module.


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

3. **Customize (Optional)**<br>
   Edit the `Presets.json` and `JwtAuth.json` files to match your specific requirements.
   

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
- 📂 **Logs Folder**: A `./data/logs` folder will be created to store application logs for monitoring and debugging.


## 🔍 Accessing the API and UI
- **API Documentation (Swagger UI)**:<br>
  http://localhost:8080/swagger/ <br>
  Use this to explore and test API endpoints directly.

- **UI Module (Packing Demo)**:<br>
  http://localhost:8080/<br>
  Interact with Binacle.Net through a user-friendly interface.

## ⚙️Customizing the Configuration
- 🗂️**Custom Presets**:<br>
  Edit the Presets.json file to modify bin configurations. Changes will take effect after restarting the application:
  ```bash
  docker compose down
  docker compose up
  ```
- 🔐 **JWT Authentication**:<br>
  Adjust authentication settings in the JwtAuth.json file as needed.

## ✅ Conclusion
By following these steps, you can effortlessly run Binacle.Net with all features enabled, including:
- Local storage with Azurite
- API interaction via Swagger UI
- A dynamic UI Module for demonstrations
- Comprehensive logging for monitoring and troubleshooting

## 📄 Additional Resources
- [Binacle.Net Documentation](https://github.com/ChrisMavrommatis/Binacle.Net/wiki)
- [Docker Compose Reference](https://docs.docker.com/compose/)
 
