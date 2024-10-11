# Binacle.Net with All Features

This sample demonstrates how to set up and run Binacle.Net with all features enabled, including Azurite as the database provider for the Service Module.

## Prerequisites
Before you begin, ensure that [Docker](https://www.docker.com) is installed on your machine.

## Getting Started
Clone or download this repository to your local machine.

Alternatively, you can just download the contents of this folder. Ensure all the following files are present in the same directory:
- `docker-compose.yml` - The docker compose file that includes all configuration for the services.
- `Presets.json` – Custom bin configurations.
- `JwtAuth.json` – JWT authentication configuration.

Modify `Presets.json`, `JwtAuth.json` files as needed to match your requirements.

## Running the Application
To start the application, open a terminal in the directory where the sample is located and run:

```bash
docker compose up
```

This will start the Binacle.Net API with:

- **Swagger UI**: Available at http://localhost:8080/swagger/index.html for easy API exploration and testing.
- **Azurite Storage Emulator**: Simulates Azure Storage locally. Azurite will create a `./azurite` folder to persist data across container restarts. It handles the storage operations for the Service Module.
- **Service Module**: Configured to use Azurite as its database provider, enabling local storage operations for the API.
- **UI Module**: Accessible via `http://localhost:8080/`, providing a user interface for interacting with Binacle.Net.
- **Logs Folder**: A `./data` folder will be created, containing the `logs` folder where log files from Binacle.Net will be stored.

## Customizing the Configuration
- **Custom Presets**: To adjust bin configurations, edit the `Presets.json` file. Changes will be applied upon restarting the application.
- **JWT Authentication**: You can modify the authentication settings by editing the `JwtAuth.json` file.

## Conclusion
By following these steps, you can easily run Binacle.Net with all its features, including local storage with Azurite, API interaction via Swagger UI, and logging capabilities.
 
