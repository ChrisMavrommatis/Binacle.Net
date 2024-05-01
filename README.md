# Binacle.NET

## Overview
Binacle.NET is an API created to address the 3D Bin Packing Problem in a Bin Selection Variation.

It is designed to be integrated into e-commerce platforms that offer parcel shipment to self-service locker systems. These lockers securely store the parcels until customers retrieve them using a unique code, typically sent to their mobile phones.

Upon receiving a set of bins and items, Binacle.NET swiftly determines the optimal bin, if any, capable of accommodating all items efficiently.

Websites offering the locker shipping option, will most commonly want to present this option to their customer before they place the order, this is typically done in cart or during checkout which are critical stages during the customer's purchase journey.

Binacle.NET caters precisely to this use case, employing a heuristic algorithm to swiftly address the problem, ensuring minimal wait times for customers.

E-commerce platforms that wish to provide a locker shipping option usually either base their packaging on the dimensions of the end locker container or utilize their own pre-defined boxes tailored to fit the lockers.

## Algorithm
Binacle.Net utilizes a hybrid variant of the First Fit Decreasing algorithm to address the 3D Bin Packing Problem in a Bin Selection Variation.

Numerous heuristics have been devised to tackle the Bin Packing Problem, with the First Fit Decreasing algorithm emerging as one of the most popular and efficient solutions.

Although not guaranteed to yield a 100% accuracy rate, the algorithm has been fine-tuned to ensure that when a suitable bin is identified, it will invariably accommodate all items. 
However, it's worth noting that due to its heuristic nature, there are instances where items might feasibly fit within a bin, yet the algorithm may either allocate them to a larger bin or fail to identify a suitable one.


## Get Started

Make sure you have [Docker](https://www.docker.com/get-started/) installed on your system.

Then run the following command in your terminal to pull and run the latest Binacle.NET Docker image from Docker Hub:
```bash
docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True chrismavrommatis/binacle-net:latest
```
Then you can access the Swagger UI by visiting the following URL in your browser

```
https://localhost:8080/swagger/
```


## About the API
Binacle.Net at its core offers the following 3 endpoints.

1. **Presets**: This endpoint lists all available presets, which are pre-defined collections of bins. 
2. **Query by Custom**: With this endpoint, you can send both the bins and the items in a single request. The endpoint will respond with the bin that accommodates all of the items, if such a bin exists. 
3. **Query by Preset**: This endpoint allows you to send only the items and the preset key. In response, the endpoint will provide the bin from the specified preset that accommodates all of the items, if possible.

While you have the option to send both the bins and the items together using the **Query by Custom** endpoint, it's recommended to avoid this approach if you already know the bins beforehand. Instead, consider using the **Query by Preset** endpoint for better efficiency.

Binacle.Net includes specific bin [Presets](https://github.com/ChrisMavrommatis/Binacle.Net/blob/main/Api/Binacle.Net.Api/Config_Files/Presets.json) by default, which you can customize to align with your business requirements.

### Service Module
In addition to the core functionality of Binacle.Net, there's a Service Module available, though it's disabled by default.

This Module expands and customizes the core Binacle.Net functionality for use in a public environment.

It introduces several features:

- **Healthcheck**: Allows automated systems to verify the API's status.
- **Rate Limiting**: Implements a global limit of 10 requests per 60 seconds on all endpoints, irrespective of user. This measure is primarily to prevent potential flooding of the public service. 
- **Authentication**: Grants certain privileged users the ability to authenticate and bypass the rate limiter.
- **User Management**: Designed for service administrators.
- **Cloud Logging**: Facilitates log storage outside the image without requiring volume mounts.
- **Telemetry**: Provides monitoring capabilities for the application.

It's essential to understand that these features are specifically designed for the hosting environment envisioned for Binacle.Net's deployment. Therefore, the Service Module is heavily opinionated and primarily intended for use by the creator, rather than general users. 

Furthermore, it's been engineered to ensure that any modifications to the Service Module do not affect users who are not utilizing it, as evidenced by its **disabled by default** status.

Binacle.Net is freely available as an open-source project. With minimal effort, you can deploy your instance in your environment.


## Running the Container with a Custom Internal Port
By default, Binacle.NET runs on port 8080.

If you prefer to run the application with a different port inside the container, you can specify the desired port by passing the `ASPNETCORE_HTTP_PORTS` environment variable.

In the following example we're specifying port 80 as the internal port within the container.
```bash
docker run --name binacle-net -e ASPNETCORE_HTTP_PORTS=80 -p 8080:80 chrismavrommatis/binacle-net:latest
```

## Customizing the Presets
To adjust the presets, download the [Presets.json](https://github.com/ChrisMavrommatis/Binacle.Net/blob/main/Api/Binacle.Net.Api/Config_Files/Presets.json) file and modify it according to your needs. 
Although Binacle.Net assumes the usage of centimeters, you can utilize different measurement systems as long as the dimensions remain integers and consistent across bins and items.

Use a bind volume mount to replace the default Presets.json located in `/app/Config_Files/Presets.json` with your customized version.

You can either directly run Binacle.Net from the command line or use a Docker Compose file which is the suggested way.

### Command line using Docker
Place the updated Presets.json file anywhere on your system and execute the appropriate command based on your operating system and preferences.

*Note: Since we are using a volume to bind mount a single file we must provide the full path when running from a command line.*

- For Linux & MacOS (without Swagger)
  ```bash
  docker run --name binacle-net -p 8080:8080 -v $(pwd)/Presets.json:/app/Config_Files/Presets.json:ro chrismavrommatis/binacle-net:latest
  ```
- For Linux & MacOS (with Swagger)
  ```bash
  docker run --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -v $(pwd)/Presets.json:/app/Config_Files/Presets.json:ro chrismavrommatis/binacle-net:latest
  ```
- For Windows (without Swagger)
  ```bat 
  docker run --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -v %cd%/Presets.json:/app/Config_Files/Presets.json:ro chrismavrommatis/binacle-net:latest
  ```
- For Windows (with Swagger)
  ```bat
  docker run --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -v %cd%/Presets.json:/app/Config_Files/Presets.json:ro chrismavrommatis/binacle-net:latest
  ```

### Using the Docker Compose file
Place the modified Presets.json file in the same directory as your Docker Compose file.
If you're unfamiliar with Docker Compose files, create a file named `compose.yaml` using the provided template below.

You can then, execute the following command from the directory containing both the Docker Compose file and Presets.json:

```bash
docker compose up
```

#### Docker Compose file template

*Note: Using the long syntax in the Docker Compose file allows us to specify a relative path as opposed to an absolute one.*

```yaml
services:
  binacle-net:
    image: chrismavrommatis/binacle-net:latest
    ports:
      - "8080:8080"
    volumes:
      - type: bind
        source: ./Presets.json
        target: /app/Config_Files/Presets.json
        read_only: true
    environment:
      - SWAGGER_UI=True
```

## Links

- [🐳 Binacle.Net on Dockerhub](https://hub.docker.com/r/chrismavrommatis/binacle-net)
- [Postman Collection](https://www.postman.com/chrismavrommatis/workspace/binacle-net/)

  

