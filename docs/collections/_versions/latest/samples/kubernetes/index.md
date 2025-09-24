---
title: Kubernetes
permalink: /version/latest/samples/kubernetes/
nav:
  order: 2
  parent: Samples
  icon: ☸️ 
---

This section provides sample configurations to run Binacle.Net on an existing kubernetes cluster.


All the samples assume you:
- Have an existing Kubernetes cluster.
- Have one or more eshops already running in the cluster, typically with Nginx Ingress Controller.
- Will run Binacle.Net in the same cluster and not exposed.
- Have `kubectl` configured to interact with your cluster.

## 1️⃣ Minimal Setup
This sample demonstrates a minimal Kubernetes setup for Binacle.Net with essential features.
Minimal setup is ideal for Kubernetes clusters as you rarely want to expose the UI in such environments.

Key Features:
- Basic API functionality
- Lightweight configuration for easy setup and testing
- Customizable bin configurations via Presets.json

Visit the [Minimal Setup]({% vlink /samples/kubernetes/minimal-setup/index.md %}) for more details.
