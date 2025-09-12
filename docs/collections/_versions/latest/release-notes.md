---
title: Release Notes v2.0.0
nav:
  order: 2
  icon: 🛠️
---

Binacle.Net Version 2.0 is a major update from v1.3.0. 

This page summarizes key changes, improvements, and migration tips.

> v2.0.0 introduces breaking changes and new features. Existing integrations must be reviewed and updated.
{: .block-warning}

---


---


## 🚀 Major Changes

Service module was redesigned and restructured, breaking any existing integrations using it. 
Documentation for the service module is no longer publicly available.


Service Module changed... now its'\s no logner available to the public with documentation
now instead of users we have accounts and subscriptions
Any existing integration using the service module can no longer work as they were.


This was done because the service module was primarily designed for Binacle.Net to be used as a SaaS product.

Because the official site is public and allows anyone to test it,
I wanted to limit the number of requests anonymous users could do to avoid abuse.
Since I am paying for the hosting and the API calls, I wanted to avoid unexpected costs.

Binacle.Net will continue to operate as a service and allow anonymous users to make requests.
It stil retains the same global limit for all users although I may change this in the future.

Howerver now It enables me to issue accounts with 'demo' subscriptions that have their own limits.
This way I can give out accounts to people who want to test the service more extensively without worrying about abuse.

It also allows me to issue normal subscriptions in case someone helps me with hosting costs and or cover compute /cloud costs.

All this increases the complexity of the service module, increasing the complexity and effort of mantaining the documentation.
From this version onwards the service module will no longer be documented publicly. Additionally any breaking changes in the service module will not be documented nor increase the major version number.
Any companies who want to use Binacle.Net as a service should contact me directly.

It still remains part of the open source project, so anyone can still run their own instance of Binacle.Net and use the service module, however no documentation will be provided for it.


All api calls were rewriten and now all API documentation is available through the OpenAPI Standard.
Swagger UI is still included but now works with Open API Spec.
Added Scalar UI for easier testing of the API.
So some minor changes were made to the API endpoints, but it should be backward compatible.
Only The documentation should have changes, I tried to keep it as same as i could.


Texts wioth xunit v3


REmoved the V1 endpoints and all the old code related to it.

UI module added the vendor  css.js inside the docker image, so it wont rely on the cdn 

Some minor fixes and improvements to the UI module

woirked on new documentation site to preserve documentation for each version.

improoved FFS and WFD algorithms


packjing logs removed leggacy fitting and legacy packing, it is now packing and fitting as they have been merged