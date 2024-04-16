# Binacle.NET

## Overview
Binacle.NET an API designed to address the 3D bin packing problem in a bin selection variation.

It is designed to be integrated into websites that wish to ship parcels for orders that will end up in self-served locker systems until the customer retrieves them from the locker, usually with a code sent to their phone.

The API will, given a set of bins, attempt to find which one of the bins, if any, will be able to accomodate all of the items.

A website wishing to provide the locker shipping option will most commonly want to offer their customer this option before the customer places the order, usually in cart or during checkout.

Binacle.Net was designed for the above usecase and utilizes a heuristic algorithm to provide a fast answer to the above problem, so the customer will not have to wait.


### Algorithm

Binacle.Net utilizes a hybrid variant of the First Fit Decreasing algorithm to address the 3D bin packing problem in a bin selection variation.

Numerous heuristics have been devised to tackle the bin packing problem, with the First Fit Decreasing algorithm emerging as one of the most popular and efficient solutions.

Although not guaranteed to yield a 100% accuracy rate, the algorithm has been fine-tuned to ensure that when a suitable bin is identified, it will invariably accommodate all items. 
However, it's worth noting that due to its heuristic nature, there are instances where items might feasibly fit within a bin, yet the algorithm may either allocate them to a larger bin or fail to identify a suitable one.


# Getting Started

Follow the steps bellow to get Binacle.Net up and running with Docker.

1. **Install Docker:** If you haven't already, [Install Docker](https://www.docker.com/get-started/) on your system.
2. **Pull Binacle.Net Docker Image**
   
   Run the following command in your terminal to pull the latest Binacle.NET Docker image from Docker Hub:
   ```bash
   docker pull chrismavrommatis/binacle-net:latest
   ```
3. **Run Binacle.NET Container**
  
   Once the image is pulled successfully, run the following command to start a Binacle.NET container:
   ```bash
   docker run -d --name binacle-net -p 8080:8080 chrismavrommatis/binacle-net:latest
   ```
   Or run the following command to start a Binacle.NET with Swagger
   ```bash
   docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True chrismavrommatis/binacle-net:latest
   ```
4. **Access Binacle.Net**
   
   You can now send requests to its API. By default, Binacle.NET runs on port 8080.
	 ```bash
	 https://localhost:8080/
	 ```
   Alternatively if you have run Binacle.Net with Swagger enabled you could access Swagger in you browser.
	 ```bash
	 https://localhost:8080/swagger/
	 ```

# Customizing Binacle.Net presets

Binacle.Net ships with specific bin [Presets](https://github.com/ChrisMavrommatis/Binacle.Net/blob/main/Api/Binacle.Net.Api/Config_Files/Presets.json), but you can change the to suit your business.
Although you can send the bins with the items using the Query by Custom endpoint, its best avoided in production, plus you save a bit on the bandwidth.

To change the presets grab the [Presets.json](https://github.com/ChrisMavrommatis/Binacle.Net/blob/main/Api/Binacle.Net.Api/Config_Files/Presets.json) file and change it how you want.
While Binacle.Net assumes centimeters will be used there is nothing stopping you from using a different system, assuming the dimensions are integers and the same unit for the bins and the items.

Use a bind volume mount to overwrite the Presets.json located in `/app/Config_Files/Presets.json` with your own.

You can either directly run it from the command line or use a Dockerfile which is the suggested way.

## Running from the command line
Place the new Presets.json file anywhere on your system and from that location run the following command depending on your system and preference.

Since we are using a volume to bind mount a single file we must provide the full path when running from a command line

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

## Using a Dockerfile
Place the new Presets.json file anywhere in your system and in that folder create a Dockerfile using the template below as a guide.

If you don't know what a Dockerfile is then create a file called `Dockerfile` with the template below.

Then from that location run the following command on your terminal
```bash
docker compose up
```

### Dockerfile Template

Using the long syntax in the Dockerfile allows us to specify a relative path as opposed to an absolute one.

```yaml
version: '2'
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


