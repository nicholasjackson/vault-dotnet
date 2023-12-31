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
    id = resource.network.local.id
  }

  health_check {
    timeout = "30s"

    exec {
      script = <<-EOF
      #!/bin/bash
      pg_isready
      EOF
    }
  }
}