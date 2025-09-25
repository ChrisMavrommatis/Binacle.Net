---
title: Service Module
permalink: /version/latest/configuration/service-module/
nav:
  parent: Configuration
  order: 3
  icon: 🛡️
---


The **Service Module** of Binacle.Net has undergone a complete redesign and restructuring,  
introducing fundamental changes that break compatibility with all existing integrations.

⚠️ **Public documentation for the Service Module is no longer available.**

## 🔍 Background and Motivation

Originally, the Service Module was developed to support Binacle.Net as a Software-as-a-Service (SaaS) platform.  
Since the official site is publicly accessible and allows everyone to test the service,  
there was an urgent need to manage resource consumption and prevent abuse from anonymous users.

💡 Because all hosting and API call costs are personally borne, safeguarding against unexpected expenses became a priority.  
This led to a reimagining of how access and usage are managed.

## 🔄 Transition to Accounts and Subscriptions

A key change in the redesigned Service Module is the shift from anonymous and registered users to a  
structured **accounts and subscriptions** system:

### 👤 Anonymous Access
Anonymous users can still make requests to Binacle.Net and are subject 
to a fixed **global throttling limit** that applies collectively to all anonymous traffic.

### 🗝️ Accounts
The Admin can issue Accounts and Subscriptions, but to access the service beyond anonymous limits,
each account must have an active subscription associated with it.

### 📜 Subscriptions
There are two subscription types available:

#### 🆓 Demo Subscription
Provides individual rate limits specific to each account,  
allowing more generous usage compared to anonymous access, without sharing global limits.

#### 💼 Normal Subscription
Intended for supporters who contribute towards hosting or cloud costs.

These subscriptions are **not subject to rate limits**, enabling unrestricted usage and fostering sustainable growth of the service.

## 📚 Impact on Documentation and Maintenance

With this change, the Service Module’s architecture and usage complexity have increased significantly.  
Maintaining comprehensive and up-to-date public documentation for it is no longer feasible or practical.

**Hence:**

- 🚫 From this release forward, **no public documentation will be provided for the Service Module**.
- 🔕 Breaking changes will **not** be documented publicly nor trigger a major version increment.
- 🤝 Organizations interested in commercial integration or extensive Service Module use 
  should **contact directly** for private support and collaboration agreements.

## 🔓 Open Source Availability

The Service Module remains an open-source component within the Binacle.Net project. This allows anyone to:

- 🛠️ Clone and run their own instance of Binacle.Net.
- ⚙️ Utilize the Service Module fully within their self-managed environment.

However, since no official documentation or support is offered for the Service Module anymore, 
**self-hosted users must rely exclusively on the source code and community resources** for implementation and troubleshooting.

---

⚙️ This shift allows me to dedicate less time maintaining the Service Module and its documentation,
freeing up valuable resources to focus on advancing the core product.

By streamlining service management, Binacle.Net can continue growing sustainably while still providing access  
to advanced features through accounts and subscriptions.