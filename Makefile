REGISTRY_NAME = registry
REGISTRY_PORT = 5000
CLUSTER_NAME = cluster
REGISTRY_URL = k3d-$(REGISTRY_NAME):$(REGISTRY_PORT)

# Image definitions
IMAGES = contentrus-tenantmanagement contentrus-billing contentrus-selfprovision contentrus-notificationservice
IMAGE_DOCKERFILES = infrastructure/Dockerfile.tenantmanagement infrastructure/Dockerfile.billing infrastructure/Dockerfile.selfprovision infrastructure/Dockerfile.notificationservice

.PHONY: all create-registry create-cluster kubeconfig set-as-default-kubeconfig clean-registry clean-cluster clean-all restart-docker build-images check-images


all: check-k3d docker-set-default

check-k3d:
	@command -v k3d >/dev/null 2>&1 || { \
		echo >&2 "❌ k3d is not installed. Please install it: https://k3d.io/"; exit 1; \
	}
	@echo "✅ k3d is installed."

docker-set-default:
	@echo "Setting Docker default context..."
	docker context use default

create-registry:
	@if ! k3d registry list | grep -q $(REGISTRY_NAME); then \
		echo "Creating registry '$(REGISTRY_NAME)'..."; \
		k3d registry create $(REGISTRY_NAME) --port $(REGISTRY_PORT); \
	else \
		echo "Registry '$(REGISTRY_NAME)' already exists."; \
	fi

check-images:
	@echo "Checking if images exist in registry..."
	@missing_images=""; \
	for image in $(IMAGES); do \
		if ! docker pull $(REGISTRY_URL)/$$image:latest >/dev/null 2>&1; then \
			echo "❌ Image $$image not found in registry"; \
			missing_images="$$missing_images $$image"; \
		else \
			echo "✅ Image $$image found in registry"; \
		fi; \
	done; \
	if [ -n "$$missing_images" ]; then \
		echo "Building missing images..."; \
		$(MAKE) build-images MISSING_IMAGES="$$missing_images"; \
	else \
		echo "✅ All images are available in registry"; \
	fi

build-images:
	@echo "Building and pushing images..."
	@if echo "$(MISSING_IMAGES)" | grep -q "contentrus-tenantmanagement" || [ -z "$(MISSING_IMAGES)" ]; then \
		echo "Building Tenant Management..."; \
		docker build -f infrastructure/Dockerfile.tenantmanagement -t $(REGISTRY_URL)/contentrus-tenantmanagement:latest .; \
		docker push $(REGISTRY_URL)/contentrus-tenantmanagement:latest; \
	fi
	@if echo "$(MISSING_IMAGES)" | grep -q "contentrus-billing" || [ -z "$(MISSING_IMAGES)" ]; then \
		echo "Building Billing Service..."; \
		docker build -f infrastructure/Dockerfile.billing -t $(REGISTRY_URL)/contentrus-billing:latest .; \
		docker push $(REGISTRY_URL)/contentrus-billing:latest; \
	fi
	@if echo "$(MISSING_IMAGES)" | grep -q "contentrus-selfprovision" || [ -z "$(MISSING_IMAGES)" ]; then \
		echo "Building Self Provision UI..."; \
		docker build -f infrastructure/Dockerfile.selfprovision -t $(REGISTRY_URL)/contentrus-selfprovision:latest .; \
		docker push $(REGISTRY_URL)/contentrus-selfprovision:latest; \
	fi
	@if echo "$(MISSING_IMAGES)" | grep -q "contentrus-notificationservice" || [ -z "$(MISSING_IMAGES)" ]; then \
		echo "Building Notifications Service..."; \
		docker build -f infrastructure/Dockerfile.notificationservice -t $(REGISTRY_URL)/contentrus-notificationservice:latest .; \
		docker push $(REGISTRY_URL)/contentrus-notificationservice:latest; \
	fi
	@echo "✅ All images built and pushed successfully"

