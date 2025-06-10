---
title: Service Module
permalink: /version/v1.3.0/configuration/service-module/
nav:
  parent: Configuration
  order: 3
  icon: 🛡️
---

The Service Module enables Binacle.Net to function as a managed service in public environments, providing:
- ✅ Rate limiting
- ✅ User management
- ✅ Authentication

Authenticated users can bypass rate limits, ensuring seamless access to API endpoints.

> [!Note]
> This module is disabled by default.

While Binacle.Net is primarily designed for private cloud hosting, the Service Module enables controlled public deployment by balancing performance, security, and cost-effectiveness.

## ⚙️ Configuration
All configuration files for the Service Module are located in the `/app/Config_Files/ServiceModule` directory.

### 📑 Directory Structure
```text
app
└── Config_Files
    └── ServiceModule
        ├── ConnectionStrings.json
        ├── JwtAuth.json
        ├── RateLimiter.json
        └── Users.json
```

## 🗄️ Database
The Service Module relies on a database to manage users. You must configure a database for proper operation.

🔗 [Learn more about database configuration →](./database/)

## 🔐 Authentication
The module uses **stateless JWT tokens** for authentication. Users authenticate using their **email and password** to receive a token.

🔗 [See authentication configuration →](./authentication/)

## 👥 Users
When the Service Module is first enabled, a default admin user is created.

🔗 [Users configuration guide →](./users/)

## 📊 Rate Limiter
To prevent excessive requests, unauthenticated users are rate-limited.

🔗 [Configure rate limiting →](./rate-limiter/)

## 📡 API Endpoints
Once enabled, the Service Module exposes additional API endpoints for user authentication and management.

🔗 [See User's API details →](../../../api/users/)

## 🔧 Activating the Service Module
To enable the Service Module, set the environment variable:
```bash
SERVICE_MODULE=True
```

