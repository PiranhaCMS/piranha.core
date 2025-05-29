Currently ArgoCD is in non HA mode (to save resources)

After port forwarding
argocd login localhost:32887 --insecure

Username: admin
Password: check secret argocd-initial-admin-secret

----------------------------------------------------------------------------

OutOfSync -> target state differs from live state
kubectl get all -n t1