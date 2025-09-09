---
title: Core Concepts
permalink: core-concepts
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
- [🧠 Algorithms](#algorithms)
- [🛠️ Functions](#functions)

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

- ✅ Fast and efficient for large datasets
- ✅ Outperforms WFD and BFD in computational speed
- ⚖️ Not always perfectly optimal, may leave unused space

### 🧊 Worst Fit Decreasing (WFD)
Worst Fit Decreasing (WFD) is another hybrid heuristic. 
Items are sorted by size and placed in the space leaving the most unused room in the bin.

- ✅ Efficient in niche situations
- ⚖️ Tends to spread items out, which may help with distribution but not always with space usage
- ❌ Generally slower and less efficient than FFD and BFD

### 📏 Best Fit Decreasing (BFD)
Best Fit Decreasing (BFD) aims for the most snug packing, placing each item in the spot that leaves the least
unused space in the bin.

- ✅ Often yields slightly better packing efficiency than FFD or WFD
- ⚖️ A middle ground in computational performance compared to FFD and WFD


> Not all algorithms may be supported in every API release. Check the API documentation for your Binacle.Net version.
{:.block-note }

---

## 🛠️ Functions
Binacle.Net provides two ecore operations.

- 🧩 **Fitting**: Determines if a set of items can physically fit inside a bin.
- 📦 **Packing**: Calculates if items fit and provides their exact placement coordinates within the bin.

Both functions are engineered for high-speed performance and precision, enabling instantaneous packing decisions for production environments.

📌 For more technical details, consult the API documentation for your Binacle.Net version.