# Binacle.Net with Custom Presets

This sample demonstrates how to set up and run Binacle.Net with custom presets using Docker Compose.

## Prerequisites
Before you start, make sure you have the [docker](https://www.docker.com) installed on your machine.

## Getting Started
Clone or download this repository to your local machine.
Alternatively, you can just download the contents of this folder. Make sure that both `docker-compose.yml` and `Presets.json` files are in the same directory.
If needed, modify the `Presets.json` file to fit your specific bin configurations.


## Running the Application
To launch the application, open a terminal in the directory where the sample is located and run:

```bash
docker compose up
```

This will start the Binacle.Net API, including the Swagger UI for easy interaction with the API.

### Accessing the API
Once the containers are running, you can access the Swagger UI in your browser at:

```bash
http://localhost:8080/swagger/index.html
```

The Swagger UI allows you to explore and test the API endpoints directly.

## Customizing Presets
If you want to customize the bin configurations, simply edit the `Presets.json` file in the project folder. The updated configurations will be automatically used by the API when it is restarted.
