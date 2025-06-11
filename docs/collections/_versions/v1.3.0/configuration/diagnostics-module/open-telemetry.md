---
title: Open Telemetry
nav:
  parent: Diagnostics Module
  order: 4
  icon: 📡
---

**OpenTelemetry** offers distributed tracing, metrics, and logging, enabling you to monitor and analyze your application's performance. If Binacle.Net is part of your system, you can push telemetry data to popular platforms like **Grafana**, **Jaeger**, **Prometheus**, and **Azure Application Insights** for real-time observability.

## 🛠️ Configuration
OpenTelemetry is configured via the `OpenTelemetry.json` file.

Below is the default configuration:

```json
{
  "OpenTelemetry": {
    "ServiceNamespace": "binacle",
    "ServiceInstanceId": "1",
    "AdditionalAttributes": {
      
    },
    "Otlp": {
      "Enabled": false,
      "Endpoint": null,
      "Protocol": "grpc"
    },
    "AzureMonitor": {
      "Enabled": false,
      "ConnectionString": null,
      "EnableLiveMetrics": false,
      "SamplingRatio": 1
    },
    "Metrics": {
      "AdditionalMeters": [

      ],
      "AdditionalAttributes": {
        
      }
    },
    "Tracing": {
      "AdditionalSources": [

      ],
      "AdditionalAttributes": {
        
      }
    },
    "Logging": {
      "AdditionalAttributes": {
        
      }
    }
  }
}
```

You can modify OpenTelemetry configuration using **Production Overrides** by creating an `OpenTelemetry.Production.json` file, or by using **Environment Variables**.

- 📁 **Location**: `/app/Config_Files/DiagnosticsModule`
- 📌 **Full Path**: `/app/Config_Files/DiagnosticsModule/OpenTelemetry.Production.json`

For more information on overriding configurations, refer to the [Configuration]({% vlink /configuration/index.md %}#%EF%B8%8F-overriding-configuration) page.

## 🔧 Configuration Options

### 🌍 General Settings
- `ServiceNamespace` (_string_) – Defines the namespace for your service.
- `ServiceInstanceId` (_string_) – Unique identifier for the service instance.
- `AdditionalAttributes` (_object_) – Custom key-value attributes to attach to telemetry data.
### 📊 Metrics
- `AdditionalMeters` (_array_) – List of additional meters to include for metric collection.
- `AdditionalAttributes` (_object_) – Custom attributes for metrics.
### 🎯 Tracing
- `AdditionalSources` (_array_) – Additional tracing sources to listen to.
- `AdditionalAttributes` (_object_) – Custom attributes for traces.
### 📜 Logging
- `AdditionalAttributes` (_object_) – Custom attributes for log entries.

### 🌐 OTLP (OpenTelemetry Protocol) Exporter
The OTLP Exporter is the recommended way to send OpenTelemetry data. It allows you to export traces, metrics, and logs to platforms like **Grafana**, **Jaeger**, **Tempo**, **Prometheus**, and more.

- `Enabled` (_boolean_) – Enables or disables the OTLP exporter.
- `Endpoint` (_string?_) – The OTLP collector URL (e.g., `http://localhost:4317`).
- `Protocol` (_string_) – Supported values:
    - `"grpc"` (__default__)
    - `"http/protobuf"`

The recommended way to configure the OTLP exporter is via the officially supported environment variables.

For a full list of available variables, check the [OpenTelemetry .NET OTLP Exporter Docs](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.OpenTelemetryProtocol/README.md#environment-variables).


### ☁️ Azure Monitor Exporter
The Azure Monitor Exporter sends telemetry to Azure Application Insights for monitoring and live diagnostics.

- `Enabled` (_boolean_) – Enables or disables the Azure Monitor exporter.
- `ConnectionString` (_string?_) – The Azure Monitor connection string.
- `EnableLiveMetrics` (_boolean_) – Enables live metrics streaming in Application Insights.
- `SamplingRatio` (_float_, default: `1`) – Defines how much telemetry data to sample (1 = 100% of requests, 0.5 = 50%, etc.).

🔹 Azure Monitor Exporter requires an Application Insights resource. You can retrieve your connection string from the Azure Portal under the "Instrumentation Key" section of your Application Insights instance.
