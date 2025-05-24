helm install istio-base istio/base -n istio-system --set defaultRevision=default --create-namespace
helm install istiod istio/istiod -n istio-system --wait

kubectl create namespace tcommon
helm install istio-ingress istio/gateway -n tcommon --wait

kubectl -n tcommon get svc istio-ingress