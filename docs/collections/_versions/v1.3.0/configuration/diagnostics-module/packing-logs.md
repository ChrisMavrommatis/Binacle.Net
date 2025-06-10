---
title: Packing Logs
nav:
  parent: Diagnostics Module
  order: 3
  icon: 📦
---

Packing Logs track API usage by logging requests, parameters, and results. These logs help you analyze:

- 📊 **Service Usage** – Understand how the API is being utilized.
- 📏 **Popular Sizes** – Identify the most frequently requested package dimensions.
- 📦 **Packing Efficiency** – Determine the frequency of successful packings.
- 🔄 **Function Popularity** – Track which packing or fitting functions are used most often.

Packing Logs are stored in **NDJSON** (newline-delimited JSON) format. Binacle.Net does not perform any built-in analysis; the logs are simply generated, and interpretation is left to external tools.

## 🛠️ Configuration
Packing Logs are configured via the `PackingLogs.json` file.

**Default configuration:**
```json
{
  "PackingLogs": {
    "Enabled": false,
    "LegacyPacking": {
      "Path": "data/pack-logs/legacy-packing/",
      "FileName": "{0}.ndjson",
      "DateFormat": "yyyyMMdd",
      "ChannelLimit": 100
    },
    "LegacyFitting": {
      "Path": "data/pack-logs/legacy-fitting/",
      "FileName": "{0}.ndjson",
      "DateFormat": "yyyyMMdd",
      "ChannelLimit": 100
    },
    "Packing": {
      "Path": "data/pack-logs/packing/",
      "FileName": "{0}.ndjson",
      "DateFormat": "yyyyMMdd",
      "ChannelLimit": 100
    }
  }
}
```

You can modify the Packing Logs using **Production Overrides** by creating a `PackingLogs.Production.json` file, or by using **Environment Variables**.
- 📁 **Location**: `/app/Config_Files/DiagnosticsModule`
- 📌 **Full Path**: `/app/Config_Files/DiagnosticsModule/PackingLogs.Production.json`

For more information on overriding configurations, refer to the [Configuration](../../#%EF%B8%8F-overriding-configuration) page.

## 🔧 Configuration Options
- `Enabled` (_boolean_) – Enables or disables packing logs.
- Log Types:
    - `LegacyPacking` – Logs from **v2 packing functions**.
    - `LegacyFitting` – Logs from **v1 & v2 fitting functions**.
    - `Packing` – Logs from **v3 packing functions**.

For each type the options is as follows.
- `Path` (_string_) – Directory where log files are stored.
- `FileName` (_string_) – Log file name. `{0}` represents the date.
- `DateFormat` (_string_) – Defines the format for `{0}` in FileName (e.g., `yyyyMMdd`).
- `ChannelLimit` (_integer_) – Maximum queue size for logs:
    - `0` = Unlimited <br>
      Limited only by available system memory.
    - `> 0` = Limits the log queue size. <br>
      If requests come in and the log writter can't keep up causing the queue to exceed this limit, then the newest logs will be dropped to prevent system overload.