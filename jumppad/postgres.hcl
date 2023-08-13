resource "container" "postgres" {
  image {
    name = "postgres"
  }

  port {
    local  = 5432
    remote = 5432
    host   = 5432
  }

  environment = {
    POSTGRES_PASSWORD = variable.postgres_password
    POSTGRES_DB       = variable.postgres_db
  }

  network {
    id = variable.vault_network
  }
}