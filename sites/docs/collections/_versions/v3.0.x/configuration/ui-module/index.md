---
title: UI Module
permalink: /version/v3.0.x/configuration/ui-module/
nav:
  parent: Configuration
  order: 4
  icon: 🖥️
---

The UI Module provides a user-friendly interface for interacting with Binacle.Net, allowing users to explore its features visually. 
This makes it easier to demonstrate and understand the system’s capabilities without making direct API calls.

> This module is disabled by default.
{: .block-note}

## ⚙️ Configuration
All configuration files for the UI Module are located in the `/app/Config_Files/UiModule` directory.

### 📑 Directory Structure
```text
app
└── Config_Files
    └── UiModule
        └── ConnectionStrings.json
```

## 📦 Packing Demo
The Packing Demo allows users to interact with the packing process visually by submitting bin and
item data and observing how items are packed into bins.

- 🔹 Step-by-step Interaction: Users can navigate through the packing process, handling items one by one.
- 🔹 Real-time Visualization: Watch as each item is placed inside the bins in real-time.

## 📜 Protocol Decoder
The Protocol Decoder enables users to decode ViPaq-encoded packing data and visualize the container layouts interactively.
It helps analyze packing arrangements and navigate through the layout easily.

- 📌 To obtain ViPaq data, set `includeViPaqData` on a packing request in
  [Version 3]({% vlink /api/v3.md %}) or [Version 4]({% vlink /api/v4.md %}) of the API.
- 📌 The decoder reads the **v3.0.0 format only**. Data produced by v2.1.1 and earlier is rejected - see
  [ViPaq Protocol]({% vlink vipaq-protocol.md %}).


## 🔧 Activating the UI Module
To enable the UIModule, set the environment variable:
```bash
UI_MODULE=True
```

## 🛠️ Configuration
The UI Module attempts to auto-detect the **Binacle.Net API URL** by default.
However, in some cases—particularly when using proxies or forwarding services—automatic detection may fail,
leading to issues with the Packing Demo or API communication.

If automatic detection fails, you can manually specify the API endpoint for **BinacleApi** in the
`ConnectionStrings.json` file.

**Default configuration:**
```json
{
  "ConnectionStrings": {
    "BinacleApi": "endpoint=https://localhost:8080/"
  }
}
```

🔗 For more details on how to configure connection strings, refer to the
[Configuration Basics > Connection String Fallbacks]({% link _common_pages/configuration-basics.md %}#-connection-string-fallbacks) section.
