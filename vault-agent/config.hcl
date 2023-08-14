vault {
  address = "http://localhost:8200"
  retry {
    num_retries = 5
  }
}

auto_auth {
  method {
    type = "approle"

    config = {
      role_id_file_path                   = ".role-id"
      secret_id_file_path                 = ".secret-id"
      remove_secret_id_file_after_reading = false
    }
  }
}

template {
  contents = <<-EOF
  {
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
      {{ with secret "database/creds/fighters" -}}
        "Fighters": "server=localhost;username={{ .Data.username }};password={{ .Data.password }};database=Fighters-6558502e-64dd-43fe-99d8-b519b4408469"
      {{- end }}
    }
  }
  EOF

  destination = "../app/appsettings.json"
}