create-cluster:
	@echo "Creating cluster '$(CLUSTER_NAME)'..."
	@if ! k3d cluster list | grep -q $(CLUSTER_NAME); then \
		k3d cluster create $(CLUSTER_NAME) \
			--registry-use k3d-$(REGISTRY_NAME):$(REGISTRY_PORT) \
			--k3s-arg "--disable=traefik@server:0" \
			--k3s-arg "--disable=servicelb@server:0" --no-lb \
			--k3s-arg '--flannel-backend=none@server:*' \
			--k3s-arg '--disable-network-policy@server:*' \
			--k3s-arg '--cluster-cidr=192.168.0.0/16@server:*' \
			--wait; \
		k3d kubeconfig merge $(CLUSTER_NAME) --kubeconfig-switch-context; \
		$(MAKE) check-images; \
		docker pull docker.io/calico/csi:v3.29.0; \
		docker pull docker.io/calico/node-driver-registrar:v3.29.0; \
		docker pull docker.io/calico/pod2daemon-flexvol:v3.29.0; \
		docker pull docker.io/calico/typha:v3.29.0; \
		docker pull docker.io/calico/cni:v3.29.0; \
		docker pull docker.io/calico/node:v3.29.0; \
		docker pull docker.io/calico/apiserver:v3.29.0; \
		k3d image import calico/node-driver-registrar:v3.29.0 -c $(CLUSTER_NAME); \
		k3d image import calico/pod2daemon-flexvol:v3.29.0 -c $(CLUSTER_NAME); \
		k3d image import calico/csi:v3.29.0 -c $(CLUSTER_NAME); \
		k3d image import calico/typha:v3.29.0 -c $(CLUSTER_NAME); \
		k3d image import calico/cni:v3.29.0 -c $(CLUSTER_NAME); \
		k3d image import calico/node:v3.29.0 -c $(CLUSTER_NAME); \
		k3d image import calico/apiserver:v3.29.0 -c $(CLUSTER_NAME); \
		kubectl create -f https://raw.githubusercontent.com/projectcalico/calico/v3.29.0/manifests/tigera-operator.yaml; \
		kubectl create -f https://raw.githubusercontent.com/projectcalico/calico/v3.29.0/manifests/custom-resources.yaml; \
		kubectl patch installation default --type=merge --patch='{"spec":{"calicoNetwork":{"containerIPForwarding":"Enabled"}}}'; \
		kubectl apply -f https://raw.githubusercontent.com/metallb/metallb/v0.14.9/config/manifests/metallb-native.yaml; \
		kubectl -n metallb-system wait --for=condition=ready pod -l app=metallb --timeout=90s; \
		bash infrastructure/3p-charts/metallb/metallb-config.sh; \
		helm repo add istio https://istio-release.storage.googleapis.com/charts; \
		helm repo update; \
		helm install istio-base istio/base -n istio-system --set defaultRevision=default --create-namespace; \
		helm install istiod istio/istiod -n istio-system --wait; \
		helm install istio-ingress istio/gateway -n common --create-namespace --wait; \
		helm repo add argo https://argoproj.github.io/argo-helm; \
		helm repo update; \
		helm install argowf argo/argo-workflows -n argowf -f infrastructure/argo/argowf/setup/wf-values.yml --create-namespace; \
		kubectl create rolebinding default-admin --clusterrole=admin --serviceaccount=argowf:default -n argowf; \
		kubectl create clusterrole secret-writer --verb=create --resource=secrets; \
		kubectl create clusterrolebinding secret-writer-binding --clusterrole=secret-writer --serviceaccount=argowf:default; \
		kubectl create clusterrole namespace-creator --verb=create --resource=namespaces; \
		kubectl create clusterrolebinding namespace-creator-binding --clusterrole=namespace-creator --serviceaccount=argowf:default; \
		kubectl create secret generic github-creds --from-file=ssh-private-key=credentials/id_ed25519 -n argowf; \
		kubectl create secret generic azure-cred-secret --from-file=username=credentials/azure_username.txt --from-file=password=credentials/azure_password.txt -n argowf; \
		helm install argocd argo/argo-cd -n argocd -f infrastructure/argo/argowf/setup/cd-values.yml --create-namespace; \
		kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/notifications_catalog/install.yaml; \
		kubectl apply -f infrastructure/argo/argocd/deploy-status-notification.yaml -n argocd; \
		kubectl apply -f infrastructure/argo/argocd/project.yaml; \
		kubectl apply -f infrastructure/argo/argocd/tenants-application-set.yaml; \
		kubectl apply -f infrastructure/argo/argowf/tenant-provisioning-with-credentials-template.yml -n argowf; \
		kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.17.2/cert-manager.yaml; \
		kubectl -n cert-manager wait --for=condition=ready pod -l app.kubernetes.io/name=webhook --timeout=120s; \
		kubectl -n cert-manager wait --for=condition=ready pod -l app.kubernetes.io/name=cainjector --timeout=60s; \
		kubectl -n cert-manager wait --for=condition=ready pod -l app.kubernetes.io/name=cert-manager --timeout=60s; \
		sleep 10; \
		kubectl apply -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml || true; \
		sleep 5; \
		kubectl apply -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml; \
		kubectl -n opentelemetry-operator-system wait --for=condition=ready pod -l app.kubernetes.io/name=opentelemetry-operator-webhook --timeout=120s; \
		helm repo add prometheus-community https://prometheus-community.github.io/helm-charts; \
		helm repo add kiali https://kiali.org/helm-charts; \
		helm repo add kong https://charts.konghq.com; \
		helm repo update; \
		helm install --set cr.create=true --set cr.namespace=common --set cr.spec.auth.strategy="anonymous" --namespace common kiali-operator kiali/kiali-operator; \
		kubectl apply -f infrastructure/kiali/kiali.yaml; \
		helm install kube-prometheus-stack --namespace common prometheus-community/kube-prometheus-stack; \
		helm install opentelemetry infrastructure/3p-charts/opentelemetry -n common --wait; \
		helm upgrade kube-prometheus-stack prometheus-community/kube-prometheus-stack   -n common   -f infrastructure/grafana/grafana-values.yaml; \
		$(MAKE) download-istio-dashboards; \
		$(MAKE) load-grafana-dashboards; \
		kubectl create namespace control; \
		sh infrastructure/secrets/tenantmanagement-mysql-env-secret-create.sh; \
		sh infrastructure/secrets/tenantmanagement-env-secret-create.sh; \
		sh infrastructure/secrets/stripe-env-secret-create.sh; \
		sh infrastructure/secrets/frontend-env-secret-create.sh; \
		sh infrastructure/secrets/billing-env-secret-create.sh; \
		helm install mysql oci://registry-1.docker.io/bitnamicharts/mysql -f infrastructure/3p-charts/mysql/values-tenantmanagement.yaml -n control --set namespaceOverride=control; \
		helm install rabbitmq bitnami/rabbitmq -n control --set auth.username=user --set auth.password=password; \
		kubectl apply -f https://github.com/kubernetes-sigs/gateway-api/releases/download/v1.1.0/standard-install.yaml -n control; \
		kubectl apply -f infrastructure/custom-charts/kong/gatewayclass.yaml -n control; \
		kubectl apply -f infrastructure/custom-charts/kong/gateway.yaml -n control; \
		kubectl apply -f infrastructure/custom-charts/kong/cors.yaml -n control; \
		helm install kong kong/ingress -n control; \
		kubectl apply -f infrastructure/custom-charts/ingress/ingress.yaml -n control; \
		kubectl apply -f infrastructure/custom-charts/stripe/role.yaml -n control; \
		kubectl apply -f infrastructure/custom-charts/stripe/rolebinding.yaml -n control; \
		kubectl apply -f infrastructure/custom-charts/stripe/configmap.yaml -n control; \
		kubectl apply -f infrastructure/custom-charts/stripe/stripe.yaml -n control; \
		helm install tenantmanagement -n control infrastructure/custom-charts/tenantmanagement/; \
		helm install billing -n control infrastructure/custom-charts/billing/; \
		helm install selfprovisionui -n control infrastructure/custom-charts/selfprovisionui/; \
		helm install notificationservice -n control infrastructure/custom-charts/notifications/; \
	else \
		echo "Cluster '$(CLUSTER_NAME)' already exists."; \
	fi

