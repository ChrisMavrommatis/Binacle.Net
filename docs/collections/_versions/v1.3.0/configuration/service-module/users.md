---
title: Users
nav:
  parent: Service Module
  order: 3
  icon: 👥
---


In the **Service Module**, only **admin users** have the ability to manage other users. To ensure there’s always an admin available, a default admin account is provided. This section will guide you through configuring the default admin and managing admin accounts effectively.

## 🛠️ Configuration
The user setting are configured via the `Users.json` file.

**Default configuration:**
```json
{
  "Users": {
    "DefaultAdminUser": "admin@binacle.net:B1n4cl3Adm!n"
  }
}
```

You can modify the Users using **Production Overrides** by creating a `Users.Production.json` file, or by using **Environment Variables**.
- 📁 Location: `/app/Config_Files/ServiceModule`
- 📌 Full Path: `/app/Config_Files/ServiceModule/Users.Production.json`

For more information on this refer to the [Configuration](../../#%EF%B8%8F-overriding-configuration) page.

> [!Warning]
> Environment variables take precedence over settings defined in the `Users.json` and `Users.Production.json` files.
>
> This allows you to securely store the default admin credentials without exposing them in configuration files.

## 🔧 Configuration Options
- **DefaultAdminUser** : Specifies the default admin user. The fomat is `{email}:{password}`.

> [!Note]
>
> The Service Module will always ensure the default admin exists in the database. However, it does not enforce whether the account is active.

## 🔑 Recovering Admin Access
In the event that you accidentally demote yourself or deactivate the only active admin user, and the default admin is also inactive, you have two options to recover access:

1. **Update the Default Admin Credentials**:
   Use any preferred method (Environment Variables, Production Overrides) to change the default admin credentials through `DefaultAdminUser`. This will create a new admin account upon application restart.

2. **Direct Database Modification**:
   Directly modify the database to either reactivate the default admin or promote another user to admin. This is often the most efficient recovery method without having to rely on multiple admin accounts.