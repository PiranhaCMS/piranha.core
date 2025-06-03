NAMESPACE=control
SECRET_NAME="tenantmanagement-env-secret"
ENV_FILE_PATH="infrastructure/secrets/.env.tenantmanagement"
DOCKER_IMAGE="k3d-registry:5000/contentrus-tenantmanagement"

echo "Getting .env file"
if [ ! -f "$ENV_FILE_PATH" ]; then
  echo ".env file not found at $ENV_FILE_PATH"
  exit 1
fi

echo "Pulling Docker image from registry..."
docker pull "$DOCKER_IMAGE"
if [ $? -ne 0 ]; then
  echo "Failed to pull the image from the registry. Exiting."
  exit 1
fi

ENV_CONTENT=$(cat "$ENV_FILE_PATH")

kubectl create secret generic $SECRET_NAME --from-literal=.env="$ENV_CONTENT" -n $NAMESPACE