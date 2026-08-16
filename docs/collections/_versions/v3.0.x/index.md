---
title       : Welcome to Binacle.Net Docs!
menu_title  : v3.0.x
permalink: /version/v3.0.x/
nav:
  exclude: true
  order: 1
  icon: 🏠
---


Binacle.Net is an API created to address the 3D Bin Packing Problem in real time.

When provided with a set of bins and items, Binacle.Net quickly identifies the most suitable bin,
if available, that can accommodate all items efficiently. 

This capability is particularly valuable for websites offering locker shipping options, allowing them to present 
this choice to customers during critical stages, such as the cart or checkout process.

By employing heuristic algorithms, Binacle.Net ensures rapid responses and minimal wait times for customers. 
E-commerce platforms can leverage this API to either base their packaging on the dimensions of the lockers themselves 
or utilize pre-defined boxes designed to fit seamlessly within those lockers.

---

## 🚀 [Quick Start]({% vlink /quick-start.md %})
Get started with Binacle.Net in just a few steps! The Quick Start Guide covers the basics you need to run it quickly.

## 🛠️ [Release Notes]({% vlink release-notes.md %})
See what's new in the {{ page.version }} version of Binacle.Net.

## 📡 [API]({% vlink /api/index.md %})
Understand the API endpoints, including how to use presets, send custom bin and item data, and query by preset to
find the most efficient packing solution.

Below are the main API versions and related resources:
- ✅ [V3]({% vlink /api/v3.md %}): fitting and packing with a choice of algorithm. Stable, and the recommended version.
- 🧪 [V4]({% vlink /api/v4.md %}): 16 endpoints organized by the answer you want. **Experimental** - it can change at any time.

**V2 was removed in this version.** If you still call it, see the
[v2.1.x documentation]({{ '/version/v2.1.x/' | relative_url }}).

## 🗜️ [ViPaq Protocol]({% vlink vipaq-protocol.md %})
The compact format the packing endpoints return. The format changed in v3.0.0 and is stable from this release.

## 🔧 Configuration
Customize Binacle.Net to suit your environment. Explore the following configuration modules:

- [🏗️ Core]({% vlink /configuration/core/index.md %}): Provides essential API functionality, including Presets and running behind a proxy.
- [📊 Diagnostics Module]({% vlink /configuration/diagnostics-module/index.md %}): Configure logging, health checks, and telemetry.
- [🛡️ Service Module]({% vlink /configuration/service-module/index.md %}): Allows Binacle.Net to run as a Service. Built for the hosted service - **no public documentation from v2.0.0 onward**.
- [🖥️ UI Module]({% vlink /configuration/ui-module/index.md %}): Enable the visual demo interface for packing simulations.

## 📦 [Samples]({% vlink /samples/index.md %})
Docker Compose and Kubernetes setups to copy and edit, including one for running the API behind your own backend.

---

Binacle.Net Docs will guide you through every aspect of using and integrating Binacle.Net.
Whether you're looking for a quick deployment or deep customization, all the resources you need are here!
