---
title: Presets
nav:
  parent: Core
  order: 1
  icon: 📖
---

Using presets in Binacle.Net can significantly streamline your packing process, 
especially when bin configurations are consistent.

Instead of including bin definitions in every API request, 
you can define them once in the preset file and reuse them across multiple requests.

> While Binacle.Net assumes the use of centimeters, you can use any measurement system as 
> long as the dimensions are integers and consistent across bins and items.
{: .block-caution}

## 🛠️ Configuration
Presets are stored in the `Presets.json` file.

This file contains predefined bin configurations that can be used directly in your packing operations.

**Default configuration:**
```json
{
  "PresetOptions": {
    "Presets": {
      "rectangular-cuboids": {
        "Bins": [
          { "ID": "Small", "Length": 60, "Width": 40, "Height": 10 },
          { "ID": "Medium", "Length": 60, "Width": 40, "Height": 20 },
          { "ID": "Large", "Length": 60, "Width": 40, "Height": 30 }
        ]
      },
      "perfect-cubes": {
        "Bins": [
          { "ID": "Small", "Length": 10, "Width": 10, "Height": 10 },
          { "ID": "Medium", "Length": 20, "Width": 20, "Height": 20 },
          { "ID": "Large", "Length": 30, "Width": 30, "Height": 30 }
        ]
      },
      "sample": {
        "Bins": [
          { "ID": "Small", "Length": 62, "Width": 45, "Height": 8 },
          { "ID": "Medium", "Length": 62, "Width": 45, "Height": 17 },
          { "ID": "Large", "Length": 62, "Width": 45, "Height": 36 }
        ]
      }
    }
  }
}
```

You can modify the presets using **Production Overrides** by creating a `Presets.Production.json` file.

For more information on this refer to the [Configuration]({% vlink /configuration/index.md %}#%EF%B8%8F-overriding-configuration) page.

However, unlike other configurations, presets are rarely modified incrementally. 
In most cases, it’s best to override the entire file instead of making selective changes.

- 📁 **Location**: `/app/Config_Files`
- 📌 **Full Path**: `/app/Config_Files/Presets.json`

---

## 🔄 Overriding Presets
There are multiple ways to override the default `Presets.json` file, particularly when using Docker. 

Below are two methods for overriding the presets file in a Docker environment:

### 🖥️ Option 1: Command Line
1. Download the [Presets.json]({% vlink /downloads/Presets.json %}){:download="" target="_blank"} 
  file and modify it according to your needs.
2. Place the modified `Presets.json` file anywhere on your system.
3. Open your terminal and navigate to the directory containing your modified Presets.json file.
4. Run the following Docker command to mount the updated presets file:

```bash
docker run --name binacle-net \
  -p 8080:8080 \
  -e SWAGGER_UI=True \
  -v $(pwd)/Presets.json:/app/Config_Files/Presets.json:ro \
  binacle/binacle-net:{{ page.version_tag }}
```

> If you are running under **Windows** replace `$(pwd)` with `%cd%` and `\` with `^`, so the command looks like this:
{: .block-tip}

```cmd
docker run --name binacle-net ^
  -p 8080:8080 ^
  -e SWAGGER_UI=True ^
  -v %cd%\Presets.json:/app/Config_Files/Presets.json:ro ^
  binacle/binacle-net:{{ page.version_tag }}
```

> Since we are using a bind mount for a single file, the full path must be provided.
{: .block-note}

### 📝 Option 2: Docker Compose
If you prefer to use Docker Compose, follow these steps:

1. Create a Docker Compose file (compose.yaml) if you don’t already have one.
2. Download the [Presets.json]({% vlink /downloads/Presets.json %}){:download="" target="_blank"} 
  file and modify it according to your needs.
3. Place the modified Presets.json file in the same directory as your compose.yaml.
4. Navigate to the directory containing both the Presets.json and compose.yaml files.
5. Run the following command:
```bash
docker compose up
```

**Docker Compose File Template**

> Using the long syntax in the Docker Compose file allows you to specify a relative path rather than an absolute one.
{: .block-note}

<br>

```yml
services:
  binacle-net:
    image: binacle/binacle-net:{{ page.version_tag }}
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

