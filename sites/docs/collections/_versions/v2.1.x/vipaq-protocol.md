---
title: ViPaq Protocol
nav:
  order: 7
  icon: 🗜️
---

This page describes the ViPaq wire format as produced and read by Binacle.Net {{ page.version }}.
For what ViPaq is and why it exists, see [ViPaq Protocol]({% link _common_pages/vipaq-protocol.md %}).

> ⚠️ ViPaq is experimental and may change between versions.
>
> Strings produced by this version are not readable by Binacle.Net v3.0.0 or later, which uses a different format.
{: .block-warning}

## ⚙️ How It Works

ViPaq serializes the bin's dimensions along with each item's size and coordinates into a single encoded string, 
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
