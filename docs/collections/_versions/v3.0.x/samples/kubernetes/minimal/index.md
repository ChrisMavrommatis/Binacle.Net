---
title: Minimal
permalink: /version/v3.0.x/samples/kubernetes/minimal/
nav:
  order: 1
  parent: Kubernetes
  icon: 1️⃣
---

A minimal deployment of Binacle.Net on an existing Kubernetes cluster.

## 🛠️ Prerequisites
- An existing Kubernetes cluster.
- One or more eshops already running in the cluster, typically with Nginx Ingress Controller.
- Binacle.Net will run in the same cluster and not be exposed.
- `kubectl` configured to interact with your cluster.

## 📥 Download the following files
- [`binacle-deployment.yaml`]({% vlink /samples/kubernetes/minimal/binacle-deployment.yaml %}){:download="" target="_blank"}
- [`binacle-net-service.yaml`]({% vlink /samples/kubernetes/minimal/binacle-net-service.yaml %}){:download="" target="_blank"}
- [`binacle-presets-configmap.yaml`]({% vlink /samples/kubernetes/minimal/binacle-presets-configmap.yaml %}){:download="" target="_blank"}
- [`binacle-pvc.yaml`]({% vlink /samples/kubernetes/minimal/binacle-pvc.yaml %}){:download="" target="_blank"}

## ✏️ Customize
Edit the JSON in `binacle-presets-configmap.yaml` to use your own bins.

> `binacle-pvc.yaml` assumes your cluster has dynamic provisioning enabled.
> Otherwise you will need to create a corresponding PersistentVolume.
{:.block-note}

## 🚀 Running the Application
Apply the configurations:
```bash
kubectl apply -f binacle-pvc.yaml
kubectl apply -f binacle-presets-configmap.yaml
kubectl apply -f binacle-deployment.yaml
kubectl apply -f binacle-net-service.yaml
```

This launches the Binacle.Net API with:
- 📖 **Custom Presets**: loaded from your `binacle-presets-configmap.yaml`.
- 📂 **Logs Folder**: a persistent volume for application logs.
- ⚙️ **Service**: a ClusterIP service for internal communication within the cluster.

## 🌐 Accessing the API
Once the deployment is running, other services in the same cluster reach the API on:
```text
http://binacle-net-service:8080/
```

## 🌐 Behind an ingress controller
An ingress controller is a proxy, so the app sees its address rather than the caller's. If you restrict health
checks by IP, or run the Service Module with rate limiting, enable
[Forwarded Headers]({% vlink /configuration/core/forwarded-headers.md %}) as well.

## 📄 Additional Resources
- [Kubernetes Documentation](https://kubernetes.io/docs/home/)