load-grafana-dashboards:
	@echo "Loading Grafana dashboards..."
	@find infrastructure/grafana/grafana-dashboards -name "*.json" | while read file; do \
		dashboard_name=$$(basename $$file .json); \
		configmap_name=$$(basename $$file .json | tr '[:upper:]' '[:lower:]' | tr '.' '-'); \
		echo "Creating ConfigMap for dashboard: $$dashboard_name"; \
		kubectl create configmap "grafana-dashboard-$$configmap_name" \
			--from-file="$$dashboard_name.json=$$file" \
			-n common \
			--dry-run=client -o yaml | \
		kubectl apply -f -; \
		kubectl label configmap "grafana-dashboard-$$configmap_name" \
			grafana_dashboard=1 -n common --overwrite; \
	done

download-istio-dashboards:
	@echo "Downloading Istio dashboards for Grafana..."
	@mkdir -p infrastructure/grafana/grafana-dashboards/istio
	@if [ ! -f infrastructure/grafana/grafana-dashboards/istio/istio-mesh-dashboard.json ]; then \
		echo "Downloading istio-mesh-dashboard.json"; \
		curl -s -o infrastructure/grafana/grafana-dashboards/istio/istio-mesh-dashboard.json https://raw.githubusercontent.com/istio/istio/master/manifests/addons/dashboards/istio-mesh-dashboard.gen.json; \
	else \
		echo "istio-mesh-dashboard.json already exists, skipping download"; \
	fi
	@if [ ! -f infrastructure/grafana/grafana-dashboards/istio/istio-service-dashboard.json ]; then \
		echo "Downloading istio-service-dashboard.json"; \
		curl -s -o infrastructure/grafana/grafana-dashboards/istio/istio-service-dashboard.json https://raw.githubusercontent.com/istio/istio/master/manifests/addons/dashboards/istio-service-dashboard.json; \
	else \
		echo "istio-service-dashboard.json already exists, skipping download"; \
	fi
	@if [ ! -f infrastructure/grafana/grafana-dashboards/istio/istio-workload-dashboard.json ]; then \
		echo "Downloading istio-workload-dashboard.json"; \
		curl -s -o infrastructure/grafana/grafana-dashboards/istio/istio-workload-dashboard.json https://raw.githubusercontent.com/istio/istio/master/manifests/addons/dashboards/istio-workload-dashboard.json; \
	else \
		echo "istio-workload-dashboard.json already exists, skipping download"; \
	fi
	@if [ ! -f infrastructure/grafana/grafana-dashboards/istio/istio-control-plane-dashboard.json ]; then \
		echo "Downloading istio-control-plane-dashboard.json"; \
		curl -s -o infrastructure/grafana/grafana-dashboards/istio/istio-control-plane-dashboard.json https://grafana.com/api/dashboards/7645/revisions/259/download; \
	else \
		echo "istio-control-plane-dashboard.json already exists, skipping download"; \
	fi
	@if [ ! -f infrastructure/grafana/grafana-dashboards/istio/istio-performance-dashboard.json ]; then \
		echo "Downloading istio-performance-dashboard.json"; \
		curl -s -o infrastructure/grafana/grafana-dashboards/istio/istio-performance-dashboard.json https://raw.githubusercontent.com/istio/istio/master/manifests/addons/dashboards/istio-performance-dashboard.json; \
	else \
		echo "istio-performance-dashboard.json already exists, skipping download"; \
	fi
	@if [ ! -f infrastructure/grafana/grafana-dashboards/istio/ztunnel-dashboard.json ]; then \
		echo "Downloading ztunnel-dashboard.json (replaces wasm extension dashboard)"; \
		curl -s -o infrastructure/grafana/grafana-dashboards/istio/ztunnel-dashboard.json https://raw.githubusercontent.com/istio/istio/master/manifests/addons/dashboards/ztunnel-dashboard.gen.json; \
	else \
		echo "ztunnel-dashboard.json already exists, skipping download"; \
	fi
	@echo "✅ Istio dashboards available."

set-as-default-kubeconfig:
	@echo "Merging kubeconfig and setting it as default at ~/.kube/config..."
	k3d kubeconfig merge $(CLUSTER_NAME) --kubeconfig-switch-context
	cp ~/.config/k3d/kubeconfig-$(CLUSTER_NAME).yaml ~/.kube/config

clean-registry:
	@echo "Deleting registry '$(REGISTRY_NAME)' on port $(REGISTRY_PORT)..."
	k3d registry delete $(REGISTRY_NAME) || true

clean-cluster:
	@echo "Deleting cluster '$(CLUSTER_NAME)'..."; \
	k3d cluster delete $(CLUSTER_NAME) || true

clean-all:
	@echo "Deleting cluster '$(CLUSTER_NAME)' and registry '$(REGISTRY_NAME)' (port $(REGISTRY_PORT))..."
	k3d cluster delete $(CLUSTER_NAME) || true
	k3d registry delete $(REGISTRY_NAME) || true

restart-docker:
	@echo "Restarting Docker..."
	sudo systemctl restart docker || true