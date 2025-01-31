# Binacle.Net with UI Module

This sample demonstrates how to set up and run Binacle.Net with the UI Module Enabled.

## Prerequisites
Before you begin, ensure that [Docker](https://www.docker.com) is installed on your machine.

## Getting Started
Clone or download this repository to your local machine.

Alternatively, you can just download the contents of this folder. Ensure all the following files are present in the same directory:
- `docker-compose.yml` - The docker compose file that includes all configuration for the services.
- `Presets.json` – Custom bin configurations.

If needed, modify the `Presets.json` file to fit your specific bin configurations.

## Running the Application
To start the application, open a terminal in the directory where the sample is located and run:

```bash
docker compose up
```

This will start the Binacle.Net API with:

- **Swagger UI**: Available at http://localhost:8080/swagger/index.html for easy API exploration and testing.
- **UI Module**: Accessible via `http://localhost:8080/`, providing a user interface for interacting with Binacle.Net.
- **Logs Folder**: A `./data` folder will be created, containing the `logs` folder where log files from Binacle.Net will be stored.

## Customizing the Configuration
- **Custom Presets**: To adjust bin configurations, edit the `Presets.json` file. Changes will be applied upon restarting the application.


## Accessing the API and UI
Once the containers are running, you can access the Swagger UI in your browser at:

```bash
http://localhost:8080/swagger/index.html
```

The Swagger UI allows you to explore and test the API endpoints directly.

To access the UI Module, navigate to:

```bash
http://localhost:8080/
```

