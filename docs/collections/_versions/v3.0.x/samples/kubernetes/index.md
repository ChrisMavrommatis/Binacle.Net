---
title: Kubernetes
permalink: /version/v3.0.x/samples/kubernetes/
nav:
  order: 2
  parent: Samples
  icon: ☸️
---

Sample configurations for running Binacle.Net on an existing Kubernetes cluster.

All the samples assume you:
- Have an existing Kubernetes cluster.
- Have one or more eshops already running in the cluster, typically with Nginx Ingress Controller.
- Will run Binacle.Net in the same cluster and not exposed.
- Have `kubectl` configured to interact with your cluster.

## 1️⃣ Minimal
A minimal Kubernetes setup with the essentials. Minimal suits a cluster well, since you rarely want the UI
exposed in one.

Key features:
- Basic API functionality
- Lightweight configuration for easy setup and testing
- Customizable bin configurations via `Presets.json`

Visit [Minimal]({% vlink /samples/kubernetes/minimal/index.md %}) for more details.
