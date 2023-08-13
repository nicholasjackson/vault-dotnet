#!/bin/bash

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
      GRANT SELECT ON ALL TABLES IN SCHEMA public TO \"{{name}}\";" \
  revocation_statements="ALTER ROLE \"{{name}}\" NOLOGIN;"\
  default_ttl="1h" \
  max_ttl="24h"

vault policy write fighters ./policy.hcl

vault auth enable approle

vault write auth/approle/role/fighters \
    secret_id_ttl=2h \
    token_max_ttl=2h \
    policies=fighters

vault read --format=json \
  auth/approle/role/fighters/role-id \
  | jq -r '.data.role_id' \
  > .role-id

vault write -f --format=json \
  auth/approle/role/fighters/secret-id \
  | jq -r '.data.secret_id' \
  > .secret-id