---
title       : Welcome to Binacle.Net Docs!
menu_title  : v1.3.0
permalink: /version/v1.3.0/
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

Explore the following sections to get started and learn more about Binacle.Net:

## 🚀 [Quick Start]({% vlink /quick-start.md %})
Get started with Binacle.Net in just a few steps! The Quick Start Guide covers the basics you need to run it quickly.

## 🔍 [How Binacle.Net Works]({% vlink /how-binacle-net-works.md %})
Discover the algorithms behind Binacle.Net and learn how it handles fitting and packing items into bins.

## 📡 [About the API]({% vlink /api/index.md %})
Understand the API endpoints, including how to use presets, send custom bin and item data, and query by preset to 
find the most efficient packing solution.

Below are the main API versions and related resources:
- 🚨 [V1]({% vlink /api/v1.md %}): The original version of the API. **It is deprecated**
- ⚖️ [V2]({% vlink /api/v2.md %}): Introduces several new features, including the packing function which tracks the 
  positions of the items.
- 🧪 [V3]({% vlink /api/v3.md %}): Introduces algorithm selection: **This version is still experimental**
- 👥 [Users]({% vlink /api/users.md %}): Becomes available only after you enable the Service Module.

## 🔗 [Integration Guide]({% vlink /integration-guide/index.md %})
Learn how to integrate Binacle.Net into your platform. Includes detailed sections on:

- [📏 Dimensions and Unit of Measurement]({% vlink /integration-guide/dimensions-and-unit-of-measurement.md %})
- [📦 Your Bin Set]({% vlink /integration-guide/your-bin-set.md %})
- [🌟 Typical Integration Process]({% vlink /integration-guide/typical-integration-process.md %})

## 🔧 [Configuration]({% vlink /configuration/index.md %})
Customize Binacle.Net to suit your environment. Explore the following configuration modules:

- [🏗️ Core]({% vlink /configuration/core/index.md %}): Provides essential API functionality, including Presets customization.
- [📊 Diagnostics Module]({% vlink /configuration/diagnostics-module/index.md %}): Configure logging, health checks, and telemetry.
- [🛡️ Service Module]({% vlink /configuration/service-module/index.md %}): Enable rate limiting, authentication, and cloud logging.
- [🖥️ UI Module]({% vlink /configuration/ui-module/index.md %}): Enable the visual demo interface for packing simulations.

---

Binacle.Net Docs will guide you through every aspect of using and integrating Binacle.Net. 
Whether you're looking for a quick deployment or deep customization, all the resources you need are here!
