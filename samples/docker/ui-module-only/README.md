# Binacle.Net with UI Module

This sample demonstrates how to set up and run Binacle.Net with the UI Module Enabled.

## 🛠️ Prerequisites
Before you start, make sure you have [Docker](https://www.docker.com) and [Docker Compose](https://docs.docker.com/compose/) installed on your machine.

## 📥 Getting Started

1. **Clone the Repository**<br>
   Clone or download this repository to your local machine.
   ```bash
   git clone https://github.com/ChrisMavrommatis/Binacle.Net.git
   ```
   Alternatively, download the contents of this folder directly.

3. **Verify Files**<br>
   Ensure the following files are present in the same directory:
   - `docker-compose.yml` – Docker Compose configuration for all services.
   - `Presets.json` – Your custom bin configurations.

4. **Customize (Optional)**<br>
   Edit the `Presets.json` file to adjust the bin configurations as per your needs.

## 🚀 Running the Application
In the project directory, start the application by running:
```bash
docker compose up
```
This will launch the Binacle.Net API with:
- 🌐 **Swagger UI**: http://localhost:8080/swagger/ for easy API exploration.
- 🖥️ **UI Module**: Accessible via http://localhost:8080/, providing an intuitive user interface for interacting with Binacle.Net.
- 📂 **Logs Folder**: A `./data/logs` folder will be created to store API logs for monitoring and debugging.

## Customizing the Configuration
- **Custom Presets**: To adjust bin configurations, edit the `Presets.json` file. Changes will be applied upon restarting the application.

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

## 📂 Logs Folder
When running the application, a `./data` folder will be created to store application data, including logs for monitoring and debugging. It's important to ensure that the `./data` and `./data/logs` directories have write permissions for proper functionality.

### Setting Permissions
Run the following commands to create the directory and set the required permissions:

```bash
mkdir -p ./data
sudo chmod -R 777 ./data
```
This will grant full access to `./data` and its subdirectories.

> [!Note]
> 777 gives full access to all users. Adjust permissions as needed for security.

## 📄 Additional Resources
- [Binacle.Net Documentation](https://github.com/ChrisMavrommatis/Binacle.Net/wiki)
- [Docker Compose Reference](https://docs.docker.com/compose/)

Happy packing! 📦✨
