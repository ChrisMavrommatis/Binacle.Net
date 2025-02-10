# Binacle.Net with Custom Presets

This sample demonstrates how to set up and run Binacle.Net with custom presets using Docker Compose.

## 🛠️ Prerequisites
Before you start, make sure you have [Docker](https://www.docker.com) and [Docker Compose](https://docs.docker.com/compose/) installed on your machine.

## 📥 Getting Started

1. **Clone the Repository**

    Clone or download this repository to your local machine.
    ```bash
    git clone https://github.com/ChrisMavrommatis/Binacle.Net.git
    ```
    Alternatively, download the contents of this folder directly.

3. **Verify Files**
   
    Ensure the following files are present in the same directory:
    - `docker-compose.yml` – Docker Compose configuration for all services.
    - `Presets.json` – Your custom bin configurations.

4. Customize (Optional)
   
    Edit the `Presets.json` file to adjust the bin configurations as per your needs.

## 🚀 Running the Application
In the project directory, start the application by running:

```bash
docker compose up
```
This will launch the Binacle.Net API with:

- 🌐 **Swagger UI**: http://localhost:8080/swagger/ for easy API exploration.
- 📂 **Logs Folder**: A `./data/logs` folder will be created to store API logs for monitoring and debugging.

## 🔍 Accessing the API
Once the containers are running, open your browser and go to:

```bash
http://localhost:8080/swagger/
```
Here, you can explore and test the API endpoints with an intuitive interface.

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

## 📄 Additional Resources
- [Binacle.Net Documentation](https://github.com/ChrisMavrommatis/Binacle.Net/wiki)
- [Docker Compose Reference](https://docs.docker.com/compose/)
