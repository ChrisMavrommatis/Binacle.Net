# Docker Compose Samples
This folder provides sample configurations to run Binacle.Net with Docker Compose, showcasing different features and modules.
You can choose the setup that fits your needs, from basic API functionality to full feature sets with UI and Service Modules.

## 📦 Available Samples

### 1️⃣ Core with Custom Presets
This sample demonstrates how to set up and run Binacle.Net with custom bin packing presets.
- Directory: `samples/CoreWithPresets`
- Key Features:
  - Customizable bin configurations via Presets.json
  - Lightweight API setup
 
### 2️⃣ With All Features Enabled
Run Binacle.Net with all features enabled.
- Directory: `samples/WithAllFeatures`
- Key Features:
  - Full-featured Binacle.Net setup
  - UI Module for interactive demos
  - Service Module with Azurite as a local storage provider
  - Customizable bin configurations via Presets.json

### 3️⃣ With UI Module Only
This sample focuses on running Binacle.Net with just the UI Module enabled, ideal for interactive demos and quick visual testing.

- Directory: `samples/WithUIModuleOnly`
- Key Features:
  - UI Module for interactive demos
  - Customizable bin configurations via Presets.json


## ⚙️ Prerequisites
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://www.docker.com/get-started) (included with Docker Desktop)


## 🚀 Getting Started

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/ChrisMavrommatis/Binacle.Net.git
cd Binacle.Net/samples
```

### 2️⃣ Run a Sample

#### Core with Custom Presets
```bash
cd CoreWithPresets
docker compose up -d
```

#### With All Features Enabled
```bash
cd WithAllFeatures
docker compose up -d
```

#### With UI Module Only
```bash
cd WithUIModuleOnly
docker compose up -d
```

## 🌐 Access the Application
- Swagger UI (API Explorer): http://localhost:8080/swagger/
- UI Module (if enabled): http://localhost:8080/

## 🔧 Customization
- **Presets**: Modify `Presets.json` to adjust bin packing configurations.
- **Authentication**: Configure `JwtAuth.json` in the "All Features" sample for accessing the API as an admin or a user with no rate limiter.
- **Storage**: The "All Features" sample uses Azurite for local storage. Data persists in the `./azurite` folder.

## 🗑️ Stopping and Cleaning Up
To stop the containers:
```bash
docker compose down
```

To remove all data (including volumes):
```bash
docker compose down -v
```
