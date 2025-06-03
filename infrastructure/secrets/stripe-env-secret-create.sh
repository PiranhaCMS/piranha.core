NAMESPACE="control"

export $(grep -v '^#' infrastructure/secrets/.env.billing | xargs)

kubectl create secret generic stripe-env-secret \
  --from-literal=STRIPE_API_KEY="$Stripe__SecretKey" \
  -n "$NAMESPACE"