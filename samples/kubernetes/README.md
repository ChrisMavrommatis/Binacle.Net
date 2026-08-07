# Kubernetes Samples
This folder provides sample configurations to run Binacle.Net on an existing kubernetes cluster.


All the samples assume you: 
- Have an existing Kubernetes cluster.
- Have one or more eshops already running in the cluster, typically with Nginx Ingress Controller.
- Will run Binacle.Net in the same cluster and not exposed.
- Have `kubectl` configured to interact with your cluster.

## 📦 Available Samples

### 1️⃣ Minimal
This sample demonstrates a minimal Kubernetes setup for Binacle.Net with essential features.
Minimal setup is ideal for Kubernetes clusters as you rarely want to expose the UI in such environments.

**Directory**: `samples/kubernetes/minimal`

Key Features:
- Basic API functionality
- Lightweight configuration for easy setup and testing
- Customizable bin configurations via Presets.json


