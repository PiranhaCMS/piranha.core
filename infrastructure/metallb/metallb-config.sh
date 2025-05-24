cidr_block=$(docker network inspect k3d-cluster | jq '. [0].IPAM.Config[0].Subnet' | tr -d '"');
base_addr=${cidr_block%???};
first_addr=$(echo $base_addr | awk -F'.' '{print $1,$2,$3,240}' OFS='.');
range=$first_addr/29;
cat <<EOF | kubectl apply -f -
# This file configures MetalLB with a specific IP address pool and L2 advertisement.
apiVersion: metallb.io/v1beta1
kind: IPAddressPool
metadata:
  namespace: metallb-system
  name: my-ip-pool
spec:
  addresses:
  - $range
---
apiVersion: metallb.io/v1beta1
kind: L2Advertisement
metadata:
  namespace: metallb-system
  name: l2-advertisement
spec:
  ipAddressPools:
  - my-ip-pool

EOF
