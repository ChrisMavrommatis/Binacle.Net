---
title: Core Concepts
nav:
  order: 2
  icon: 🔍
---


Binacle.Net is designed to solve the 3D Bin Packing Problem efficiently using specialized 
**heuristic algorithms** and **real-time computation techniques**. 

By balancing speed and accuracy, it provides optimized packing solutions for logistics, warehousing, 
and e-commerce applications.

---


## Contents
Binacle.Net leverages specialized algorithms and techniques to address various aspects of the bin packing problem.

Below are the key sections detailing how the system functions:
- [🧠 Algorithms](#-algorithms)
- [🛠️ Functions](#️-functions)

---

## 🧠 Algorithms
In order to solve the Bin Packing problem in real time Binacle.Net employs heuristic algorithms suitable for real-time use.

While such algorithms, like FFD, don’t always guarantee a theoretically optimal solution,
Binacle.Net is designed to ensure that when it confirms a bin is suitable, all items will fit without error. 

However, in rare cases, the algorithm might miss possible fits because of its heuristic approach,
a trade-off favoring speed in practical scenarios.

### ⚖️ First Fit Decreasing (FFD)
Binacle.Net's hybrid First Fit Decreasing (FFD) algorithm sorts items by decreasing size and places each item in the 
first available space that fits within a bin.

- ✅ Places each item as soon as a space is found, without searching for a better one
- ⚖️ Not always perfectly optimal, may leave unused space

### 🧊 Worst Fit Decreasing (WFD)
Worst Fit Decreasing (WFD) is another hybrid heuristic. 
Items are sorted by size and placed in the space leaving the most unused room in the bin.

- ✅ Useful in niche situations
- ⚖️ Tends to spread items out, which may help with distribution but not always with space usage

### 📏 Best Fit Decreasing (BFD)
Best Fit Decreasing (BFD) aims for the most snug packing, placing each item in the spot that leaves the least
unused space in the bin.

- ✅ Keeps the space left around each placement as small as it can
- ⚖️ Examines the candidate spaces for each item rather than taking the first one that fits


> Not all algorithms may be supported in every API release. Check the API documentation for your Binacle.Net version.
{:.block-note }

> Which algorithm is fastest, or packs tightest, depends on your data and on the Binacle.Net version you run.
> These descriptions say what each algorithm does, not how they rank against each other. If it matters to your
> workload, measure all three on your own bins and items.
{:.block-note }

---

## 🛠️ Functions
Binacle.Net provides two core operations.

- 🧩 **Fitting**: Checks whether a set of items can fit inside a bin.
- 📦 **Packing**: Not only determines if items fit but also calculates their exact placement within the bin.

### 🧩 Fitting
The Fitting function evaluates if a given set of items can fit into a specified bin.

**Why use Fitting?**
- ✅ Ideal for pre-checks, ensuring items fit before checkout or shipping
- ✅ Returns results indicating which items fit and which do not
- ✅ Provides a quick, real-time assessment of bin suitability

### 📦 Packing
The Packing function goes beyond simple fitting. It determines where each item is placed within the bin. If all items don't fit, it optimizes placement to pack as many items as possible.

**Why use Packing?**
- ✅ Tracks the exact position of each item within the bin
- ✅ Optimizes space usage, maximizing packing efficiency
- ✅ Helps fulfillment teams by providing step-by-step instructions for packing


Both functions are engineered for high-speed performance and precision, enabling instantaneous packing decisions for production environments.

📌 For more technical details, consult the API documentation for your Binacle.Net version.