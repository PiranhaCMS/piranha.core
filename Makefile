# Add your credentials to k3d-registries.yaml to not get rate-limited when pulling images from Docker Hub.

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
		helm install istio-ingress istio/gateway -n tcommon --create-namespace --wait; \
	else \
		echo "Cluster '$(CLUSTER_NAME)' already exists."; \
	fi

set-as-default-kubeconfig:
	@echo "Merging kubeconfig and setting it as default at ~/.kube/config..."
	k3d kubeconfig merge $(CLUSTER_NAME) --kubeconfig-switch-context
	cp ~/.config/k3d/kubeconfig-$(CLUSTER_NAME).yaml ~/.kube/config

clean-registry:
	@echo "Deleting registry '$(REGISTRY_NAME)' on port $(REGISTRY_PORT)..."
	k3d registry delete $(REGISTRY_NAME) || true

clean-cluster:
	@echo "Deleting cluster '$(CLUSTER_NAME)'..."; \
	helm ls -a --all-namespaces | awk 'NR > 1 { print  "-n "$2, $1}' | xargs -L1 helm delete
	k3d cluster delete $(CLUSTER_NAME) || true

clean-all:
	@echo "Deleting cluster '$(CLUSTER_NAME)' and registry '$(REGISTRY_NAME)' (port $(REGISTRY_PORT))..."
	helm ls -a --all-namespaces | awk 'NR > 1 { print  "-n "$2, $1}' | xargs -L1 helm delete
	k3d cluster delete $(CLUSTER_NAME) || true
	k3d registry delete $(REGISTRY_NAME) || true

restart-docker:
	@echo "Restarting Docker..."
	sudo systemctl restart docker || true