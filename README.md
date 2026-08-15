# Binacle.Net

## 📝 Overview
Binacle.Net is an API created to address the 3D Bin Packing Problem in real time.

It is an ideal fit for e-commerce platforms offering parcel shipments to self-service locker systems,
providing optimal bin packing calculations to ensure efficient use of space and smooth customer experiences during checkout.

## 🚀 Quick Start
Simply execute the following command in your terminal:

```bash
docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -e UI_MODULE=True -e SCALAR_UI=True binacle/binacle-net:latest
```

> `latest` follows the newest release, and a new major version can break your integration. It is the right tag
> for trying Binacle.Net out. **Pin a version for anything you keep** - `binacle/binacle-net:3.0` will track
> fixes within the 3.0 line without ever changing behaviour under you, once v3.0.0 is published. Until then, pin
> `binacle/binacle-net:3.0.0-beta.1`.

Images from `3.0.0-beta.2` onward are signed and carry a bill of materials -
**[SECURITY.md](SECURITY.md#verifying-a-release)** has the two commands that check what you pulled.
### 🌐 Access the Interface
- Swagger UI (API Documentation): http://localhost:8080/swagger/
- Scalar UI (Alternative to Swagger): http://localhost:8080/scalar/
- UI Module & Packing Demo: http://localhost:8080/

Start exploring Binacle.Net now! 🚀

## 📂 Repository Structure

```text
/Binacle.Net      # Root directory
├── /api          # HTTP API — ASP.NET Core minimal APIs (v3, v4) and modules
├── /lib          # Core 3D bin-packing engine (Binacle.Lib)
├── /vipaq        # ViPaq — compact binary format for packing results
├── /shared       # Shared test kernel and benchmark data
├── /packages     # JavaScript/TypeScript packages (npm workspaces)
├── /ruby         # Ruby gems — Jekyll plugins for the docs and web sites
├── /docs         # Documentation site (Jekyll)
├── /web          # Binacle.Net website (Jekyll)
├── /samples      # Docker Compose and Kubernetes deployment samples
├── /tooling      # Every task the repo can run - just modules, scripts, local compose
├── /assets       # Shared static assets copied into the sites at build time
├── /artifacts    # Build output - published app, sites, OpenAPI, test results, coverage
└── /results      # Benchmark and packing-efficiency output
```

Each slice folder has its own `README.md` with details.

## 🛠️ Building from source

You need the .NET SDK, Node, Ruby, `just` and - for the container image only - Docker.
**[DEVELOPMENT.md](DEVELOPMENT.md)** has the versions, the pin files and the install commands.

```bash
just install                     # npm workspaces, both sites' gems, then the asset copy
just test all                    # every suite that needs nothing brought up
just build image                 # publish, then tag binacle-net:local
```

`just` with no arguments lists every task.

## 📄 License

This work is dual-licensed under the GNU General Public License v3.0 and the Creative Commons Attribution-ShareAlike 4.0 International License (CC BY-SA 4.0).

`SPDX-License-Identifier: GPL-3.0 AND CC-BY-SA-4.0`

### Code
The code in this project is licensed under the GNU General Public License v3.0. <br/>

See the [LICENSE.GPL-3.0](LICENSE.GPL-3.0) file for details.

### Documentation and Content
All documentation, images, and other content files in this project are licensed under the
[Creative Commons Attribution-ShareAlike 4.0 International License (CC BY-SA 4.0)](https://creativecommons.org/licenses/by-sa/4.0/).

See [LICENSE.CC-BY-SA-4.0](LICENSE.CC-BY-SA-4.0) for the full license text.

### Third-Party Libraries
Binacle.Net uses third-party libraries and dependencies. 

See the [NOTICE](NOTICE) file for complete attribution details.

## Security
See [SECURITY.md](SECURITY.md) for my security policy and how to report vulnerabilities.

---

Copyright (c) 2023-2026 Chris Mavrommatis. All rights reserved.
