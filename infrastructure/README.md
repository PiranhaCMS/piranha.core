# Infrastructure Documentation

This directory contains the Kubernetes and infrastructure configuration for the multi-tenant application setup.

## Directory Overview

### Docker Configuration
- `Dockerfile.hostedsite`: Docker configuration for the hosted site application
- `Dockerfile.manager`: Docker configuration for the manager application
- `Makefile`: Contains build and deployment automation scripts

### Helm Charts
- `3p-charts/`: Third-party Helm charts
    - `mysql/`: MySQL database configuration charts
- `custom-charts/`: Custom application Helm charts
    - `hostedsite/`: Helm chart for the hosted site application
    - `manager/`: Helm chart for the manager application


## Building & Pushing Docker Images

**MUST**: From the root directory of this repo

### Build & Push Hosted Site
```bash
docker build -f infrastructure/Dockerfile.hostedsite -t k3d-registry:5000/contentrus-hostedsite .
```
```bash
docker push k3d-registry:5000/contentrus-hostedsite
```

### Build & Push Hosted Manager
```bash
docker build -f infrastructure/Dockerfile.manager -t k3d-registry:5000/contentrus-manager .
```
```bash
docker push k3d-registry:5000/contentrus-manager
```

