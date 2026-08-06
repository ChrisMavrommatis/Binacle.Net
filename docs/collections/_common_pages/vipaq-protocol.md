--- 
title: ViPaq Protocol
nav:
  icon: 🗜️
  order: 99
---

**ViPaq** is a protocol for compactly encoding the packing information of a single bin.

By using efficient binary-level encoding, ViPaq enables:
- ✅ Compact storage of packing data
- ✅ Reduced bandwidth for data transfer
- ✅ Easy sharing via a concise copy-pastable string

> This page describes what ViPaq is. The exact format is documented per version - see
> [the ViPaq Protocol page for {{ site.data.versions.current }}]({{ '/version/' | append: site.data.versions.current | append: '/vipaq-protocol/' | relative_url }}),
> or pick your version from the [Versions]({% link _common_pages/version.html %}) page.
{: .block-note}

## 🎯 Purpose

Binacle.Net's API provides detailed packing responses that can get large with many items.
ViPaq condenses this data into a single portable string, ideal for storage, transmission, or quick sharing.

## ⚙️ How It Works

ViPaq encodes one packing result: a single bin plus the items packed into it.

It carries:

- **Bin**: its dimensions - Length, Width, Height
- **Items**: each item's dimensions (L, W, H) and its position in the bin (X, Y, Z)

That is everything needed to redraw the packing, and nothing else. Items come back in the order they were sent.

## 🔄 Tokens Do Not Move Between Versions

A ViPaq string is only readable by an implementation that speaks the same version of the format, and the format
changed in Binacle.Net v3.0.0.

- Binacle.Net **v2.1.1 and earlier** produce the older format.
- Binacle.Net **v3.0.0 and later** produce and read only the newer one. There is no fallback reader.

If you run an old and a new image side by side, their tokens do not interoperate. An old string handed to a new
decoder fails with a format error rather than being silently misread, so the failure is visible rather than
producing a wrong packing.

Stored strings are worth regenerating after an upgrade. Consult the page for your version for the format itself,
and the API documentation for your version for where ViPaq data is available.
