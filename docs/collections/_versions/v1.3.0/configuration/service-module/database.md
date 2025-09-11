---
title: Database
nav:
  parent: Service Module
  order: 1
  icon: 🗄️
---

The Service Module requires a storage solution to save user data.

## 📋 Supported Databases
- Azure Storage (Azure Tables)

## 🛠️ Configuration
The database is configured via the `ConnectionStrings.json` file.

**Default configuration:**
```json
{
  "ConnectionStrings": {
    "AzureStorage": ""
  }
}
```

> Ensure only one database provider is configured at a time.
{: .block-warning}

🔗 For more details on how to configure connection strings, refer to the 
[Connection String Fallbacks section]({% vlink /configuration/index.md %}#-connection-string-fallbacks).

---

## 🏢 Azure Storage (Azure Tables)
To use Azure Storage, you must first create an Azure Storage account. 
The Service Module specifically interacts with Azure Tables and will automatically create a table named users.

> If a table with this name already exists, ensure there are no conflicts, or consider using a different storage account.
{: .block-warning}

> The Service Module only interacts with Azure Tables—it does not use other storage account features such as Blobs, 
> Queues, or Files.
{: .block-note}

A typical Azure Storage connection string looks like this:
```text
DefaultEndpointsProtocol=https;AccountName=your_account_name;AccountKey=your_account_key;TableEndpoint=https://your_account_name.table.core.windows.net/;
```