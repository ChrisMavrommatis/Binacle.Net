---
title: Packing Logs
nav:
  parent: Diagnostics Module
  order: 3
  icon: 📦
---

Packing Logs track API usage by logging requests, parameters, and results. These logs help you analyze:

- 📊 **Service Usage** - Understand how the API is being utilized.
- 📏 **Popular Sizes** - Identify the most frequently requested package dimensions.
- 📦 **Packing Efficiency** - Determine the frequency of successful packings.
- 🔄 **Function Popularity** - Track which packing or fitting functions are used most often.

Packing Logs are stored in **NDJSON** (newline-delimited JSON) format.

Binacle.Net does not perform any built-in analysis; the logs are simply generated, and interpretation is left
to external tools.

> **The configuration was flattened in v3.0.0 and the change is breaking.** A configuration left in the old
> nested shape with `Enabled: true` now fails startup validation. See
> [Upgrading from v2.1.x](#upgrading).
{: .block-warning}

## 🛠️ Configuration
Packing Logs are configured via the `PackingLogs.json` file.

**Default configuration:**
```json
{
  "PackingLogs": {
    "Enabled": false,
    "Path": "data/pack-logs/",
    "FileName": "{0}.ndjson",
    "DateFormat": "yyyyMMdd",
    "ChannelLimit": 100,
    "RetentionDays": null
  }
}
```

You can modify the Packing Logs using **Production Overrides** by creating a
`PackingLogs.Production.json` file, or by using **Environment Variables**.
- 📁 **Location**: `/app/Config_Files/DiagnosticsModule`
- 📌 **Full Path**: `/app/Config_Files/DiagnosticsModule/PackingLogs.Production.json`

For more information on overriding configurations, refer to the
[Configuration Basics]({% link _common_pages/configuration-basics.md %}#%EF%B8%8F-overriding-configuration) page.

## 🔧 Configuration Options
- `Enabled` (_boolean_) - Enables or disables packing logs.
- `Path` (_string_) - Directory where log files are stored.
- `FileName` (_string_) - Log file name. `{0}` represents the date, and it is required.
- `DateFormat` (_string_) - Defines the format for `{0}` in FileName (e.g., `yyyyMMdd`).
- `ChannelLimit` (_integer_) - Maximum queue size for logs:
    - `0` = Unlimited <br>
      Limited only by available system memory.
    - `> 0` = Limits the log queue size. <br>
      If requests come in and the log writter can't keep up causing the
      queue to exceed this limit, then the newest logs will be dropped to prevent system overload.
- `RetentionDays` (_integer?_) - Delete log files older than this many days. See below.

Fitting and packing share one log. Both write to the same file.

## 🗑️ Retention
`RetentionDays` is **off by default** (`null`), which keeps every file until you remove it yourself.

Set it to a positive number of days and the app sweeps once on start and once a day after that, deleting log
files older than that. Each deletion is logged. Only files matching the configured `FileName` pattern in the
configured `Path` are touched, and only at the top level - nothing else in the folder is at risk.

```json
{
  "PackingLogs": {
    "Enabled": true,
    "RetentionDays": 30
  }
}
```

Left unset on a busy deployment, these files grow until the disk is full.

## 📄 What a log line looks like
One JSON object per line. Bins and items are written in a compact form, keyed by the id you sent:
`LxWxH` for a bin, `LxWxH [quantity]` for an item, and `LxWxH (x,y,z)` for a placed one.

```json
{"Timestamp":"2026-01-13T09:41:22.1830000+00:00","Parameters":["Packing","FFD"],"Bins":{"Small":"60x40x10"},"Items":{"box_1":"2x5x10 [2]"},"Results":{"Small":{"Status":"FullyPacked","PackedBinVolumePercentage":0.83,"PackedItemsVolumePercentage":100,"PackedItems":{"box_1":["2x5x10 (0,0,0)","2x5x10 (0,5,0)"]},"UnpackedItems":{}}}}
```

`Timestamp` is new in v3.0.0. Anything reading these files can ignore an unknown field, but a parser that
rejects one will need updating.

## 🔼 Upgrading from v2.1.x {#upgrading}
`Path`, `FileName`, `DateFormat` and `ChannelLimit` used to sit in nested `Fitting` and `Packing` blocks. They
now sit directly under `PackingLogs`, and the two blocks are gone.

**Before**
```json
{
  "PackingLogs": {
    "Enabled": true,
    "Fitting": {
      "Path": "data/pack-logs/fitting/",
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

**After**
```json
{
  "PackingLogs": {
    "Enabled": true,
    "Path": "data/pack-logs/",
    "FileName": "{0}.ndjson",
    "DateFormat": "yyyyMMdd",
    "ChannelLimit": 100
  }
}
```

Three things to do:

1. Move the four settings up one level and delete both blocks. Left as they were with `Enabled: true`, startup
   validation fails.
2. Repoint whatever collects the logs from `data/pack-logs/packing/` to `data/pack-logs/`. The old `packing/`
   and `fitting/` directories are safe to delete once you have kept what you want from them.
3. If you set these with environment variables, the names lose a level too:
   `PackingLogs__Packing__Path` becomes `PackingLogs__Path`.
