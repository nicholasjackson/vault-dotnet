#!/bin/sh

vault secrets enable database

vault write database/config/fighters \
    plugin_name=postgresql-database-plugin \
    allowed_roles="*" \
    connection_url="postgresql://{{username}}:{{password}}@postgres.container.jumppad.dev:5432/Fighters-6558502e-64dd-43fe-99d8-b519b4408469?sslmode=disable" \
    username="postgres" \
    password="password"

vault write database/roles/fighters \
  db_name=fighters \
  creation_statements="CREATE ROLE \"{{name}}\" WITH LOGIN PASSWORD '{{password}}' VALID UNTIL '{{expiration}}'; \
      GRANT ALL ON SCHEMA public TO \"{{name}}\";" \
  revocation_statements="ALTER ROLE \"{{name}}\" NOLOGIN; \
      SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.usename = '{{name}}';" \
  default_ttl="1m" \
  max_ttl="5m"

cat <<EOF > ./policy.hcl
path "database/creds/fighters" {
  capabilities = ["update", "read"]
}

path "/auth/token/lookup-self" {
  capabilities = ["read"]
}
EOF

vault policy write fighters ./policy.hcl

vault auth enable approle

vault write auth/approle/role/fighters \
    secret_id_ttl=2h \
    token_max_ttl=2h \
    policies=fighters

vault read --format=json \
  auth/approle/role/fighters/role-id \
  | sed -n 's/.*"role_id": "\(.*\)".*/\1/p' \
  | tr -d '\n' \
  > /output/.role-id

vault write -f --format=json \
  auth/approle/role/fighters/secret-id \
  | sed -n 's/.*"secret_id": "\(.*\)".*/\1/p' \
  | tr -d '\n' \
  > /output/.secret-id