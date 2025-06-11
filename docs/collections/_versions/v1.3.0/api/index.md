---
title: API
permalink: /version/v1.3.0/api/
nav:
  order: 5
  icon: 📡
---

Binacle.Net is a powerful API designed to solve the 3D Bin Packing Problem in real time. Whether you need to **verify if items fit** or **optimize their placement**, Binacle.Net offers fast, efficient solutions.

With multiple API versions and flexible packing functions, Binacle.Net adapts to different use cases, from **e-commerce fulfillment** to **warehouse logistics**.

## 🔢 API Versions
Binacle.Net offers three versions of its API, each tailored to different needs.

### 🚨 Version 1 (Deprecated)
Version 1 focuses solely on the Fitting function, identifying the smallest bin that can accommodate all provided items.

➡️ Learn more about [Version 1]({% vlink /api/v1.md %})

### ⚖️ Version 2
Version 2 of the Binacle.Net API expands upon Version 1 by providing both the Fitting and Packing functions. In addition to finding the smallest bin for fitting, it returns results for all bins. This version provides more detailed information and increased flexibility compared to the deprecated Version 1.

➡️ Learn more about [Version 2]({% vlink /api/v2.md %})

### 🧪 Version 3 (Experimental)
Version 3 of the Binacle.Net API builds upon Version 2's Pack By Custom but only accepts an algorithm selection as a parameter. It also introduces ViPaq, a protocol for encoding packing information efficiently.

➡️ Learn more about [Version 3]({% vlink /api/v3.md %})

---

## 🛠️ Core Functions
Binacle.Net provides two functions to address different packing needs:

- 🧩 **Fitting**: Checks whether a set of items can fit inside a bin.
- 📦 **Packing**: Not only determines if items fit but also calculates their exact placement within the bin.

### 🧩 Fitting
The Fitting function evaluates if a given set of items can fit into a specified bin.

🚀 **Why use Fitting?**
- ✅ Ideal for pre-checks, ensuring items fit before checkout or shipping
- ✅ Returns results indicating which items fit and which do not
- ✅ Provides a quick, real-time assessment of bin suitability

### 📦 Packing
The Packing function goes beyond simple fitting. It determines where each item is placed within the bin. If all items don't fit, it optimizes placement to pack as many items as possible.

📌 **Why use Packing?**
- ✅ Tracks the exact position of each item within the bin
- ✅ Optimizes space usage, maximizing packing efficiency
- ✅ Helps fulfillment teams by providing step-by-step instructions for packing


