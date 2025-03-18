# Docker Compose Samples
This folder provides sample configurations to run Binacle.Net with Docker Compose, showcasing different features and modules. You can choose the setup that fits your needs, from basic API functionality to full-featured setups with UI and Service Modules.

## 📦 Available Samples

### 1️⃣ Minimal Setup
This sample demonstrates a minimal Docker Compose setup for Binacle.Net with essential features.
- Directory: `samples/docker/minimal-setup`
- Key Features:
  - Basic API functionality
  - Lightweight configuration for easy setup and testing

### 2️⃣ Full Deployment
Run Binacle.Net with all features enabled for a complete experience.
- Directory: `samples/docker/full-deployment`
- Key Features:
  - Full-featured Binacle.Net setup
  - UI Module for interactive demos
  - Service Module with Azurite as a local storage provider
  - Customizable bin configurations via Presets.json
  - OpenTelemetry integration for monitoring
  - Aspire Dashboard for observability

### 3️⃣ UI Module Only
A sample focused on running Binacle.Net with just the UI Module enabled, ideal for interactive demos or visual testing.
- Directory: `samples/docker/ui-module-only`
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

#### Minimal Setup
```bash
cd docker/minimal-setup
docker compose up -d
```

#### Full Deployment with All Features
```bash
cd docker/full-deployment
docker compose up -d
```

#### UI Module Only
```bash
cd docker/ui-module-only
docker compose up -d
```

## 🌐 Access the Application
- Swagger UI (API Explorer): http://localhost:8080/swagger/
- UI Module (if enabled): http://localhost:8080/
- Aspire Dashboard (Only In Full Deployment): http://localhost:18888

## 🔧 Customization
- **Presets**: Modify `Presets.json` to adjust bin packing configurations.
- **Authentication**: Configure `JwtAuth.json` in the "Full Deployment" sample for accessing the API as an admin or user.
- **Storage**: The "Full Deployment" sample uses Azurite for local storage. Data persists in the `./azurite` folder.
- **Telemetry and Observability**: The "Full Deployment" sample integrates OpenTelemetry for monitoring, sending telemetry data to the Aspire Dashboard.

## 🗑️ Stopping and Cleaning Up
To stop the containers:
```bash
docker compose down
```

To remove all data (including volumes):
```bash
docker compose down -v
```
