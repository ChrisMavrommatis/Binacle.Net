---
title: Minimal Setup
permalink: /version/latest/samples/kubernetes/minimal-setup/
nav:
  order: 1
  parent: Kubernetes
  icon: 1️⃣
---

This sample demonstrates how to set up and run Binacle.Net on an existing Kubernetes cluster.

## 🛠️ Prerequisites
- An existing Kubernetes cluster.
- One or more eshops already running in the cluster, typically with Nginx Ingress Controller.
- Binacle.Net will run in the same cluster and not be exposed.
- `kubectl` configured to interact with your cluster.


## Download the following files
- [`binacle-deployment.yaml`]({% vlink /samples/kubernetes/minimal-setup/binacle-deployment.yaml %})
- [`binacle-net-service.yaml`]({% vlink /samples/kubernetes/minimal-setup/binacle-net-service.yaml %})
- [`binacle-presets-configmap.yaml`]({% vlink /samples/kubernetes/minimal-setup/binacle-presets-configmap.yaml %})
- [`binacle-pvc.yaml`]({% vlink /samples/kubernetes/minimal-setup/binacle-pvc.yaml %})


## Customize (Optional)
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
