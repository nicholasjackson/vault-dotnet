# Vault Config Provider for C#

This repository is a proof of concept Config Provider that uses HashiCorp Vault secrets.
At present only AppRole and Database secrets are implemented.

## Pre-requisites
* Jumppad - [https://jumppad.dev/docs/introduction/installation](https://jumppad.dev/docs/introduction/installation)
* Docker - [https://www.docker.com/](https://www.docker.com/)
* dotnet - (brew install dotnet)

## Running

To create the environment and to build the application run the following command

```shell
cd ./jumppad
jumppad up
```

```shell
Running configuration from:  ./

INFO Creating resources from configuration path=/Users/nic/code/github.com/nicholasjackson/vault-dotnet/jumppad
INFO Creating ImageCache ref=resource.image_cache.default
INFO Creating output ref=output.VAULT_ADDR
INFO Building image context=/Users/nic/code/github.com/nicholasjackson/vault-dotnet/app dockerfile=Dockerfile.build image=jumppad.dev/localcache/app:HOrohq5J
INFO Please wait, still creating resources [Elapsed Time: 15.001052]
INFO Creating Network ref=resource.network.local
INFO Creating output ref=output.VAULT_TOKEN
INFO Creating Container ref=resource.container.vault
INFO Creating Container ref=resource.container.postgres
INFO Remote executing script ref=resource.remote_exec.vault_bootstrap
```

This will create the following resources in Docker

* Vault Server url: http://localhost:8200 token: root 
* Postgres Server url: localhost:5432 user: postgres password: password database: Fighters-6558502e-64dd-43fe-99d8-b519b4408469

## Configuration

The Vault cluster is configured with the role `database/role/fighters` that enables dynamic 
database credentials to be issued for the Postgres database. To use this role from the application
authentication has been configured using `AppRole` and the role `auth/approle/role/fighters` that 
has access to use the database secret.

When you run `jumppad up` Vault is automatically configured and the approle `role-id` and `secret-id`
that the application needs to authenticate to Vault are written to the main folder in this repository.

To see the full Vault configuration you can look at the script `./jumppad/setup.sh`.

## Running the application

To start the application run the following command

```shell
cd ./bin
dotnet ./VaultDotNet.dll
```

```shell
info: Vault.Configuration.VaultConfigurationProvider[0]
      Created VaultConfiguraionProvider
dbug: Vault.Configuration.VaultConfigurationProvider[0]
      Authenticating with App Role
dbug: Vault.Configuration.VaultConfigurationProvider[0]
      lease duration 0, renewable False
dbug: Vault.Configuration.VaultConfigurationProvider[0]
      rate limited getting secret database/creds/fighters
info: Vault.Configuration.VaultConfigurationProvider[0]
      db_username v-approle-fighters-79CJyWH08VfXFNiNpRcP-1692103895
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (14ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT EXISTS (SELECT 1 FROM pg_catalog.pg_class c JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace WHERE c.relname='__EFMigrationsHistory');
```

When the applications starts it will attempt to authenticate to Vault using the
configured auth method in the `Auth` section of `./bin/appsettings.json` file.

It will then attempt to request the secrets defined in `Secrets` section of the file
before storing them as configuration items. This is completed using the VaultConfigurationProvider.
`./app/Vault.Configuration/VaultConfigurationProvider.cs`.

The Database context `./app/Data/Fighters.cs` is then created and will use the `db_username` and `db_password`
context items set from the VaultConfigurationProvider to create a database connection string.

The system then runs the migrations to create the database.

You can access the application by opening `http://localhost:5000/Fighters` in your browser.

## Secrets management

The secrets are configured using a short lease of 1 minute and are automatically
managed by the `VaultConfigurationProvider`. When the secret lease is about to expire
`VaultConfigurationProvider` automatically will renew the secret and update the context.
The update process is managed using a background process in `./app/Vault.Configuration/VaultSecretWatcher.cs`

## Destroying the environment

To stop the environment and cleanup any contains run the following command.

```shell
jumppad down
```

Additionally you can run the following command to remove any Docker containers 
Jumppad downloads.

```shell
jummpad purge
```