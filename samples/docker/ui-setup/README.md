# Binacle.Net - UI Setup

This sample demonstrates how to set up and run Binacle.Net with the UI Module Enabled.

## 🛠️ Prerequisites
Before you start, make sure you have [Docker](https://www.docker.com) and 
[Docker Compose](https://docs.docker.com/compose/) installed on your machine.

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

3. **Customize (Optional)**<br>
   Edit the `Presets.json` file to adjust the bin configurations as per your needs.

## 🚀 Running the Application
In the project directory, start the application by running:
```bash
docker compose up
```
This will launch the Binacle.Net API with:
- 📖 **Custom Presets**: Loaded from your `Presets.json`.
- 🌐 **Swagger UI**: http://localhost:8080/swagger/ for easy API exploration.
- 🌐 **Scalar UI**: http://localhost:8080/scalar/ as an alternative to Swagger API.
- 🖥️ **UI Module**: Accessible via http://localhost:8080/, providing an intuitive user interface for interacting with Binacle.Net.
- 📂 **Logs Folder**: A `./data/logs` folder will be created to store API logs for monitoring and debugging.

## ⚙️ Customizing Presets
To modify bin configurations:
1. Open the `Presets.json` file in your preferred editor.
2. Make your changes to the bin definitions.
3. Restart the application to apply the updates:<br>
    ```bash
    docker compose down
    docker compose up
    ```
Your custom presets will now be active in the API.

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

## ⚙️ Customizing Presets
To modify bin configurations:
1. Open the `Presets.json` file in your preferred editor.
2. Make your changes to the bin definitions.
3. Restart the application to apply the updates:<br>
    ```bash
    docker compose down
    docker compose up
    ```

## 📂 Logs Folder
When running the application, a `./data` folder will be created to store application data, including logs for monitoring and debugging.
It's important to ensure that the `./data` and `./data/logs` directories have write permissions for proper functionality.

### Setting Permissions
Run the following commands to create the directory and set the required permissions:

```bash
mkdir -p ./data/logs
sudo chmod -R 777 ./data
```
This will grant full access to `./data` and its subdirectories.

> [!Note]
> 777 gives full access to all users. Adjust permissions as needed for security.

## 📄 Additional Resources
- [Binacle.Net Documentation](https://docs.binacle.net/)
- [Docker Compose Reference](https://docs.docker.com/compose/)

Happy packing! 📦✨
