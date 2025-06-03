NAMESPACE=control
SECRET_NAME="tenantmanagement-mysql-env-secret"
ENV_FILE_PATH="infrastructure/secrets/.env.tenantmanagement-mysql"

export $(grep -v '^#' $ENV_FILE_PATH | xargs)

kubectl create secret generic $SECRET_NAME\
  --from-literal=mysql-root-password="$mysql_root_password" \
  --from-literal=mysql-user="$mysql_user" \
  --from-literal=mysql-password="$mysql_password" \
  --from-literal=mysql-replication-password="$mysql_replication_password" \
  -n $NAMESPACE