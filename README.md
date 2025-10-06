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
### 🌐 Access the Interface
- Swagger UI (API Documentation): http://localhost:8080/swagger/
- Scalar UI (Alternative to Swagger): http://localhost:8080/scalar/
- UI Module & Packing Demo: http://localhost:8080/

Start exploring Binacle.Net now! 🚀

## Repository Structure

```text
/Binacle.Net  # Root directory
├── /build              # Build scripts, generated files and output artifacts
├── /config             # Configuration files for running the API, Benchmarks, Tests, Docs locally
├── /dep                # Dependency Projects Not part of Binacle.Net, Binacle.Lib or Binacle.ViPaq
├── /docs               # Documentation Site
├── /doc                # Documentation files
├── /res                # Resources (Http Requests, etc)
├── /samples            # Example Projects for running Binacle.Net 
├── /src                # Source Code
├── /test               # Test Projects
```