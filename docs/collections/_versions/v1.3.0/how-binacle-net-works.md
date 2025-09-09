---
title: How Binacle.Net Works
nav:
  order: 2
  icon: 🔍
---

Binacle.Net is designed to solve the 3D Bin Packing Problem efficiently using specialized **heuristic algorithms** and **real-time computation techniques**. By balancing speed and accuracy, it provides optimized packing solutions for logistics, warehousing, and e-commerce applications.

##### Contents
Binacle.Net leverages specialized algorithms and techniques to address various aspects of the bin packing problem. Below are the key sections detailing how the system functions:
- [🧠 Algorithms](#-algorithms)
- [🛠️ Functions](#-functions)

---

## 🧠 Algorithms
In order to solve the Bin Packing problem in real time Binacle.Net employs heuristic algorithms.

While heuristic algorithms like FFD are not guaranteed to always find the perfect solution (i.e., 100% accuracy), 
Binacle.Net is designed to ensure that when it identifies that a bin is suitable, it will invariably accommodate all items. 
However, it's worth noting that due to its heuristic nature, there are instances where items might technically 
fit into a bin, but the algorithm may not recognize it as the best option.

⚡ This trade-off enables faster, real-time calculations while maintaining a high success rate for practical use cases.

### ⚖️ First Fit Decreasing (FFD)
Binacle.Net implements a hybrid variant of the First Fit Decreasing (FFD) algorithm,
a widely adopted heuristic for bin packing problems. 

This algorithm sorts items in decreasing order of size and places each item into the first available
space within a single bin that can accommodate it.

**Why FFD?**
- ✅ Balances efficiency and speed
- ✅ Outperforms WFD & BFD in computational speed
- ⚖️ Not always optimal, may leave gaps

### ⚠️ Worst Fit Decreasing (WFD) (Experimental)
The Worst Fit Decreasing (WFD) algorithm is a hybrid heuristic currently in the experimental stage, 
accessible via the **v3 API endpoint**.

Like FFD and BFD, it sorts items in decreasing order of size but deliberately places each item in the least 
optimal (worst) available space within a single bin—specifically, where it leaves the most unused space after placement.

- ✅ Can have better packing efficiency in niche scenarios
- ⚖️ Spreads items out, which may or may not be ideal
- ❌ Slower than FFD & WFD, with generally weaker efficiency

### ⚠️ Best Fit Decreasing (BFD) (Experimental)
The Best Fit Decreasing (BFD) algorithm, also in the experimental stage and available via the **v3 API endpoint**,
is another hybrid heuristic. 
Like FFD and WFD, it sorts items in decreasing order of size but aims to place each item in the most optimal (best)
available space within a single bin—specifically, where it leaves the least remaining space after placement.

**Why BFD?**
- ✅ Packing efficiency is usually better compared to FFD and WFD
- ⚖️ Falls between FFD and WFD in computational performance

---

## 🛠️ Functions
Binacle.Net provides two essential functions to address your packing needs: **Fitting** and **Packing**.

- 🧩 **Fitting**: Checks if a set of items can fit into a bin.
- 📦 **Packing**: Not only determines if items fit, but also calculates their precise placement within the bin.

These functions are designed for speed and precision, enabling quick, real-time packing decisions.

📌 For a deeper dive into these functions, visit the [About the API]({% vlink /api/index.md %}) page.

