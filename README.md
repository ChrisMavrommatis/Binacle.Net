# Binacle.NET


## Overview
Binacle.NET is an API created to address the 3D Bin Packing Problem in a Bin Selection Variation.

It is designed to be integrated into e-commerce platforms that offer parcel shipment to self-service locker systems. These lockers securely store the parcels until customers retrieve them using a unique code, typically sent to their mobile phones.

Upon receiving a set of bins and items, Binacle.NET swiftly determines the optimal bin, if any, capable of accommodating all items efficiently.

Websites offering the locker shipping option, will most commonly want to present this option to their customer before the they place the order, this is typically done in cart or during checkout which are critical stages during the customer's purchase journey.

Binacle.NET caters precisely to this use case, employing a heuristic algorithm to swiftly address the problem, ensuring minimal wait times for customers.

E-commerce platforms that wish to provide a locker shipping option usually either base their packaging on the dimensions of the end locker container or utilize their own pre-defined boxes tailored to fit the lockers.

## Algorithm
Binacle.Net utilizes a hybrid variant of the First Fit Decreasing algorithm to address the 3D Bin Packing Problem in a Bin Selection Variation.

Numerous heuristics have been devised to tackle the Bin Packing Problem, with the First Fit Decreasing algorithm emerging as one of the most popular and efficient solutions.

Although not guaranteed to yield a 100% accuracy rate, the algorithm has been fine-tuned to ensure that when a suitable bin is identified, it will invariably accommodate all items. 
However, it's worth noting that due to its heuristic nature, there are instances where items might feasibly fit within a bin, yet the algorithm may either allocate them to a larger bin or fail to identify a suitable one.


## Getting Started
Follow the steps below to get Binacle.Net up and running with Docker.

1. **Install Docker:** If you haven't already, [Install Docker](https://www.docker.com/get-started/) on your system.
   
3. **Pull Binacle.Net Docker Image**
   
   Run the following command in your terminal to pull the latest Binacle.NET Docker image from Docker Hub:
   ```bash
   docker pull chrismavrommatis/binacle-net:latest
   ```
   
5. **Run Binacle.NET Container**
   
   Once the image is pulled successfully, run the following command to start a Binacle.NET container:
   ```bash
   docker run -d --name binacle-net -p 8080:8080 chrismavrommatis/binacle-net:latest
   ```
   Or run the following command to start Binacle.NET with Swagger UI:
   ```bash
   docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True chrismavrommatis/binacle-net:latest
   ```
   
7. **Access Binacle.Net**
  
   You can now send requests to the API. By default, Binacle.NET runs on port 8080.
	 ```
	 https://localhost:8080/
	 ```
   Alternatively if you run Binacle.Net with Swagger enabled you could access the Swagger UI in your browser.
	 ```
	 https://localhost:8080/swagger/
	 ```


## About the API
Binacle.Net offers 3 endpoints

1. **Presets**: This endpoint lists all available presets, which are pre-defined collections of bins. 
2. **Query by Custom**: With this endpoint, you can send both the bins and the items in a single request. The endpoint will respond with the bin that accommodates all of the items, if such a bin exists. 
3. **Query by Preset**: This endpoint allows you to send only the items and the preset key. In response, the endpoint will provide the bin from the specified preset that accommodates all of the items, if possible.

While you have the option to send both the bins and the items together using the **Query by Custom** endpoint, it's recommended to avoid this approach if you already know the bins beforehand. Instead, consider using the **Query by Preset** endpoint for better efficiency.

Binacle.Net includes specific bin [Presets](https://github.com/ChrisMavrommatis/Binacle.Net/blob/main/Api/Binacle.Net.Api/Config_Files/Presets.json) by default, which you can customize to align with your business requirements.


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
  ```bash
  docker run --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -v %cd%/Presets.json:/app/Config_Files/Presets.json:ro chrismavrommatis/binacle-net:latest
  ```
- For Windows (with Swagger)
  ```bash
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
