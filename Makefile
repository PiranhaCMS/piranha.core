REGISTRY_NAME = registry
REGISTRY_PORT = 5000
CLUSTER_NAME = cluster

.PHONY: all create-registry create-cluster kubeconfig set-as-default-kubeconfig clean-registry clean-cluster clean-all restart-docker

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
		helm repo update; \
		helm install --set cr.create=true --set cr.namespace=common --set cr.spec.auth.strategy="anonymous" --namespace common kiali-operator kiali/kiali-operator; \
		kubectl apply -f infrastructure/kiali/kiali.yaml; \
		helm install kube-prometheus-stack --namespace common prometheus-community/kube-prometheus-stack; \
		helm install opentelemetry infrastructure/3p-charts/opentelemetry -n common --wait; \
		helm upgrade kube-prometheus-stack prometheus-community/kube-prometheus-stack   -n common   -f infrastructure/grafana/grafana-values.yaml; \
		$(MAKE) download-istio-dashboards; \
		$(MAKE) load-grafana-dashboards; \
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