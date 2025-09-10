---
title: API
permalink: /version/v1.3.0/api/
nav:
  order: 5
  icon: 📡
---

Three API versions are available, each suited to different requirements.

## Prerequisites

- [Core Concepts]({% link pages/core-concepts.md %}) - Understand the algorithms and functions used by Binacle.Net.

---

## 🚨 Version 1
Version 1 focuses solely on the Fitting function, identifying the smallest bin that can accommodate all provided items.

> This version is deprecated and may not receive updates or support.
> 
> It will be removed in Binacle.Net V2.0.0.
> It is recommended to use Version 2.
{: .block-warning}

➡️ Learn more about [Version 1]({% vlink /api/v1.md %})

---

## ⚖️ Version 2
Version 2 of the Binacle.Net API expands upon Version 1 by providing both the Fitting and Packing functions. 

In addition to finding the smallest bin for fitting, it returns results for all bins. 

This version provides more detailed information and increased flexibility compared to the deprecated Version 1.

➡️ Learn more about [Version 2]({% vlink /api/v2.md %})

---

## 🧪 Version 3 
Version 3 of the Binacle.Net API builds upon Version 2's Pack By Custom but only accepts an algorithm selection as a parameter. 

It also introduces ViPaq, a protocol for encoding packing information efficiently.

> This API version is experimental and can change at any time.
{: .block-warning}

➡️ Learn more about [Version 3]({% vlink /api/v3.md %})

---
