path "database/creds/fighters" {
  capabilities = ["update", "read"]
}

path "/auth/token/lookup-self" {
  capabilities = ["read"]
}
