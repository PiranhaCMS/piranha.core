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

helm install mysql oci://registry-1.docker.io/bitnamicharts/mysql -f infrastructure/3p-charts/mysql/values-tenantmanagement.yaml -n tcommon --set namespaceOverride=tcommon

rabbit

helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

helm install rabbitmq bitnami/rabbitmq -n tcommon \
  --set auth.username=user \
  --set auth.password=password


helm repo add kong https://charts.konghq.com
helm repo update

kubectl apply -f https://github.com/kubernetes-sigs/gateway-api/releases/download/v1.1.0/standard-install.yaml -n tcommon


# helm install kong kong/kong --namespace tcommon \
#  --set ingressController.installCRDs=false \
#  --set proxy.type=LoadBalancer
kubectl apply -f infrastructure/custom-charts/kong/gatewayclass.yaml

kubectl apply -f infrastructure/custom-charts/kong/gateway.yaml

--
helm install selfprovisionui -n tcommon infrastructure/custom-charts/selfprovisionui/

helm install tenantmanagement -n tcommon infrastructure/custom-charts/tenantmanagement/
--

helm install kong kong/ingress -n tcommon

kubectl apply -f infrastructure/custom-charts/ingress/ingress.yaml -n tcommon

sh infrastructure/secrets/tenantmanagement-env-secret-create.sh
sh infrastructure/secrets/stripe-env-secret-create.sh
sh infrastructure/secrets/frontend-env-secret-create.sh
sh infrastructure/secrets/billing-env-secret-create.sh




-----------

kubectl get svc -n common


--- stripe

kubectl apply -f infrastructure/custom-charts/stripe/role.yaml -n control
kubectl apply -f infrastructure/custom-charts/stripe/rolebinding.yaml -n control
kubectl apply -f infrastructure/custom-charts/stripe/configmap.yaml -n control
kubectl apply -f infrastructure/custom-charts/stripe/stripe.yaml -n control