# Binacle.Net - Minimal Setup
This sample demonstrates how to set up and run Binacle.Net on an existing Kubernetes cluster.

## Prerequisites
- An existing Kubernetes cluster.
- One or more eshops already running in the cluster, typically with Nginx Ingress Controller.
- Binacle.Net will run in the same cluster and not be exposed.
- `kubectl` configured to interact with your cluster.

## 📥 Getting Started
1. **Clone the Repository**<br>
   Clone or download this repository to your local machine.
   ```bash
   git clone https://github.com/ChrisMavrommatis/Binacle.Net.git
    ```
    Alternatively, download the contents of this folder directly.
2. **Verify Files**<br>
   Ensure the following files are present in the same directory:
   - `binacle-deployment.yaml` – Kubernetes deployment configuration for all services.
   - `binacle-net-service.yaml` – Kubernetes service configuration for Binacle.Net.
   - `binacle-presets-configmap.yaml` – ConfigMap configuration to load a custom`Presets.json`.
   - `binacle-pvc.yaml` – Persistent Volume Claim for logs.

3. **Customize (Optional)**<br>
   Edit the json in the `binacle-presets-configmap.yaml` file to customize the presets as needed.

> [!Note]
> `binacle-pvc.yaml` assumes your cluster has dynamic provisioning enabled. 
> Otherwise, you’ll need to create a corresponding PersistentVolume.

## 🚀 Running the Application
Apply all the configurations by running:
```bash
kubectl apply -f binacle-pvc.yaml
kubectl apply -f binacle-presets-configmap.yaml
kubectl apply -f binacle-deployment.yaml
kubectl apply -f binacle-net-service.yaml
```

This will launch the Binacle.Net API with:
- 📖 **Custom Presets**: Loaded from your `binacle-presets-configmap.yaml`.
- 📂 **Logs Folder**: A persistent volume will be created to store API logs for monitoring and debugging.
- ⚙️ **Service**: A ClusterIP service will be created to allow internal communication within the cluster.

## 🌐 Accessing the API
Once the deployment is running, other services in the same cluster can interact with the API on:
```text
http://binacle-net-service:8080/
```


## 📄 Additional Resources
- [Binacle.Net Documentation](https://docs.binacle.net/)
- [Kubernetes Documentation](https://kubernetes.io/docs/home/)
