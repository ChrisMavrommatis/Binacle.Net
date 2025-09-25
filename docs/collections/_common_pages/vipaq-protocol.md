--- 
title: ViPaq Protocol
nav:
  icon: 🗜️
  order: 99
---

**ViPaq** is protocol designed for compactly encoding packing information of a single bin.

By using efficient binary-level encoding, ViPaq enables:
- ✅ Compact storage of packing data
- ✅ Reduced bandwidth for data transfer
- ✅ Easy sharing via a concise copy-pastable string

> ⚠️ ViPaq is experimental and may change.
> 
> Consult the documentation for your Binacle.Net version, for availability and support.
{: .block-warning}

## 🎯 Purpose

Binacle.Net’s API provides detailed packing responses that can get large with many items.  
ViPaq compresses this data into a simple, portable string format—ideal for storage, transmission, or quick sharing.

## ⚙️ How It Works

ViPaq serializes the bin’s dimensions along with each item’s size and coordinates into a single encoded string, 
preserving all necessary data for visualization and decoding.

## 📌 Data Structure
```text
[Header] 
[Number of Items] 
[Bin: Length, Width, Height] 
[Item 1: Length, Width, Height, X, Y, Z]
[Item 2: Length, Width, Height, X, Y, Z] 
... 
[Item N: Length, Width, Height, X, Y, Z]
```

### 🛠️ Components

- **Header**: Decoding metadata
- **Number of Items**: Total encoded items
- **Bin**: Dimensions — Length, Width, Height
- **Items**: Each with dimensions (L, W, H) and position coordinates (X, Y, Z)

### 🔑 Encoding & Compression Techniques

- **Base64 Encoding**: Converts binary data into a transfer-friendly string
- **Variable Length Encoding (VLE)**: Reduces storage by minimizing redundant data
- **Gzip Compression**: Automatically applied for larger data, enhancing compactness
