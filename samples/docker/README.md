# Docker Compose Samples
This folder provides sample configurations to run Binacle.Net with Docker Compose, showcasing different features and modules.
You can choose the setup that fits your needs, from basic API functionality to full-featured setups.

## 📦 Available Samples

### 1️⃣ Minimal Setup
This sample demonstrates a minimal Docker Compose setup for Binacle.Net with essential features.

**Directory**: `samples/docker/minimal-setup`

Key Features:
- Basic API functionality
- Lightweight configuration for easy setup and testing
- Customizable bin configurations via Presets.json


### 2️⃣ UI Setup
A sample focused on running Binacle.Net with all the UI features. 
This setup is ideal for users interacting with Binacle.Net for the first time or for demo purposes.

**Directory**: `samples/docker/ui-setup`

Key Features:
- UI Module for interactive demos
- Swagger & Scalar UI for API exploration
- Customizable bin configurations via Presets.json


### 3️⃣ Service Npgsql
An as a Service sample with PostgreSQL as the database backend and the UI Module enabled for a user-friendly interface.

**Directory**: `samples/docker/service-npgsql`

Key Features:
- Full Service Module functionality
- PostgreSQL database backend
- UI Module for easy interaction
- Swagger & Scalar UI for API exploration
- Customizable bin configurations via Presets.json
- Health Checks
- Packing Logs

### 4️⃣ Service Azure
An as a Service sample with Azure services as the database backend and the UI Module enabled for a user-friendly interface.

**Directory**: `samples/docker/service-azure`

Key Features:
- Full Service Module functionality
- Azure services as the database backend
- UI Module for easy interaction
- Swagger & Scalar UI for API exploration
- Customizable bin configurations via Presets.json
- Health Checks
- Packing Logs
- OpenTelemetry for monitoring
- Aspire Dashboard for real-time metrics