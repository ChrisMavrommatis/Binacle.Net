---
title: Rate Limiter
nav:
  parent: Service Module
  order: 4
  icon: 📉
---


When the **Service Module** is enabled, **rate limiting** is applied to calculation endpoints to prevent abuse and ensure fair usage. By default, Binacle.Net enforces a predefined rate limit, but you can customize these settings to fit your needs.

## 🛠️ Configuration
The rate limiter is configured via the `RateLimiter.json` file.

**Default configuration:**
```json
{
  "RateLimiter": {
    "Anonymous" : "SlidingWindow::10/60-10"
  }
}
```

You can modify the Rate Limiter using **Production Overrides** by creating a `RateLimiter.Production.json` file, or by using **Environment Variables**.
- 📁 **Location**: `/app/Config_Files/ServiceModule`
- 📌 **Full Path**: `/app/Config_Files/ServiceModule/RateLimiter.Production.json`

For more information on this refer to the [Configuration]({% vlink /configuration/index.md %}#%EF%B8%8F-overriding-configuration) page.

## 🔧 Configuration Options

### 👤 Anonymous Requests
The Anonymous setting defines the rate limit for unauthenticated users.

This global limit applies collectively to all anonymous users, meaning if the limit is set to 10 requests per minute, it applies across all anonymous users, not individually.

## ⚙️ Rate Limiter Configuration
The rate limiter follows this syntax:
```text
{RateLimiterType}::{Number of requests}/{Time in seconds}[-{Segments}]
```

🔑 **Supported Rate Limiter Types**
- **Fixed Window** (`FixedWindow`)
- **Sliding Window** (`SlidingWindow`)


## 🕰️ Fixed Window
The **Fixed Window** strategy enforces a strict limit within a fixed time window. Once the window resets, users can make new requests.

**Format:**
```text
FixedWindow::{Number of requests}/{Time in seconds}
```

**Example:**
```text
FixedWindow::10/60
```

- **FixedWindow**: Uses the fixed window strategy.
- **10**: Allows 10 requests during the time window.
- **60**: Time window lasts 60 seconds (1 minute).

If a user exceeds this limit, their requests are rejected until the next window.

## ↔️ Sliding Window
The **Sliding Window** strategy provides a more flexible limit by dividing the time window into smaller segments. This helps smooth traffic distribution and prevents sudden bursts.

**Format:**
```text
SlidingWindow::{Number of requests}/{Time in seconds}-{Number of segments}
```

**Example:**
```text
SlidingWindow::100/30-3
```

- **SlidingWindow**: Uses the sliding window strategy.
- **100**: Allows 100 requests during the full time window.
- **30**: Time window lasts 30 seconds.
- **3**: The window is divided into 3 segments.

When a segment expires, its requests are recycled into the current segment for a more balanced request flow.
