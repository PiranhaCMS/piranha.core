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

### Build & Push Tenant Management
```bash
docker build -f infrastructure/Dockerfile.tenantmanagement -t k3d-registry:5000/contentrus-tenantmanagement .
```
```bash
docker push k3d-registry:5000/contentrus-tenantmanagement
```

### Build & Push Billing Service
```bash
docker build -f infrastructure/Dockerfile.billing -t k3d-registry:5000/contentrus-billing .
```
```bash
docker push k3d-registry:5000/contentrus-billing
```

### Build & Push Self Provision UI
```bash
docker build -f infrastructure/Dockerfile.selfprovision -t k3d-registry:5000/contentrus-selfprovision .
```
```bash
docker push k3d-registry:5000/contentrus-selfprovision
```

### Build and Push Notifications Service
```bash
docker build -f infrastructure/Dockerfile.notificationservice -t k3d-registry:5000/contentrus-notificationservice .
```
```bash
docker push k3d-registry:5000/contentrus-notificationservice
```

### Build and Push Onboarding Service
```bash
docker build -f infrastructure/Dockerfile.onboarding -t k3d-registry:5000/contentrus-onboarding .
```
```bash
docker push k3d-registry:5000/contentrus-onboarding
```