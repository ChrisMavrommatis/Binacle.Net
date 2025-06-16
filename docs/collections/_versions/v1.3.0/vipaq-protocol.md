--- 
title: ViPaq Protocol
nav:
  order: 99
---

**ViPaq** is a protocol designed to efficiently encode packing information for a single bin.

By leveraging binary-level encoding, it maximizes data compactness, making it ideal for:
- ✅ Storage – Efficiently save packing data
- ✅ Transfer – Minimize bandwidth usage
- ✅ Copy-Pasting – Easily share data as a compact string

> ViPaq is still experimental and may change in future versions.
{: .block-warning}

## 🎯 Purpose
ViPaq was developed to offer a simple, copy-pasteable string format for storing and transferring bin packing data.

While Binacle.Net’s API returns detailed packing data, responses can become large when handling numerous items. ViPaq addresses this by compressing the packing information into a compact, manageable representation.

## ⚙️ How it works
ViPaq encodes the bin's dimensions along with each item's dimensions and coordinates in a serialized format. This ensures all essential data is preserved for visualization and decoding.

## 📌 Data Structure
```bash
[Header] [Number of Items] [Bin: Length, Width, Height] [Item 1: Length, Width, Height, X, Y, Z] [Item 2: Length, Width, Height, X, Y, Z] ... [Item N: Length, Width, Height, X, Y, Z]
```

### 🛠️ Components
- **Header**: Contains metadata for decoding.
- **Number of Items**: Specifies how many items are encoded.
- **Bin**: Stores the bin’s length, width, and height.
- **Items**: Each item is represented by its:
  - **Dimensions** → (Length, Width, Height)
  - **Position** → (X, Y, Z)

### 🔑 Encoding & Compression
- ✅ Base64 Encoding → Converts the binary data into a **transferable** string format.
- ✅ Variable Length Encoding (VLE) → Optimizes storage by reducing unnecessary data.
- ✅ Gzip Compression → Automatically applied when the encoded data exceeds a size threshold.
