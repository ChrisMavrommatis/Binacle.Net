---
title: Authentication
nav:
  parent: Service Module
  order: 2
  icon: 🔐
---

The **Service Module** in Binacle.Net uses **stateless JWT tokens** to authenticate users and bypass the rate limiter. 
These tokens are generated through the authentication endpoint and remain valid for a specified duration.


## 🛠️ Configuration
The JWT Auth settings are configured via the `JwtAuth.json` file.

**Default configuration:**
```json
{
  "JwtAuth": {
    "Issuer": "https://localhost:7194",
    "Audience": "https://localhost:7194",
    "TokenSecret": "ThisIsAVerySecretKeyMeantToBeStoredSecurelyAndNotLikeThisSoPleaseChangeIt",
    "ExpirationInSeconds": 3600
  }
}
```

You can modify the JWT Settings using **Production Overrides** by creating a `JwtAuth.Production.json` file, 
or by using **Environment Variables**.
- 📁 **Location**: `/app/Config_Files/ServiceModule`
- 📌 **Full Path**: `/app/Config_Files/ServiceModule/JwtAuth.Production.json`

For more information on this refer to the 
[Configuration]({% vlink /configuration/index.md %}#%EF%B8%8F-overriding-configuration) page.

> Environment variables take precedence over settings defined in the `JwtAuth.json` and `JwtAuth.Production.json` files.
>
> This enables you to securely store only the **TokenSecret** as an environment variable while 
> keeping the rest of the configuration in the JSON file.
{: .block-warning}


## 🔧 Configuration Options
1. **Issuer**: The entity issuing the token, typically your application's base URL.
2. **Audience**: The intended recipient of the token, usually the same as the issuer.
3. **TokenSecret**: A secret key used to sign the JWT. Use a long and complex key for security.
4. **ExpirationInSeconds**: The token’s validity period (in seconds). Default: 3600 seconds (1 hour).


