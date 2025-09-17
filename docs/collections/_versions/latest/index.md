---
title       : Welcome to Binacle.Net Docs!
menu_title  : latest
permalink: /version/latest/
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

## [Release Notes]({% vlink release-notes.md %})
See what's new in the {{ page.version }} version of Binacle.Net.

## 🔍 [Core Concepts]({% link _common_pages/core-concepts.md %})
Discover the algorithms behind Binacle.Net and learn how it handles fitting and packing items into bins.

## 📡 [About the API]({% vlink /api/index.md %})
Understand the API endpoints, including how to use presets, send custom bin and item data, and query by preset to
find the most efficient packing solution.

Below are the main API versions and related resources:
- ⚖️ [V2]({% vlink /api/v2.md %}): ...TODO...
- 🧪 [V3]({% vlink /api/v3.md %}): ...TODO...

## 🔧 Configuration
Customize Binacle.Net to suit your environment. Explore the following configuration modules:

- 🏗️ Core: Provides essential API functionality, including Presets customization.
- 📊 Diagnostics Module: Configure logging, health checks, and telemetry.
- 🛡️ Service Module: Enable rate limiting, authentication, and cloud logging.
- 🖥️ UI Module: Enable the visual demo interface for packing simulations.

---

Binacle.Net Docs will guide you through every aspect of using and integrating Binacle.Net.
Whether you're looking for a quick deployment or deep customization, all the resources you need are here!
