variable "vault_network" {
  default = "resource.network.local"
}

variable "vault_version" {
  default = "1.14.1"
}

variable "vault_root_token" {
  default = "root"
}

# Should the Vault ui be opened in the browser after run
variable "vault_open_browser" {
  default = ""
}

# Optional IP address to add to Vault
variable "vault_ip_address" {
  default = ""
}

# Path to a folder that is mounted into the Vault container at path /data
variable "vault_data" {
  default = data("vault_data")
}

variable "vault_plugin_folder" {
  default     = data("vault_plugins")
  description = "Folder where vault will load custom plugins"
}

variable "vault_additional_volume" {
  description = "Additional volume to mount to the vault server"

  default = {
    name        = ""
    source      = data("vault_additional_data")
    destination = "/additional_data"
    type        = "bind"
  }
}

# Bootstrap script that is executed after Vault starts
# can be used to initially configure Vault
variable "vault_bootstrap_script" {
  default = <<-EOF
  #/bin/sh -e

  vault status
  EOF
}

variable "postgres_password" {
  default = "password"
}

variable "postgres_db" {
  default = "Fighters-6558502e-64dd-43fe-99d8-b519b4408469"
}